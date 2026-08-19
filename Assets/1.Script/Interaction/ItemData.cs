using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 아이템 하나를 정의하는 데이터 전용 에셋.
    /// 지금은 인벤토리에 실제로 담기지 않고, "이 아이템을 획득했다"는 사실만 표현하는 데 쓰인다.
    /// 인벤토리 시스템은 이후 별도 작업으로 추가될 예정.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemData", menuName = "OMMG/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemId = "item_new";
        [SerializeField] private string displayName = "새 아이템";

        public string ItemId => itemId;
        public string DisplayName => displayName;
    }
}
