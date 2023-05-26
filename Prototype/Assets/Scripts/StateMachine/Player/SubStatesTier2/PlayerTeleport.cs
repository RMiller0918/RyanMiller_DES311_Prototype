using UnityEngine;

public class PlayerTeleport : PlayerBaseState
{
    private bool _finished;
    private bool _targetSet;
    private const float _range = 10f;
    private Vector3 _targetLocation;
    private Vector3 _direction;

    public PlayerTeleport(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState() //Play the initial animation and switch the Teleport UI Icon. 
    {
        _ctx.TeleportIcon.SpriteChange(1);
        _isActive = true;
        _ctx.Animator.SetBool(_ctx.TeleportSetupHash, true);
        _finished = false;
        _ctx.IsTeleporting = false;
        _targetSet = false;
    }

    public override void UpdateState()
    {
        if ((_ctx.Lit || !_ctx.TeleportSetUp) && !_ctx.IsTeleporting) //set the action to be finished if the player is Lit or the player lets go of Q, only do this if the player isn't already teleporting.
            _finished = true;
        var color = _ctx.TeleMarker.IsValid ? Color.green : Color.red; //Check the target location is valid and change marker colour.
        _ctx.TeleMarker.SetColor(color);
        if (_finished) //reset the time scale to 1 and switch back to empty state
        {
            Time.timeScale = 1f;
            CheckSwitchState();
            return;
        }

        if (!_targetSet) //if the target location hasn't been set, slow time and fire out a raycast.
        {
            Time.timeScale = 0.1f;
            Debug.Log(_targetSet);
            var endPosition = FireRay();
            return;
        }

        if (_targetSet && !_ctx.IsTeleporting) //Set the teleport location
        {
            SetTeleport();
            return;
        }

        if (_ctx.IsTeleporting) //Teleport the player to the target location.
        {
            _ctx.TeleMarker.SetPosition(_targetLocation);
            _ctx.CharCont.detectCollisions = false;
            Teleport();
        }
    }

    public override void ExitState() //Return the Empty State. Initiate Repeat action blockers for Attacking and Teleporting if the player is still holding any of the corresponding buttons. Prevents player spamming abilities on exit. 
    {
        _ctx.TeleportIcon.SpriteChange(0);
        Camera.main.fieldOfView = 90;
        _ctx.Animator.SetBool(_ctx.TeleportSetupHash, false);
        _ctx.IsTeleporting = false;
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
        if (_ctx.TeleportSetUp)
            _ctx.NewTeleSetUpRequired = true;
        _ctx.CharCont.detectCollisions = true;
        _isActive = false;
    }

    public override void InitializeSubState() { } //No other state layers

    public override void CheckSwitchState() //Can only switch back to the Empty state
    {
        SwitchState(_factory.Empty());
    }

    /*
     * Fires a ray from the camera.
     * Set the end position to either the end of the ray or the point the ray hits an object.
     * Check the teleport marker is in a valid location (Shadows) and if the player presses attack Set the target location. 
     */

    private Vector3 FireRay() 
    {
        var cameraTransform = Camera.main.transform;
        var position = cameraTransform.position;
        var endPosition = position + cameraTransform.forward * _range;
        var mask = LayerMask.GetMask("UI") | LayerMask.GetMask("Ignore Raycast");
        Physics.Raycast(position, cameraTransform.forward, out var hitInfo, _range, ~mask);
        endPosition = hitInfo.transform != null ? hitInfo.point : endPosition;
        _ctx.TeleMarker.SetPosition(endPosition);
        if (_ctx.Attacking && !_targetSet && _ctx.TeleMarker.IsValid)
        {
            _targetLocation = endPosition;
            _targetSet = true;
        }

        return endPosition;
    }

    private void Teleport() //Move the player to the target location, Uses lerp instead of an straight location change. Makes the teleport feel less jarring. 
    {
        var distance = Vector3.Distance(_ctx.transform.position, _targetLocation);
        _ctx.CharCont.enabled = false;
        _ctx.transform.position = Vector3.Lerp(_ctx.transform.position, _targetLocation, (100 / (distance)) * Time.unscaledDeltaTime);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 100, 1 / distance * Time.unscaledDeltaTime);
        if (distance > 0.5f) return;
        _ctx.CharCont.enabled = true;
        _finished = true;
        _ctx.IsTeleporting = false;
        _ctx.TeleMarker.ResetPosition();
    }

    private void SetTeleport() //Plays the teleport animation and removes mana from the player, updating the MP bar. 
    {
        if (_ctx.NewAttackRequired) return;
        _ctx.Animator.SetTrigger(_ctx.TeleportTriggerHash);
        _ctx.Mana -= 25;
        _ctx.Mana = Mathf.Clamp(_ctx.Mana, 0, _ctx.Mana);
        _ctx.MBar.UpdateHealthBar(_ctx.MaxMana, _ctx.Mana);
        _ctx.IsTeleporting = true;
    }

    private Vector3 MoveDirection(Vector3 startPos, Vector3 endPos)
    {
        return endPos - startPos;
    }
}
