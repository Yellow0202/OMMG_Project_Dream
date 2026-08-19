using System.Collections.Generic;
using UnityEngine;
using OMMG.Core;
using OMMG.Inventory;

namespace OMMG.Interaction
{
    /// <summary>
    /// 대화 노드 그래프를 순회하는 진행자 싱글턴.
    /// 실제 텍스트 표시는 DialogueBox, 선택지 표시는 DialogueChoiceUI에 위임하고,
    /// 이 클래스는 "지금 어느 노드에 있고, 다음에 뭐을 보여줄지"만 책임진다.
    /// </summary>
    public class DialogueRunner : MonoBehaviour
    {
        public static DialogueRunner Instance { get; private set; }

        private DialogueNode[] currentNodes;
        private string currentSpeaker;
        private DialogueNode activeNode;
        private bool showingChoices;

        /// <summary>대사 또는 선택지 중 하나라도 표시 중이면 true.</summary>
        public bool IsActive => (DialogueBox.Instance != null && DialogueBox.Instance.IsOpen) || showingChoices;

        /// <summary>지금 선택지를 고르는 중인지 여부. 입력 드라이버가 숫자키 라우팅에 사용.</summary>
        public bool IsShowingChoices => showingChoices;

        /// <summary>
        /// IsActive가 바뀌었을 수 있는 시점마다(대화 시작/진행/선택/강제종료) 호출된다.
        /// PlayerInteractor 등이 여기 구독해서 이동 잠금 상태를 다시 동기화한다.
        /// SelectChoice처럼 대화를 즉시 끝낼 수 있는 경로가 있어서, "E 키를 눌렀을 때만" 동기화하는 방식으로는
        /// 놓치는 경우가 생기기 때문에 상태가 바뀌는 모든 진입점에서 직접 알린다.
        /// </summary>
        public event System.Action OnStateChanged;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>대화를 시작한다. nodes 중 조건을 만족하는 첫 진입 노드부터 시작한다.</summary>
        public void StartDialogue(string speaker, DialogueNode[] nodes)
        {
            try
            {
                if (nodes == null || nodes.Length == 0) return;

                var entry = FindEntryNode(nodes);
                if (entry == null)
                {
                    Debug.LogWarning("DialogueRunner: 진입 가능한 노드를 찾지 못했습니다.");
                    return;
                }

                currentSpeaker = speaker;
                currentNodes = nodes;
                EnterNode(entry);
            }
            finally
            {
                OnStateChanged?.Invoke();
            }
        }

        /// <summary>상호작용 키(E) 입력 시 호출. 선택지 표시 중에는 아무 것도 하지 않는다.</summary>
        public void Advance()
        {
            try
            {
                if (showingChoices) return;
                if (DialogueBox.Instance == null || !DialogueBox.Instance.IsOpen) return;

                DialogueBox.Instance.AdvanceOrClose();
                if (!DialogueBox.Instance.IsOpen)
                {
                    ProceedFromNode();
                }
            }
            finally
            {
                OnStateChanged?.Invoke();
            }
        }

        /// <summary>선택지 중 하나를 고른다(숫자키 또는 마우스 클릭에서 호출).</summary>
        public void SelectChoice(int displayedIndex)
        {
            try
            {
                if (!showingChoices || activeNode == null) return;

                var eligible = GetEligibleChoices(activeNode);
                if (displayedIndex < 0 || displayedIndex >= eligible.Count) return;

                var choice = eligible[displayedIndex];
                showingChoices = false;
                if (DialogueChoiceUI.Instance != null) DialogueChoiceUI.Instance.Hide();

                if (!string.IsNullOrEmpty(choice.FlagKey) && GameFlags.Instance != null)
                {
                    GameFlags.Instance.Set(choice.FlagKey, true);
                }

                if (choice.RewardItem != null && PlayerInventory.Instance != null)
                {
                    PlayerInventory.Instance.AddItem(choice.RewardItem, choice.RewardAmount);
                    if (ItemGetPopup.Instance != null)
                    {
                        ItemGetPopup.Instance.Show(choice.RewardItem.DisplayName + "을(를) 획득했습니다!");
                    }
                }

                if (!string.IsNullOrEmpty(choice.NextNodeId))
                {
                    var next = FindNode(choice.NextNodeId);
                    if (next != null)
                    {
                        EnterNode(next);
                        return;
                    }
                }

                EndDialogue();
            }
            finally
            {
                OnStateChanged?.Invoke();
            }
        }

        /// <summary>범위 이탈 등으로 대화를 강제 종료할 때 호출.</summary>
        public void ForceClose()
        {
            try
            {
                showingChoices = false;
                if (DialogueChoiceUI.Instance != null) DialogueChoiceUI.Instance.Hide();
                if (DialogueBox.Instance != null) DialogueBox.Instance.Close();
                activeNode = null;
                currentNodes = null;
            }
            finally
            {
                OnStateChanged?.Invoke();
            }
        }

        private void EnterNode(DialogueNode node)
        {
            activeNode = node;
            showingChoices = false;

            if (node.Lines != null && node.Lines.Length > 0)
            {
                if (DialogueBox.Instance != null) DialogueBox.Instance.Show(currentSpeaker, node.Lines);
            }
            else
            {
                ProceedFromNode();
            }
        }

        private void ProceedFromNode()
        {
            if (activeNode == null) { EndDialogue(); return; }

            var eligible = GetEligibleChoices(activeNode);
            if (eligible.Count > 0)
            {
                showingChoices = true;
                if (DialogueChoiceUI.Instance != null) DialogueChoiceUI.Instance.Show(eligible);
                return;
            }

            if (!string.IsNullOrEmpty(activeNode.NextNodeId))
            {
                var next = FindNode(activeNode.NextNodeId);
                if (next != null)
                {
                    EnterNode(next);
                    return;
                }
            }

            EndDialogue();
        }

        private void EndDialogue()
        {
            ForceClose();
        }

        private List<DialogueChoice> GetEligibleChoices(DialogueNode node)
        {
            var result = new List<DialogueChoice>();
            if (node.Choices == null) return result;

            for (int i = 0; i < node.Choices.Length; i++)
            {
                var c = node.Choices[i];

                if (!string.IsNullOrEmpty(c.RequiredFlag))
                {
                    bool has = GameFlags.Instance != null && GameFlags.Instance.Get(c.RequiredFlag);
                    if (has != c.RequiredValue) continue;
                }

                if (c.HideAfterChosen && !string.IsNullOrEmpty(c.FlagKey))
                {
                    bool alreadyChosen = GameFlags.Instance != null && GameFlags.Instance.Get(c.FlagKey);
                    if (alreadyChosen) continue;
                }

                result.Add(c);
            }

            return result;
        }

        private DialogueNode FindEntryNode(DialogueNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                var n = nodes[i];
                if (!n.IsEntryPoint) continue;

                if (!string.IsNullOrEmpty(n.RequiredFlag))
                {
                    bool has = GameFlags.Instance != null && GameFlags.Instance.Get(n.RequiredFlag);
                    if (has != n.RequiredValue) continue;
                }

                return n;
            }
            return null;
        }

        private DialogueNode FindNode(string id)
        {
            if (currentNodes == null) return null;
            for (int i = 0; i < currentNodes.Length; i++)
            {
                if (currentNodes[i].NodeId == id) return currentNodes[i];
            }
            return null;
        }
    }
}

