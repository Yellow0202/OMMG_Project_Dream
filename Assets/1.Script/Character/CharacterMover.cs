using System.Collections;
using UnityEngine;

namespace OMMG.Character
{
    /// <summary>
    /// 2D 캐릭터 이동을 담당하는 범용 컴포넌트.
    /// 플레이어, NPC 등 어떤 캐릭터에도 동일하게 붙일 수 있으며,
    /// 이동 방향은 SetMoveInput()을 통해서만 전달받는다.
    /// (누가/무엇이 이동을 발동시키는지는 이 스크립트의 관심사가 아니다 - 그건 별도의 드라이버가 담당한다.)
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0f)]
        private float moveSpeed = 5f;

        [Header("Idle Stop")]
        [Tooltip("입력이 0이 된 후 이동 코루틴을 종료하기까지의 유예 시간(초). " +
                 "0이면 입력이 끊기는 즉시 코루틴이 종료된다. " +
                 "추후 정지 애니메이션/감속 등이 필요해지면 이 값만 조정하면 된다.")]
        [SerializeField, Min(0f)]
        private float stopDelay = 0f;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private float lastInputTime;
        private Coroutine moveRoutine;

        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0f, value);
        }

        public float StopDelay
        {
            get => stopDelay;
            set => stopDelay = Mathf.Max(0f, value);
        }

        /// <summary>현재 이동 코루틴이 동작 중인지 여부.</summary>
        public bool IsMoving => moveRoutine != null;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// 이동 방향을 전달한다. 플레이어 입력, AI, 컷씬 스크립트 등
        /// 어떤 드라이버든 이 메서드 하나만 호출하면 이동이 처리된다.
        /// 대각선 입력은 내부에서 정규화되어 상하좌우와 동일한 속도로 이동한다.
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;

            if (input != Vector2.zero)
            {
                lastInputTime = Time.time;

                if (moveRoutine == null)
                {
                    moveRoutine = StartCoroutine(MoveRoutine());
                }
            }
        }

        private IEnumerator MoveRoutine()
        {
            var wait = new WaitForFixedUpdate();

            while (true)
            {
                if (moveInput != Vector2.zero)
                {
                    // 대각선 이동 시에도 상/하/좌/우와 같은 속도가 되도록 정규화한다.
                    Vector2 dir = moveInput.normalized;
                    rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
                }
                else if (Time.time - lastInputTime >= stopDelay)
                {
                    break;
                }

                yield return wait;
            }

            moveRoutine = null;
            OnMovementStopped();
        }

        /// <summary>
        /// 이동 코루틴이 완전히 멈추는 시점에 호출되는 확장 지점.
        /// 추후 정지 애니메이션, 감속 이징, 사운드 등을 추가할 때
        /// MoveRoutine의 구조를 건드리지 않고 이 메서드만 오버라이드하면 된다.
        /// </summary>
        protected virtual void OnMovementStopped()
        {
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            stopDelay = Mathf.Max(0f, stopDelay);
        }
#endif
    }
}

