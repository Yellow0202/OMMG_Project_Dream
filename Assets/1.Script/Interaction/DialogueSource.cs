using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 상호작용 시 대화 노드 그래프를 DialogueRunner에게 넘겨 시작시키는 컴포넌트.
    /// 실제 진행(대사 출력, 선택지 분기, 플래그 반영)은 DialogueRunner가 담당한다.
    /// </summary>
    public class DialogueSource : MonoBehaviour, IInteractable
    {
        [Tooltip("말하는 이 이름 (비워두면 표시 안 함)")]
        [SerializeField] private string speakerName = "";

        [Tooltip("이 NPC/오브젝트의 대화 노드 그래프. isEntryPoint가 켜져있는 노드 중 조건을 만족하는 첫 노드부터 시작한다.")]
        [SerializeField] private DialogueNode[] nodes;

        public void OnInteract(GameObject player)
        {
            if (DialogueRunner.Instance != null) DialogueRunner.Instance.StartDialogue(speakerName, nodes);
        }
    }
}

