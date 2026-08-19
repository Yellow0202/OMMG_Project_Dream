using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OMMG.Interaction
{
    /// <summary>
    /// "OOO을 획득했습니다!" 같은 짧은 알림을 잠깐 보여주고 자동으로 사라지는 팝업 싱글턴.
    /// </summary>
    public class ItemGetPopup : MonoBehaviour
    {
        public static ItemGetPopup Instance { get; private set; }

        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text messageText;
        [SerializeField, Min(0f)] private float displayDuration = 1.5f;

        private Coroutine hideRoutine;

        private void Awake()
        {
            Instance = this;
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Show(string message)
        {
            if (messageText != null) messageText.text = message;
            if (panelRoot != null) panelRoot.SetActive(true);

            if (hideRoutine != null) StopCoroutine(hideRoutine);
            hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(displayDuration);
            if (panelRoot != null) panelRoot.SetActive(false);
            hideRoutine = null;
        }
    }
}
