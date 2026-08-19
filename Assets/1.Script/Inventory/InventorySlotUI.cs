using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OMMG.Interaction;

namespace OMMG.Inventory
{
    /// <summary>
    /// 인벤토리 창 안의 슬롯 하나. 아이콘 + 개수 배지를 표시하고,
    /// 마우스 호버 시 ItemTooltip을 그 슬롯 위로 옮겨서 데이터를 채워 보여준다.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>ItemTooltip을 생성/재사용할 때 부모로 쓸 레이어. InventoryUI가 초기화 시 채워준다.</summary>
        public static Transform TooltipLayer;

        [SerializeField] private Image iconImage;
        [SerializeField] private Text countText;

        private RectTransform rectTransform;
        private ItemData boundItem;
        private int boundCount;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Bind(ItemData item, int count)
        {
            boundItem = item;
            boundCount = count;

            if (iconImage != null)
            {
                iconImage.sprite = item.Icon;
                iconImage.enabled = item.Icon != null;
            }

            if (countText != null)
            {
                countText.text = count > 1 ? ("x" + count) : "";
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (boundItem == null) return;
            ItemTooltip.Show(rectTransform, boundItem, boundCount, TooltipLayer);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ItemTooltip.Hide();
        }

        private void OnDisable()
        {
            // 슬롯이 비활성화되는데(재구성 등) 마우스가 그 위에 있었다면 툴팁이 남지 않도록 함
            ItemTooltip.Hide();
        }
    }
}
