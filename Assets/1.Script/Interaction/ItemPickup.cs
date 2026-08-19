using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 상호작용 시 아이템을 "획득했다"는 사실만 알리는 컴포넌트.
    /// 실제 인벤토리에 담는 처리는 이후 과제이며, 지금은 어떤 아이템인지(데이터)와
    /// 획득/개방 상태만 다룬다.
    /// </summary>
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [Tooltip("이 오브젝트가 나타내는 아이템 데이터")]
        [SerializeField] private ItemData item;

        [Tooltip("true면 획득 시 오브젝트가 사라진다(열쇠 등). false면 자리에 남아있는다(상자 등).")]
        [SerializeField] private bool consumeOnInteract = true;

        [Tooltip("상자처럼 자리에 남는 경우, 이미 연 상태를 표시하기 위한 색상")]
        [SerializeField] private Color openedTint = new Color(0.5f, 0.5f, 0.5f, 1f);

        private bool isCollected;

        public bool IsCollected => isCollected;

        public void OnInteract(GameObject player)
        {
            if (isCollected)
            {
                if (ItemGetPopup.Instance != null) ItemGetPopup.Instance.Show("이미 비어있습니다.");
                return;
            }

            isCollected = true;

            string label = item != null ? item.DisplayName : "알 수 없는 아이템";
            if (ItemGetPopup.Instance != null) ItemGetPopup.Instance.Show(label + "을(를) 획득했습니다!");

            if (consumeOnInteract)
            {
                gameObject.SetActive(false);
            }
            else
            {
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = openedTint;
            }
        }
    }
}
