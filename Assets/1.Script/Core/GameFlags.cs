using System.Collections.Generic;
using UnityEngine;

namespace OMMG.Core
{
    /// <summary>
    /// 세션 동안 유지되는 범용 스토리 플래그 저장소. 문자열 키 하나당 true/false 값 하나.
    /// 대화 선택, 아이템 획득, 이벤트 등 어떤 시스템이든 이 플래그를 읽고 써서
    /// "한 번 일어난 일이 이후 결과에 영향을 준다"를 표현할 수 있다.
    /// </summary>
    public class GameFlags : MonoBehaviour
    {
        public static GameFlags Instance { get; private set; }

        private readonly Dictionary<string, bool> flags = new Dictionary<string, bool>();

        private void Awake()
        {
            Instance = this;
        }

        public bool Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            return flags.TryGetValue(key, out var value) && value;
        }

        public void Set(string key, bool value = true)
        {
            if (string.IsNullOrEmpty(key)) return;
            flags[key] = value;
        }
    }
}
