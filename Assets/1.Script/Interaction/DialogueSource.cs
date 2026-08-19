using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 상호작용 시 정해진 대사를 DialogueBox에 출력하는 컴포넌트.
    /// </summary>
    public class DialogueSource : MonoBehaviour, IInteractable
    {
        [Tooltip("말하는 이 이름 (비워두면 표시 안 함)")]
        [SerializeField] private string speakerName = "";

        [Tooltip("순서대로 출력될 대사 목록")]
        [SerializeField, TextArea(2, 4)] private string[] lines = { "..." };

        public void OnInteract(GameObject player)
        {
            if (DialogueBox.Instance != null) DialogueBox.Instance.Show(speakerName, lines);
        }
    }
}
