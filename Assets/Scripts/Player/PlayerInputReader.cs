using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class PlayerInputReader : MonoBehaviour
{
    private const string GameplayMapName = "Gameplay";
    private const string MoveActionName = "Move";
    private const string JumpActionName = "Jump";
    private const string AttackActionName = "Attack";

    private InputActionMap gameplayActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;

    public float MoveX => Mathf.Clamp(moveAction.ReadValue<float>(), -1f, 1f);
    public bool JumpPressed => jumpAction.WasPressedThisFrame();
    public bool JumpHeld => jumpAction.IsPressed();
    public bool JumpReleased => jumpAction.WasReleasedThisFrame();
    public bool AttackReleased => attackAction.WasReleasedThisFrame();

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
        jumpAction = gameplayActions.FindAction(JumpActionName, true);
        attackAction = gameplayActions.FindAction(AttackActionName, true);

        if (!gameplayActions.enabled)
            gameplayActions.Enable();
    }
}
