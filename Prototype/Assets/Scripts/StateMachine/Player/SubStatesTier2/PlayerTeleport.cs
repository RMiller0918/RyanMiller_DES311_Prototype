using System;
using System.Collections;
using System.Collections.Generic;
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

    public override void EnterState()
    {
        _isActive = true;
        _ctx.Animator.SetBool(_ctx.TeleportSetupHash, true);
        _finished = false;
        _ctx.IsTeleporting = false;
        _targetSet = false;
        Debug.Log($"Entered Teleport State, Finished = {_finished}, Is Teleporting = {_ctx.IsTeleporting}");
    }

    public override void UpdateState()
    {
        if ((_ctx.Lit || !_ctx.TeleportSetUp) && !_ctx.IsTeleporting)
            _finished = true;
        if (_finished)
        {
            //Debug.Log(_ctx.IsTeleporting);
            CheckSwitchState();
            return;
        }

        if (!_targetSet)
        {
            //Debug.Log(_ctx.IsTeleporting);
            FireRay();
        }
        else if (_targetSet && !_ctx.IsTeleporting)
        {
            //Debug.Log(_ctx.IsTeleporting);
            SetTeleport();
        }

        if (_ctx.IsTeleporting)
        {
            //Debug.Log(_ctx.IsTeleporting);
            Teleport();
        }


        /*
        switch (_ctx.IsTeleporting)
        {
            case true when !_finished:
                Debug.Log($"Teleporting, {_ctx.IsTeleporting}, currently not finished {_finished}");
                Teleport();
                break;
            case false when _targetSet && !_finished:
                SetTeleport();
                Debug.Log($"I've chosen a target Location {_targetLocation}, {_ctx.IsTeleporting}");
                break;
            case false when !_finished:
                Debug.Log($"Picking target location, {_ctx.TeleMarker.IsValid}");
                var color = _ctx.TeleMarker.IsValid ? Color.green : Color.red;
                _ctx.TeleMarker.SetColor(color);
                FireRay();
                break;
            case false when _finished:
                Debug.Log($"Trying to switch out of the state {_finished}, no Longer teleporting {_ctx.IsTeleporting}");
                CheckSwitchState();
                break;
        }
        */



        /*
        CheckSwitchState();
        if (_finished) return;
        Debug.Log("Firing Ray");

        switch (_ctx.Attacking)
        {
            case true when _ctx.TeleMarker.IsValid:
                Teleport();
                break;
            default:
                FireRay();
                break;
        }
        */
    }

    public override void ExitState()
    {
        _ctx.Animator.SetBool(_ctx.TeleportSetupHash, false);
        _ctx.IsTeleporting = false;
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
        if (_ctx.TeleportSetUp)
            _ctx.NewTeleSetUpRequired = true;

        _isActive = false;
        Debug.Log($"I need to release Q {_ctx.NewTeleSetUpRequired}, I need to Release LeftClick {_ctx.NewAttackRequired}");
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchState()
    {
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
        _ctx.TeleMarker.SetPosition(endPosition);

        if (_ctx.Attacking && !_targetSet)
        {
            _targetLocation = endPosition;
            _targetSet = true;
        }
    }

    private void Teleport()
    {
        _ctx.CharCont.detectCollisions = false;
        var distance = Vector3.Distance(_ctx.transform.position, _targetLocation);
        distance = Mathf.Clamp(distance, 0.1f, 10);
        _ctx.CharCont.enabled = false;
        _ctx.transform.position = Vector3.Lerp(_ctx.transform.position, _targetLocation, 5f);
        Debug.Log($"{_ctx.transform.position}, {_targetLocation}");
        if (_ctx.transform.position != _targetLocation)
        {
            return;
        }
        _ctx.CharCont.enabled = true;
        _finished = true;
        _ctx.IsTeleporting = false;
        _ctx.TeleMarker.ResetPosition();
    }

    private void SetTeleport()
    {
        _ctx.Animator.SetTrigger(_ctx.TeleportTriggerHash);
        _ctx.Mana -= 25;
        _ctx.Mana = Mathf.Clamp(_ctx.Mana, 0, _ctx.Mana);
        _ctx.MBar.UpdateHealthBar(_ctx.MaxMana, _ctx.Mana);
        _direction = MoveDirection(_ctx.transform.position, _targetLocation);
        _ctx.IsTeleporting = true;
    }

    private Vector3 MoveDirection(Vector3 startPos, Vector3 endPos)
    {
        return endPos - startPos;
    }
}
