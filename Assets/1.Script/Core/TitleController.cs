using UnityEngine;
using UnityEngine.SceneManagement;

namespace OMMG.Core
{
    /// <summary>
    /// 타이틀 화면 로직. A 장소(SampleScene의 Zone A)를 그대로 흉내 낸 미니어처를 보여주되,
    /// GameFlags에 저장된 현재 상태(예: NPC를 도왔는지)에 맞춰 미니어처 구성 요소를
    /// 켜고 끈다. GameFlags는 DontDestroyOnLoad로 씬 전환에도 값이 유지되므로,
    /// 이 스크립트는 씬이 로드될 때마다(Start) 최신 상태를 다시 반영하기만 하면 된다.
    /// </summary>
    public class TitleController : MonoBehaviour
    {
        [Header("A 장소 상태에 따라 표시 여부가 바뀌는 미니어처 요소들")]
        [SerializeField] private GameObject npcMockup;

        [Header("이동할 인게임 씬 이름")]
        [SerializeField] private string gameplaySceneName = "SampleScene";

        private void Start()
        {
            Refresh();
        }

        /// <summary>GameFlags 현재 값에 맞춰 미니어처 요소들의 표시 여부를 갱신한다.</summary>
        public void Refresh()
        {
            bool helped = GameFlags.Instance != null && GameFlags.Instance.Get("npc_helped");
            if (npcMockup != null) npcMockup.SetActive(helped);
        }

        /// <summary>"게임 시작하기" 버튼에 연결되는 메서드.</summary>
        public void OnStartButtonClicked()
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}
