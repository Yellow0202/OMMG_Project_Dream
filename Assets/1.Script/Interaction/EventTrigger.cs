using UnityEngine;
using UnityEngine.Events;

namespace OMMG.Interaction
{
    /// <summary>
    /// 상호작용 시 인스펙터에 연결된 UnityEvent를 실행하는 범용 트리거.
    /// repeatable이 꺼져 있으면 최초 1회만 발동한다.
    /// </summary>
    public class EventTrigger : MonoBehaviour, IInteractable
    {
        [Tooltip("true면 상호작용할 때마다 매번 발동, false면 최초 1회만 발동")]
        [SerializeField] private bool repeatable = true;

        [SerializeField] private UnityEvent onInteract;

        private bool hasFired;

        public void OnInteract(GameObject player)
        {
            if (!repeatable && hasFired) return;

            hasFired = true;
            if (onInteract != null) onInteract.Invoke();
        }
    }
}
