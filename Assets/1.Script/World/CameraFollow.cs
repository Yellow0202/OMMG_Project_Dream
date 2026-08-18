using UnityEngine;

namespace OMMG.World
{
    /// <summary>
    /// 지정된 대상(플레이어)을 부드럽게 따라가는 카메라 컨트롤러.
    /// 평소에는 SmoothDamp로 부드럽게 따라가고, 구역 이동처럼 순간이동이 필요할 때는
    /// SnapToTarget()을 호출해 카메라를 즉시 대상 중앙으로 배치할 수 있다.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow Instance { get; private set; }

        [Tooltip("따라갈 대상 (보통 Player)")]
        [SerializeField] private Transform target;

        [Tooltip("대상 기준 카메라 오프셋 (카메라 위치 = 대상 위치 + 이 값)")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

        [Tooltip("따라가는 부드러움 정도(초). 작을수록 빠르게 붙는다.")]
        [SerializeField, Min(0.01f)] private float smoothTime = 0.15f;

        private Vector3 velocity = Vector3.zero;

        private void Awake()
        {
            Instance = this;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        /// <summary>대상 위치로 카메라를 보간 없이 즉시 이동시킨다. 구역 이동 등에 사용.</summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            transform.position = target.position + offset;
            velocity = Vector3.zero;
        }
    }
}
