using System.Collections.Generic;
using UnityEngine;

namespace OMMG.Inventory
{
    /// <summary>
    /// 인벤토리 창 싱글턴. 연출 없이 panelRoot의 SetActive on/off로만 열고 닫는다.
    /// 열릴 때마다(그리고 내용이 바끈 때마다) PlayerInventory 데이터를 기준으로 슬롯을 다시 그린다.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }

        [SerializeField] private GameObject panelRoot;
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private InventorySlotUI slotTemplate;

        private readonly List<InventorySlotUI> spawnedSlots = new List<InventorySlotUI>();
        private bool subscribed;

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

        private void Awake()
        {
            Instance = this;
            InventorySlotUI.TooltipLayer = transform;
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void Start()
        {
            TrySubscribe();
        }

        private void Update()
        {
            // PlayerInventory 싱글턴이 이 컴포넌트보다 닊게 초기화되는 경우를 대비한 안전장치.
            if (!subscribed) TrySubscribe();
        }

        private void TrySubscribe()
        {
            if (subscribed || PlayerInventory.Instance == null) return;
            PlayerInventory.Instance.OnChanged += Rebuild;
            subscribed = true;
        }

        public void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }

        public void Open()
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            Rebuild();
        }

        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            ItemTooltip.Hide();
        }

        private void Rebuild()
        {
            for (int i = 0; i < spawnedSlots.Count; i++)
            {
                if (spawnedSlots[i] != null) Destroy(spawnedSlots[i].gameObject);
            }
            spawnedSlots.Clear();

            if (PlayerInventory.Instance == null || slotTemplate == null || slotContainer == null) return;

            var entries = PlayerInventory.Instance.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var slotGO = Instantiate(slotTemplate.gameObject, slotContainer);
                slotGO.SetActive(true);

                var slot = slotGO.GetComponent<InventorySlotUI>();
                slot.Bind(entry.Item, entry.Count);
                spawnedSlots.Add(slot);
            }
        }
    }
}

