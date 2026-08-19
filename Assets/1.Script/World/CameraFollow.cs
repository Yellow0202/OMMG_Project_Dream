using System.Collections.Generic;
using UnityEngine;

namespace OMMG.World
{
    /// <summary>
    /// 지정된 대상(플레이어)을 부드럽게 따라가는 카메라 컨트롤러.
    /// 평소에는 SmoothDamp로 부드럽게 따라가고, 구역 이동처럼 순간이동이 필요할 때는
    /// SnapToTarget()을 호출해 카메라를 즉시 대상 중앙으로 배치할 수 있다.
    /// 씬에 배치된 CameraZoneBounds를 참고해서, 플레이어가 있는 구역이 화면보다 작으면
    /// 그 구역 중앙에 고정하고, 화면보다 크면 따라가되 구역 경계 밖으로는 나가지 않는다.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow Instance { get; private set; }

        private static readonly List<CameraZoneBounds> boundsRegistry = new List<CameraZoneBounds>();

        public static void RegisterBounds(CameraZoneBounds bounds)
        {
            if (!boundsRegistry.Contains(bounds)) boundsRegistry.Add(bounds);
        }

        public static void UnregisterBounds(CameraZoneBounds bounds)
        {
            boundsRegistry.Remove(bounds);
        }

        [Tooltip("따라갈 대상 (보통 Player)")]
        [SerializeField] private Transform target;

        [Tooltip("대상 기준 카메라 오프셋 (카메라 위치 = 대상 위치 + 이 값)")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

        [Tooltip("따라가는 부드러움 정도(초). 작을수록 빠르게 붙는다.")]
        [SerializeField, Min(0.01f)] private float smoothTime = 0.15f;

        private Vector3 velocity = Vector3.zero;
        private Camera cam;

        private void Awake()
        {
            Instance = this;
            cam = GetComponent<Camera>();
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = GetClampedDesiredPosition();
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        /// <summary>대상 위치로 카메라를 보간 없이 즉시 이동시킨다. 구역 이동 등에 사용.</summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            transform.position = GetClampedDesiredPosition();
            velocity = Vector3.zero;
        }

        /// <summary>
        /// target + offset을 기준으로 원하는 카메라 위치를 구하고,
        /// 플레이어가 있는 구역(CameraZoneBounds)이 등록돼 있으면 그 범위 안으로 축별로 제한한다.
        /// </summary>
        private Vector3 GetClampedDesiredPosition()
        {
            Vector3 desired = target.position + offset;

            CameraZoneBounds bounds = FindActiveBounds(target.position);
            if (bounds != null && cam != null)
            {
                float halfHeight = cam.orthographicSize;
                float halfWidth = halfHeight * cam.aspect;

                desired.x = ClampAxis(desired.x, bounds.Min.x, bounds.Max.x, halfWidth);
                desired.y = ClampAxis(desired.y, bounds.Min.y, bounds.Max.y, halfHeight);
            }

            return desired;
        }

        /// <summary>
        /// 구역 크기가 화면(halfExtent*2)보다 작거나 같으면 구역 중앙에 고정하고,
        /// 크면 desired 값을 구역 경계 안으로 클램프한다.
        /// </summary>
        private static float ClampAxis(float desired, float min, float max, float halfExtent)
        {
            float size = max - min;
            if (size <= halfExtent * 2f)
            {
                return (min + max) * 0.5f;
            }

            return Mathf.Clamp(desired, min + halfExtent, max - halfExtent);
        }

        private static CameraZoneBounds FindActiveBounds(Vector3 worldPos)
        {
            for (int i = 0; i < boundsRegistry.Count; i++)
            {
                var b = boundsRegistry[i];
                if (b != null && b.Contains(worldPos)) return b;
            }
            return null;
        }
    }
}
