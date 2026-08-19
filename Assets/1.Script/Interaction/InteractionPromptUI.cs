using UnityEngine;

namespace OMMG.Interaction
{
    /// <summary>
    /// 상호작용 가능한 대상이 범위 안에 있을 때 안내 문구를 보여주는 싱글턴.
    /// </summary>
    public class InteractionPromptUI : MonoBehaviour
    {
        public static InteractionPromptUI Instance { get; private set; }

        [SerializeField] private GameObject panelRoot;

        private void Awake()
        {
            Instance = this;
            SetVisible(false);
        }

        public void SetVisible(bool visible)
        {
            if (panelRoot != null) panelRoot.SetActive(visible);
        }
    }
}
