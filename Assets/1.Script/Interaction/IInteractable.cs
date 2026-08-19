using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 플레이어가 상호작용 키를 눌렀을 때 반응하는 컴포넌트가 구현하는 인터페이스.
    /// 하나의 오브젝트에 여러 개(ItemPickup, DialogueSource, EventTrigger 등)를 동시에 붙일 수 있으며,
    /// InteractionTarget이 상호작용 시점에 이 인터페이스를 구현한 모든 컴포넌트를 순서대로 호출한다.
    /// </summary>
    public interface IInteractable
    {
        void OnInteract(GameObject player);
    }
}
