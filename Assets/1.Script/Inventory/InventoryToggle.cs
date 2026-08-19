using UnityEngine;
using UnityEngine.InputSystem;
using OMMG.Character;
using OMMG.Interaction;

namespace OMMG.Inventory
{
    /// <summary>
    /// 플레이어 전용 인벤토리 열기/닫기 입력 드라이버.
    /// 상호작용 키(E)와는 별도의 키(I)를 사용해서 서로 충돌하지 않는다.
    /// 대화가 열려있는 동안에는 인벤토리를 열 수 없고, 인벤토리가 열려있는 동안에는
    /// 대화와 마찬가지로 플레이어 이동 입력을 잠그다.
    /// </summary>
    [RequireComponent(typeof(PlayerInputMover))]
    public class InventoryToggle : MonoBehaviour
    {
        private PlayerInputMover inputMover;

        private void Awake()
        {
            inputMover = GetComponent<PlayerInputMover>();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (!keyboard.iKey.wasPressedThisFrame) return;

            TryToggle();
        }

        /// <summary>인벤토리 토글 키 입력 시 실행되는 로직. 테스트 코드에서 직접 호출할 수도 있다.</summary>
        public void TryToggle()
        {
            if (DialogueBox.Instance != null && DialogueBox.Instance.IsOpen) return; // 대화 중엔 인벤토리 입력 무시

            if (InventoryUI.Instance == null) return;
            InventoryUI.Instance.Toggle();
            SyncMovementLock();
        }

        private void SyncMovementLock()
        {
            if (inputMover == null) return;

            bool inventoryOpen = InventoryUI.Instance != null && InventoryUI.Instance.IsOpen;
            inputMover.enabled = !inventoryOpen;
        }
    }
}

