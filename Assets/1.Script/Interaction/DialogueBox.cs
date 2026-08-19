using UnityEngine;
using UnityEngine.UI;

namespace OMMG.Interaction
{
    /// <summary>
    /// 화면 하단에 대사를 한 줄씩 보여주는 대화창 싱글턴.
    /// 상호작용 키를 다시 누르면(PlayerInteractor.TryInteract 경유) 다음 줄로 넘어가거나 닫힌다.
    /// </summary>
    public class DialogueBox : MonoBehaviour
    {
        public static DialogueBox Instance { get; private set; }

        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text bodyText;

        private string currentSpeaker;
        private string[] currentLines;
        private int currentIndex;

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

        private void Awake()
        {
            Instance = this;
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Show(string speaker, string[] lines)
        {
            if (lines == null || lines.Length == 0) return;

            currentSpeaker = speaker;
            currentLines = lines;
            currentIndex = 0;

            if (panelRoot != null) panelRoot.SetActive(true);
            DisplayCurrentLine();
        }

        public void AdvanceOrClose()
        {
            if (!IsOpen) return;

            currentIndex++;
            if (currentLines == null || currentIndex >= currentLines.Length)
            {
                Close();
                return;
            }

            DisplayCurrentLine();
        }

        public void Close()
        {
            currentLines = null;
            currentIndex = 0;
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void DisplayCurrentLine()
        {
            if (bodyText == null || currentLines == null) return;

            string line = currentLines[currentIndex];
            bodyText.text = string.IsNullOrEmpty(currentSpeaker) ? line : (currentSpeaker + ": " + line);
        }
    }
}
