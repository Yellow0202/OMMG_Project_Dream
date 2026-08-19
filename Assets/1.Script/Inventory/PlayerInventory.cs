using System;
using System.Collections.Generic;
using UnityEngine;
using OMMG.Interaction;

namespace OMMG.Inventory
{
    /// <summary>
    /// 세션 동안 유지되는 아이템 인벤토리 데이터. 씬 하나에 하나만 존재하는 싱글턴.
    /// 같은 ItemData는 하나의 슬롯(엔트리)에 개수만 누적되는 스택형 구조다.
    /// UI는 이 클래스의 데이터만 읽고 그리며, 실제 보관/증감 로직은 전부 여기서 처리한다.
    /// (클래스 이름은 PlayerInventory. 소속 네임스페이스 OMMG.Inventory와 이름이 겹치면
    /// C# 컴파일러가 네임스페이스/타입을 혼동하므로 일부러 다르게 지었다.)
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        private readonly List<InventoryEntry> entries = new List<InventoryEntry>();

        /// <summary>현재 보유 중인 아이템 목록(읽기 전용). UI가 이 순서 그대로 슬롯을 그린다.</summary>
        public IReadOnlyList<InventoryEntry> Entries => entries;

        /// <summary>인벤토리 내용이 바뀔 때마다(추가/증가) 호출된다. UI가 이 이벤트로 다시 그린다.</summary>
        public event Action OnChanged;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>아이템을 인벤토리에 더한다. 이미 있는 아이템이면 개수만 증가한다.</summary>
        public void AddItem(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0) return;

            var existing = FindEntry(item);
            if (existing != null)
            {
                existing.Count += amount;
            }
            else
            {
                entries.Add(new InventoryEntry(item, amount));
            }

            OnChanged?.Invoke();
        }

        private InventoryEntry FindEntry(ItemData item)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Item == item) return entries[i];
            }
            return null;
        }
    }

    /// <summary>인벤토리 한 슬롯(아이템 데이터 + 보유 개수)을 나타내는 엔트리.</summary>
    public class InventoryEntry
    {
        public ItemData Item { get; }
        public int Count { get; set; }

        public InventoryEntry(ItemData item, int count)
        {
            Item = item;
            Count = count;
        }
    }
}
