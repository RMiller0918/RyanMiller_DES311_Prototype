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

    public override void EnterState() //Update the Shadow Pull UI Icon. Play initial animation
    {
        _ctx.ShadowPullIcon.SpriteChange(1);
        _isActive = true;
        Debug.Log("Entered shadow pull state");
        _ctx.Animator.SetBool(_ctx.TeleportSetupHash, true);
        _ctx.TeleMarker.ResetPosition();
        _mode = Mode.selectingTarget;
        _finished = false;
        chosenEnemy = null;
    }

    public override void UpdateState() //check which mode the player is in and update accordingly.
    {
        Debug.Log($"Updating State");
        if (!_ctx.PullEnemySetUp) //if the player releases E then finish the ability
            _finished = true;
        CheckSwitchState();
        if (_finished) //reset Teleport Marker
        {
            Debug.Log("Finished shadow pull");
            _ctx.TeleMarker.ResetPosition();
            return;
        }

        var color = _ctx.TeleMarker.IsValid ? Color.green : Color.red;
        switch (_ctx.Attacking) //check which mode the player is in when Left-Click is pressed. 
        {
            case true when (_mode == Mode.selectingLocation) && !_ctx.NewAttackRequired && _ctx.TeleMarker.IsValid:
                TeleportTarget();
                _ctx.TeleMarker.SetColor(color);
                break;
            default:
                FireRay();
                _ctx.TeleMarker.SetColor(color);
                break;
        }
    }

    public override void ExitState() //reset Icon, block repeating actions if corresponding controls are still pressed.
    {
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
        if (_ctx.PullEnemySetUp)
            _ctx.NewPullRequired = true;
        _ctx.TeleMarker.ResetPosition();
        _ctx.ShadowPullIcon.SpriteChange(0);
        _isActive = false;
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchState() //switch to empty state.
    {
        if(_finished)
            SwitchState(_factory.Empty());
    }

    private void SelectTarget(GameObject obj) //selects the enemy target being teleported.
    {
        chosenEnemy = obj;
        _mode = Mode.selectingLocation;
        _ctx.NewAttackRequired = true;
        Debug.Log($"Target Selected {chosenEnemy.name}");
        _ctx.ShadowPullIcon.SpriteChange(2);
    }

    private void TeleportTarget() //moves the target to the new location. unlike with player teleportation, this is instant and not lerped. 
    {
        _ctx.Animator.SetTrigger(_ctx.TeleportTriggerHash);
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

    private void FireRay() //Fire a ray, first to determine the target character, Second to determine the target location. In both instances if the target or location are in light then they are invalid.
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
                {
                    SelectTarget(hitInfo.transform.gameObject);
                }
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