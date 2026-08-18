using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OMMG.World
{
    /// <summary>
    /// 화면 전체를 덮는 검은 이미지를 페이드 인/아웃 시키는 싱글턴.
    /// 씬에 Canvas + 이 컴포넌트가 붙은 풀스크린 Image가 하나 있어야 한다.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

        private Image image;

        private void Awake()
        {
            Instance = this;
            image = GetComponent<Image>();
            SetAlpha(0f);
        }

        public IEnumerator FadeOut(float duration)
        {
            yield return Fade(0f, 1f, duration);
        }

        public IEnumerator FadeIn(float duration)
        {
            yield return Fade(1f, 0f, duration);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                SetAlpha(to);
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float a = Mathf.Lerp(from, to, elapsed / duration);
                SetAlpha(a);
                yield return null;
            }
            SetAlpha(to);
        }

        private void SetAlpha(float a)
        {
            var c = image.color;
            c.a = a;
            image.color = c;
            image.raycastTarget = a > 0.01f;
        }
    }
}
