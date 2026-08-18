using UnityEngine;
using UnityEngine.InputSystem;

namespace OMMG.Character
{
    /// <summary>
    /// 플레이어 전용 입력 드라이버.
    /// 이동 로직(CharacterMover)과는 완전히 분리되어 있으며,
    /// 이 스크립트가 하는 일은 "키 입력을 읽어서 CharacterMover.SetMoveInput()을 호출하는 것" 뿐이다.
    /// New Input System의 이벤트 콜백(performed/canceled)을 사용하므로
    /// Update()를 직접 폴링하지 않는다. 방향키 / WASD를 모두 지원한다.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterMover))]
    public class PlayerInputMover : MonoBehaviour
    {
        private CharacterMover mover;
        private InputAction moveAction;

        private void Awake()
        {
            mover = GetComponent<CharacterMover>();

            moveAction = new InputAction(name: "Move", type: InputActionType.Value, expectedControlType: "Vector2");
            moveAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/upArrow")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/downArrow")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/rightArrow")
                .With("Right", "<Keyboard>/d");

            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
        }

        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            mover.SetMoveInput(Vector2.zero);
        }

        private void OnDestroy()
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
            moveAction.Dispose();
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            mover.SetMoveInput(ctx.ReadValue<Vector2>());
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            mover.SetMoveInput(Vector2.zero);
        }
    }
}

