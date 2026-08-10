using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class PlayerInputReader : MonoBehaviour
{
    private const string GameplayMapName = "Gameplay";
    private const string MoveActionName = "Move";
    private const string DownActionName = "Down";
    private const string JumpActionName = "Jump";
    private const string AttackActionName = "Attack";
    private const string RangedAttackActionName = "RangedAttack";

    private InputActionMap gameplayActions;
    private InputAction moveAction;
    private InputAction downAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction rangedAttackAction;

    internal float MoveX => Mathf.Clamp(moveAction.ReadValue<float>(), -1f, 1f);
    internal bool DownHeld => downAction.IsPressed();
    internal bool JumpPressed => jumpAction.WasPressedThisFrame();
    internal bool JumpHeld => jumpAction.IsPressed();
    internal bool AttackPressed => attackAction.WasPressedThisFrame();
    internal bool RangedAttackPressed => rangedAttackAction.WasPressedThisFrame();

    private void Awake()
    {
        InputActionAsset inputActions = InputSystem.actions;

        if (inputActions == null)
        {
            Debug.LogError("[PlayerInputReader] No hay un asset de Input Actions asignado como Project-wide.", this);
            enabled = false;
            return;
        }

        gameplayActions = inputActions.FindActionMap(GameplayMapName, true);
        moveAction = gameplayActions.FindAction(MoveActionName, true);
        downAction = gameplayActions.FindAction(DownActionName, true);
        jumpAction = gameplayActions.FindAction(JumpActionName, true);
        attackAction = gameplayActions.FindAction(AttackActionName, true);
        rangedAttackAction = gameplayActions.FindAction(RangedAttackActionName, true);

        if (!gameplayActions.enabled)
            gameplayActions.Enable();
    }
}
