using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 오브젝트 주위에 상호작용 가능 범위(트리거 콜라이더)를 정의하고,
    /// 플레이어가 범위에 들어오고 나가는 걸 감지해서 PlayerInteractor에 등록/해제한다.
    /// 실제 상호작용 동작은 같은 오브젝트에 붙은 IInteractable 컴포넌트들이 담당한다.
    /// Collider2D(Is Trigger = true)가 필요하다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteractionTarget : MonoBehaviour
    {
        [Tooltip("트리거에 반응할 대상의 태그")]
        [SerializeField] private string playerTag = "Player";

        private IInteractable[] interactables;

        private void Awake()
        {
            interactables = GetComponents<IInteractable>();
        }

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            var interactor = other.GetComponent<PlayerInteractor>();
            if (interactor != null) interactor.Register(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            var interactor = other.GetComponent<PlayerInteractor>();
            if (interactor != null) interactor.Unregister(this);
        }

        /// <summary>이 오브젝트에 붙은 모든 상호작용 동작을 실행한다.</summary>
        public void Interact(GameObject player)
        {
            if (interactables == null) interactables = GetComponents<IInteractable>();

            for (int i = 0; i < interactables.Length; i++)
            {
                interactables[i].OnInteract(player);
            }
        }
    }
}
