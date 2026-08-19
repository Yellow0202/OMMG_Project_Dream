using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using OMMG.Character;
using OMMG.Inventory;

namespace OMMG.Interaction
{
    /// <summary>
    /// 플레이어 전용 상호작용 입력 드라이버.
    /// 범위 안에 들어온 InteractionTarget들을 추적하고, 상호작용 키(E)를 누르면
    /// 가장 가까운 대상 하나만 상호작용시킨다. 대화(DialogueRunner)가 진행 중이면
    /// 상호작용 키가 새 대상을 찾는 대신 대화 진행에 쓰인다(선택지 표시 중엔 무시됨).
    ///
    /// 대화 중 이동 방지 규칙(둘 다 적용):
    /// 1) 대화가 진행 중인 동안에는 플레이어 이동 입력(PlayerInputMover)을 비활성화해서
    ///    애초에 범위를 벗어날 수 없게 한다.
    /// 2) 그럼에도(넉백 등 예외 상황으로) 대화를 열어준 대상의 상호작용 범위를 벗어나면
    ///    대화를 강제로 종료한다.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        private readonly List<InteractionTarget> targetsInRange = new List<InteractionTarget>();

        private PlayerInputMover inputMover;

        /// <summary>현재 진행 중인 대화를 시작시킨 대상. 대화가 끝나면 null.</summary>
        private InteractionTarget dialogueOwner;

        private bool subscribedToDialogueRunner;

        public bool HasTarget => targetsInRange.Count > 0;

        private void Awake()
        {
            inputMover = GetComponent<PlayerInputMover>();
        }

        public void Register(InteractionTarget target)
        {
            if (!targetsInRange.Contains(target)) targetsInRange.Add(target);
            UpdatePromptVisibility();
        }

        public void Unregister(InteractionTarget target)
        {
            targetsInRange.Remove(target);
            UpdatePromptVisibility();

            // 대화를 열어준 대상이 범위를 벗어나면 대화를 강제 종료한다.
            if (target == dialogueOwner && DialogueRunner.Instance != null && DialogueRunner.Instance.IsActive)
            {
                DialogueRunner.Instance.ForceClose();
                dialogueOwner = null;
                SyncMovementLock();
            }
        }

        private void Update()
        {
            TrySubscribeToDialogueRunner();

            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (!keyboard.eKey.wasPressedThisFrame) return;

            TryInteract();
        }

        /// <summary>
        /// DialogueRunner.OnStateChanged를 구독해서 이동 잠금을 다시 동기화한다.
        /// SelectChoice처럼 E 키를 거치지 않고 대화가 즉시 끝나는 경로(선택지 선택 등)에서도
        /// 잠금이 풀리도록 하기 위함. DialogueRunner가 이 컴포넌트보다 닊게 초기화될 수 있어
        /// Update에서 될 때까지 재시도한다.
        /// </summary>
        private void TrySubscribeToDialogueRunner()
        {
            if (subscribedToDialogueRunner || DialogueRunner.Instance == null) return;
            DialogueRunner.Instance.OnStateChanged += SyncMovementLock;
            subscribedToDialogueRunner = true;
        }

        /// <summary>상호작용 키 입력 시 실행되는 로직. 테스트 코드에서 직접 호출할 수도 있다.</summary>
        public void TryInteract()
        {
            if (InventoryUI.Instance != null && InventoryUI.Instance.IsOpen) return; // 인벤토리가 열려있는 동안에는 상호작용 키 무시

            if (DialogueRunner.Instance != null && DialogueRunner.Instance.IsActive)
            {
                DialogueRunner.Instance.Advance();
                if (!DialogueRunner.Instance.IsActive) dialogueOwner = null;
                SyncMovementLock();
                return;
            }

            var closest = FindClosestTarget();
            if (closest != null)
            {
                closest.Interact(gameObject);
                if (DialogueRunner.Instance != null && DialogueRunner.Instance.IsActive) dialogueOwner = closest;
            }
            SyncMovementLock();
        }

        /// <summary>대화가 진행 중인 동안 플레이어 이동 입력을 잠그고, 끝나면 풀어준다.</summary>
        private void SyncMovementLock()
        {
            if (inputMover == null) return;

            bool dialogueActive = DialogueRunner.Instance != null && DialogueRunner.Instance.IsActive;
            inputMover.enabled = !dialogueActive;
        }

        private InteractionTarget FindClosestTarget()
        {
            InteractionTarget closest = null;
            float closestDist = float.MaxValue;

            for (int i = targetsInRange.Count - 1; i >= 0; i--)
            {
                var t = targetsInRange[i];
                if (t == null || !t.gameObject.activeInHierarchy)
                {
                    targetsInRange.RemoveAt(i);
                    continue;
                }

                float dist = (t.transform.position - transform.position).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = t;
                }
            }

            return closest;
        }

        private void UpdatePromptVisibility()
        {
            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.SetVisible(HasTarget);
            }
        }
    }
}
