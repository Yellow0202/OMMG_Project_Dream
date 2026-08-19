using UnityEngine;
using UnityEngine.InputSystem;

namespace OMMG.Interaction
{
    /// <summary>
    /// 플레이어 전용 대화 선택지 입력 드라이버. 숫자키 1~4로 선택지를 고른다.
    /// 마우스 클릭 선택은 DialogueChoiceRow가 Button.onClick으로 직접 처리한다.
    /// </summary>
    public class DialogueChoiceInput : MonoBehaviour
    {
        private void Update()
        {
            if (DialogueRunner.Instance == null || !DialogueRunner.Instance.IsShowingChoices) return;

            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.digit1Key.wasPressedThisFrame) DialogueRunner.Instance.SelectChoice(0);
            else if (keyboard.digit2Key.wasPressedThisFrame) DialogueRunner.Instance.SelectChoice(1);
            else if (keyboard.digit3Key.wasPressedThisFrame) DialogueRunner.Instance.SelectChoice(2);
            else if (keyboard.digit4Key.wasPressedThisFrame) DialogueRunner.Instance.SelectChoice(3);
        }
    }
}
