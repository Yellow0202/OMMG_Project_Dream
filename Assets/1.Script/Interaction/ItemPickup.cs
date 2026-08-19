using UnityEngine;
using OMMG.Inventory;

namespace OMMG.Interaction
{
    /// <summary>
    /// 상호작용 시 아이템을 실제로 인벤토리에 넣어주는 컴포넌트.
    /// consumeOnInteract가 true면 오브젝트가 사라지고(열쇠 등),
    /// false면 자리에 남아 색상만 바뀜다(상자 등).
    /// </summary>
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [Tooltip("이 오브젝트가 나타내는 아이템 데이터")]
        [SerializeField] private ItemData item;

        [Tooltip("true면 획득 시 오브젝트가 사라진다(열쇠 등). false면 자리에 남아있는다(상자 등).")]
        [SerializeField] private bool consumeOnInteract = true;

        [Tooltip("상자처럼 자리에 남는 경우, 이미 연 상태를 표시하기 위한 색상")]
        [SerializeField] private Color openedTint = new Color(0.5f, 0.5f, 0.5f, 1f);

        [Tooltip("획득 시 인벤토리에 들어갈 개수")]
        [SerializeField, Min(1)] private int amount = 1;

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

            if (PlayerInventory.Instance != null) PlayerInventory.Instance.AddItem(item, amount);

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

