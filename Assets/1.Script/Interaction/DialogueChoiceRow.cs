using UnityEngine;
using UnityEngine.UI;

namespace OMMG.Interaction
{
    /// <summary>
    /// 선택지 목록 한 줄. 클릭하면 자신의 인덱스로 DialogueRunner.SelectChoice를 호출한다.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DialogueChoiceRow : MonoBehaviour
    {
        [SerializeField] private Text label;

        private Button button;
        private int index;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void Bind(int displayedIndex, string text)
        {
            index = displayedIndex;
            if (label != null) label.text = (displayedIndex + 1) + ". " + text;
        }

        private void OnClick()
        {
            if (DialogueRunner.Instance != null) DialogueRunner.Instance.SelectChoice(index);
        }
    }
}
