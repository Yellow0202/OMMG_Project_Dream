using UnityEngine;

namespace OMMG.World
{
    /// <summary>
    /// 카메라가 허용되는 이동 범위(구역의 경계)를 나타내는 마커.
    /// 씬에 구역마다 하나씩 배치해두면 CameraFollow가 플레이어 위치로
    /// 현재 어느 구역에 있는지 찾아서 그 범위를 기준으로 카메라를 제한한다.
    /// </summary>
    public class CameraZoneBounds : MonoBehaviour
    {
        [Tooltip("구역의 좌하단 경계 좌표")]
        [SerializeField] private Vector2 min = new Vector2(-5f, -3f);

        [Tooltip("구역의 우상단 경계 좌표")]
        [SerializeField] private Vector2 max = new Vector2(5f, 3f);

        public Vector2 Min => min;
        public Vector2 Max => max;

        public bool Contains(Vector2 point)
        {
            return point.x >= min.x && point.x <= max.x &&
                   point.y >= min.y && point.y <= max.y;
        }

        private void OnEnable()
        {
            CameraFollow.RegisterBounds(this);
        }

        private void OnDisable()
        {
            CameraFollow.UnregisterBounds(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 center = new Vector3((min.x + max.x) * 0.5f, (min.y + max.y) * 0.5f, 0f);
            Vector3 size = new Vector3(max.x - min.x, max.y - min.y, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
