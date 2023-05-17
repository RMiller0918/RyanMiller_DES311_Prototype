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
        _finished = _ctx.NewTeleSetUpRequired;
        Debug.Log("Entered Teleport");
    }

    public override void UpdateState()
    {
        if (!_ctx.TeleportSetUp)
            _finished = true;
        CheckSwitchState();
        if (_finished) return;
        Debug.Log("Firing Ray");

        var color = _ctx.TeleMarker.IsValid ? Color.green : Color.red;
        _ctx.TeleMarker.SetColor(color);

        switch (_ctx.Attacking)
        {
            case true when _ctx.TeleMarker.IsValid:
                Teleport();
                break;
            default:
                FireRay();
                break;
        }
    }

    public override void ExitState()
    {
        _ctx.TeleMarker.ResetPosition();
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
        if (_ctx.TeleportSetUp)
            _ctx.NewTeleSetUpRequired = true;
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
        var mask = LayerMask.GetMask("UI") | LayerMask.GetMask("Ignore Raycast");
        Physics.Raycast(position, cameraTransform.forward, out var hitInfo, _range, ~mask);

        endPosition = hitInfo.transform != null ? hitInfo.point : endPosition;
        _targetLocation = endPosition;
        _ctx.TeleMarker.SetPosition(endPosition);
        Debug.DrawLine(position, endPosition, Color.green);
    }

    private void Teleport()
    {
        _ctx.Mana -= 25;
        _ctx.MBar.UpdateHealthBar(_ctx.MaxMana, _ctx.Mana);
        _ctx.CharCont.enabled = false;
        _ctx.transform.position = _targetLocation;
        _ctx.CharCont.enabled = true;
        Physics.SyncTransforms();
        _finished = true;
    }
}
