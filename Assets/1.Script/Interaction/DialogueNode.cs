using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 대화 노드 그래프의 노드 하나. 순서대로 나오는 대사 몇 줄과,
    /// 그 뒤에 이어질 선택지(또는 다음 노드로 자동 진행)를 담는다.
    /// </summary>
    [System.Serializable]
    public class DialogueNode
    {
        [Tooltip("이 노드를 가리키는 고유 ID (선택지의 nextNodeId, 진입 조건 등에서 참조)")]
        [SerializeField] private string nodeId;

        [Tooltip("대화를 처음 시작할 때 진입 가능한 노드인지 여부")]
        [SerializeField] private bool isEntryPoint;

        [Tooltip("진입 조건 플래그(비워두면 조건 없음). requiredValue와 일치해야 이 노드로 진입한다.")]
        [SerializeField] private string requiredFlag;
        [SerializeField] private bool requiredValue = true;

        [Tooltip("순서대로 출력될 대사 목록")]
        [SerializeField, TextArea(2, 4)] private string[] lines = { "..." };

        [Tooltip("대사가 끝난 뒤 보여줄 선택지 목록. 비어있으면 선택지 없이 바로 nextNodeId로 진행(또는 대화 종료)")]
        [SerializeField] private DialogueChoice[] choices;

        [Tooltip("선택지가 없을 때, 대사가 끝난 뒤 자동으로 이어질 노드 ID. 비워두면 대화가 종료된다.")]
        [SerializeField] private string nextNodeId;

        public string NodeId => nodeId;
        public bool IsEntryPoint => isEntryPoint;
        public string RequiredFlag => requiredFlag;
        public bool RequiredValue => requiredValue;
        public string[] Lines => lines;
        public DialogueChoice[] Choices => choices;
        public string NextNodeId => nextNodeId;
    }

    /// <summary>
    /// 대화 노드 하나에 딸린 선택지 하나. 고를 때의 결과(플래그 저장/아이템 획득/다음 노드)를 정의한다.
    /// </summary>
    [System.Serializable]
    public class DialogueChoice
    {
        [Tooltip("선택지에 표시될 문구")]
        [SerializeField] private string text;

        [Tooltip("이 선택지를 고르면 true로 저장될 플래그(비워두면 저장 안 함)")]
        [SerializeField] private string flagKey;

        [Tooltip("hideAfterChosen이 켜져 있으면, flagKey가 이미 true일 때 이 선택지를 목록에서 숨긴다")]
        [SerializeField] private bool hideAfterChosen = true;

        [Tooltip("이 선택지가 보이기 위한 조건 플래그(비워두면 조건 없음)")]
        [SerializeField] private string requiredFlag;
        [SerializeField] private bool requiredValue = true;

        [Tooltip("선택 시 획득할 아이템(비워두면 아이템 없음)")]
        [SerializeField] private ItemData rewardItem;
        [SerializeField, Min(1)] private int rewardAmount = 1;

        [Tooltip("선택 후 이어질 노드 ID. 비워두면 대화가 종료된다.")]
        [SerializeField] private string nextNodeId;

        public string Text => text;
        public string FlagKey => flagKey;
        public bool HideAfterChosen => hideAfterChosen;
        public string RequiredFlag => requiredFlag;
        public bool RequiredValue => requiredValue;
        public ItemData RewardItem => rewardItem;
        public int RewardAmount => rewardAmount;
        public string NextNodeId => nextNodeId;
    }
}
