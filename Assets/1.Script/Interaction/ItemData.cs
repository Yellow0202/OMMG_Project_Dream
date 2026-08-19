using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 아이템 하나를 정의하는 데이터 전용 에셋.
    /// 인벤토리(OMMG.Inventory.Inventory)에 실제로 담기는 데이터이며,
    /// 이름/설명/아이콘을 인벤토리 UI와 툴팀에서 그대로 사용한다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemData", menuName = "OMMG/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemId = "item_new";
        [SerializeField] private string displayName = "새 아이템";

        [Tooltip("인벤토리 툴팀에 표시될 설명")]
        [SerializeField, TextArea(2, 4)] private string description = "";

        [Tooltip("인벤토리 슬롯에 표시될 아이콘")]
        [SerializeField] private Sprite icon;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
    }
}

