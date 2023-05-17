using UnityEditorInternal;
using UnityEngine;

public class PlayerPullEnemy : PlayerBaseState
{
    private enum Mode
    {
        selectingTarget,
        selectingLocation
    }

    private bool _finished;
    private GameObject chosenEnemy;
    private Mode _mode;
    private Vector3 _targetLocation;
    private float _range = 10f;

    public PlayerPullEnemy(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _isActive = true;
        Debug.Log("Entered shadow pull state");
        _ctx.TeleMarker.ResetPosition();
        _mode = Mode.selectingTarget;
        _finished = false;
        chosenEnemy = null;
    }

    public override void UpdateState()
    {
        if (!_ctx.PullEnemySetUp)
            _finished = true;
        CheckSwitchState();
        if (_finished)
        {
            _ctx.TeleMarker.ResetPosition();
            return;
        }
        var color = _ctx.TeleMarker.IsValid ? Color.green : Color.red;
        _ctx.TeleMarker.SetColor(color);
        switch (_ctx.Attacking)
        {
            case true when (_mode == Mode.selectingLocation) && !_ctx.NewAttackRequired && _ctx.TeleMarker.IsValid:
                TeleportTarget();
                break;
            default:
                FireRay();
                break;
        }
    }

    public override void ExitState()
    {
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
        if (_ctx.PullEnemySetUp)
            _ctx.NewPullRequired = true;
        _ctx.TeleMarker.ResetPosition();
        _isActive = false;
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchState()
    {
        if(_finished)
            SwitchState(_factory.Empty());
    }

    private void SelectTarget(GameObject obj)
    {
        chosenEnemy = obj;
        _mode = Mode.selectingLocation;
        _ctx.NewAttackRequired = true;
        Debug.Log($"Target Selected {chosenEnemy.name}");
    }

    private void TeleportTarget()
    {
        _ctx.Mana -= 25;
        _ctx.Mana = Mathf.Clamp(_ctx.Mana, 0, _ctx.MaxMana);
        _ctx.MBar.UpdateHealthBar(_ctx.MaxMana, _ctx.Mana);
        chosenEnemy.GetComponent<CharacterController>().enabled = false;
        var height = chosenEnemy.GetComponent<CharacterController>().height / 2;
        chosenEnemy.transform.position = _targetLocation + new Vector3(0,height,0);
        chosenEnemy.GetComponent<CharacterController>().enabled = true;
        Physics.SyncTransforms();
        _finished = true;
    }

    private void FireRay()
    {
        var mask = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Environment");
        var cameraTransform = Camera.main.transform;
        var position = cameraTransform.position;
        var endPosition = position + cameraTransform.forward * _range;
        switch (_mode)
        {
            case Mode.selectingTarget:
                Physics.Raycast(position, cameraTransform.forward, out var hitInfo, _range, mask);
                if (hitInfo.transform == null) return;
                if (hitInfo.transform.tag != "Enemy") return;
                if (hitInfo.transform.GetComponent<EnemyStateMachine>().Lit) return;
                if (_ctx.Attacking)
                    SelectTarget(hitInfo.transform.gameObject);
                break;
            case Mode.selectingLocation:
                Physics.Raycast(position, cameraTransform.forward, out var hit, _range, mask);
                endPosition = hit.transform != null ? hit.point : endPosition;
                _targetLocation = endPosition;
                _ctx.TeleMarker.SetPosition(endPosition);
                break;
        }
        /*
        if(Physics.Raycast(position, cameraTransform.forward, out hitInfo, _range, mask))
        {
            switch (_mode)
            {
                case Mode.selectingTarget:
                    Debug.Log(hitInfo.transform.tag);
                    if (hitInfo.transform.tag != "Enemy") return;
                    if (hitInfo.transform.GetComponent<EnemyStateMachine>().Lit) return;
                    if (_ctx.Attacking)
                        SelectTarget(hitInfo.transform.gameObject);
                    break;
                case Mode.selectingLocation:
                    endPosition = hitInfo.transform != null ? hitInfo.point : endPosition;
                    _targetLocation = endPosition;
                    _ctx.TeleMarker.SetPosition(endPosition);
                    break;
            }
        }
        */
    }
}