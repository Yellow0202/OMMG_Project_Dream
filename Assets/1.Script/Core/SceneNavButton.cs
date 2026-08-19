using UnityEngine;
using UnityEngine.SceneManagement;

namespace OMMG.Core
{
    /// <summary>
    /// 지정된 씬으로 전환하는 버튼용 범용 컴포넌트. 테스트용 "타이틀 <-> 인게임" 전환 버튼처럼
    /// 별다른 로직 없이 단순 씬 전환만 필요한 곳에서 재사용한다.
    /// </summary>
    public class SceneNavButton : MonoBehaviour
    {
        [SerializeField] private string targetSceneName;

        public void Go()
        {
            if (string.IsNullOrEmpty(targetSceneName)) return;
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
