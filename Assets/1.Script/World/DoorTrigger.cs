using System.Collections;
using UnityEngine;
using OMMG.Character;

namespace OMMG.World
{
    /// <summary>
    /// 문 오브젝트에 부착. 플레이어가 트리거 영역에 닿으면
    /// 화면 페이드 후 targetSpawnPoint 위치로 순간이동시키고 다시 페이드 인한다.
    /// Collider2D(Is Trigger = true)가 필요하다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DoorTrigger : MonoBehaviour
    {
        [Tooltip("이 문을 통과했을 때 플레이어가 도착할 위치")]
        [SerializeField] private Transform targetSpawnPoint;

        [Tooltip("트리거에 반응할 대상의 태그")]
        [SerializeField] private string playerTag = "Player";

        [Tooltip("페이드 아웃/인 각각의 시간(초)")]
        [SerializeField, Min(0f)] private float fadeDuration = 0.3f;

        private bool isTransitioning;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isTransitioning) return;
            if (!other.CompareTag(playerTag)) return;
            if (targetSpawnPoint == null)
            {
                Debug.LogWarning($"[DoorTrigger] {name}: targetSpawnPoint가 설정되지 않았습니다.", this);
                return;
            }

            StartCoroutine(TransitionRoutine(other.gameObject));
        }

private IEnumerator TransitionRoutine(GameObject player)
        {
            isTransitioning = true;

            var inputMover = player.GetComponent<PlayerInputMover>();
            var rb = player.GetComponent<Rigidbody2D>();

            if (inputMover != null) inputMover.enabled = false;

            if (ScreenFader.Instance != null)
                yield return ScreenFader.Instance.FadeOut(fadeDuration);

            if (rb != null)
                rb.position = targetSpawnPoint.position;
            else
                player.transform.position = targetSpawnPoint.position;

            if (CameraFollow.Instance != null)
                CameraFollow.Instance.SnapToTarget();

            if (ScreenFader.Instance != null)
                yield return ScreenFader.Instance.FadeIn(fadeDuration);

            if (inputMover != null) inputMover.enabled = true;

            isTransitioning = false;
        }
    }
}
