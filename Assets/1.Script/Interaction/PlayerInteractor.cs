using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using OMMG.Character;

namespace OMMG.Interaction
{
    /// <summary>
    /// 플레이어 전용 상호작용 입력 드라이버.
    /// 범위 안에 들어온 InteractionTarget들을 추적하고, 상호작용 키(E)를 누르면
    /// 가장 가까운 대상 하나만 상호작용시킨다. 대화창이 열려있는 동안에는
    /// 상호작용 키가 새 대상을 찾는 대신 대화 진행에 쓰인다.
    ///
    /// 대화 중 이동 방지 규칙(둘 다 적용):
    /// 1) 대화가 열려있는 동안에는 플레이어 이동 입력(PlayerInputMover)을 비활성화해서
    ///    애초에 범위를 벗어날 수 없게 한다.
    /// 2) 그럼에도(넉백 등 예외 상황으로) 대화를 열어준 대상의 상호작용 범위를 벗어나면
    ///    대화를 강제로 종료한다.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        private readonly List<InteractionTarget> targetsInRange = new List<InteractionTarget>();

        private PlayerInputMover inputMover;

        /// <summary>현재 열려있는 대화를 시작시킨 대상. 대화가 닫히면 null.</summary>
        private InteractionTarget dialogueOwner;

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
            if (target == dialogueOwner && DialogueBox.Instance != null && DialogueBox.Instance.IsOpen)
            {
                DialogueBox.Instance.Close();
                dialogueOwner = null;
                SyncMovementLock();
            }
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (!keyboard.eKey.wasPressedThisFrame) return;

            TryInteract();
        }

        /// <summary>상호작용 키 입력 시 실행되는 로직. 테스트 코드에서 직접 호출할 수도 있다.</summary>
        public void TryInteract()
        {
            if (DialogueBox.Instance != null && DialogueBox.Instance.IsOpen)
            {
                DialogueBox.Instance.AdvanceOrClose();
                if (!DialogueBox.Instance.IsOpen) dialogueOwner = null;
                SyncMovementLock();
                return;
            }

            var closest = FindClosestTarget();
            if (closest != null)
            {
                closest.Interact(gameObject);
                if (DialogueBox.Instance != null && DialogueBox.Instance.IsOpen) dialogueOwner = closest;
            }
            SyncMovementLock();
        }

        /// <summary>대화가 열려있는 동안 플레이어 이동 입력을 잠그고, 닫히면 풀어준다.</summary>
        private void SyncMovementLock()
        {
            if (inputMover == null) return;

            bool dialogueOpen = DialogueBox.Instance != null && DialogueBox.Instance.IsOpen;
            inputMover.enabled = !dialogueOpen;
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

