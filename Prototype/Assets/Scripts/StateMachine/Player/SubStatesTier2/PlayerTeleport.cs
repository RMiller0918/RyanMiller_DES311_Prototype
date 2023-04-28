using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : PlayerBaseState
{
    private bool _finished;
    private bool _validTarget;
    private const float _range = 10f;
    private Vector3 _targetLocation;
    public PlayerTeleport(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _finished = false;
        Debug.Log("Entered Teleport");
    }

    public override void UpdateState()
    {
        if (!_ctx.TeleportSetUp)
            _finished = true;
        CheckSwitchState();
        if (_finished) return;
        Debug.Log("Firing Ray");
        switch(_ctx.Attacking)
        {
            case true:
                Teleport();
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
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchState()
    {
        if(_finished)
            SwitchState(_factory.Empty());
    }

    private void FireRay()
    {
        var cameraTransform = Camera.main.transform;
        var position = cameraTransform.position;
        var endPosition = position + cameraTransform.forward * _range;

        Physics.Raycast(position, cameraTransform.forward, out var hitInfo, _range);

        endPosition = hitInfo.transform != null ? hitInfo.point : endPosition;
        _targetLocation = endPosition;
        Debug.DrawLine(position, endPosition, Color.green);
    }

    private void Teleport()
    {
        _ctx.CharCont.enabled = false;
        _ctx.transform.position = _targetLocation;
        _ctx.CharCont.enabled = true;
        Physics.SyncTransforms();
        _finished = true;
    }
}
