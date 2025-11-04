using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Customer
{
    public class CustomerAnimator : MonoBehaviour
    {
        public const string SpeedFloatParam = "Speed";
        public const string TurnFloatParam = "Turn";
        public const string IsMovingBoolParam = "IsMoving";
        public const string IsWaitingBoolParam = "IsWaiting";
        public const string PickUpTriggerParam = "PickUp";

        public const string IdleStateName = "Idle";
        public const string MoveStateName = "Move";
        public const string PickUpStateName = "PickUp";
        public const string WaitStateName = "Wait";

        private static readonly int SpeedParamHash = Animator.StringToHash(SpeedFloatParam);
        private static readonly int TurnParamHash = Animator.StringToHash(TurnFloatParam);
        private static readonly int IsMovingParamHash = Animator.StringToHash(IsMovingBoolParam);

        [Header("Smoothing")] [SerializeField, Range(0f, 1f)]
        private float speedSmoothTime = 0.15f;

        [SerializeField, Range(0f, 1f)] private float turnSmoothTime = 0.1f;

        private Animator _animator;
        private NavMeshAgent _navAgent;

        private float _currentSpeed;
        private float _speedVelocity; // for SmoothDamp
        private float _currentTurn;
        private float _turnVelocity;

        void Reset()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
            if (_navAgent == null)
                _navAgent = GetComponent<NavMeshAgent>();
        }

        void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
            if (_navAgent == null)
                _navAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            UpdateAnimatorParameters();
        }

        public void SetTrigger(string triggerName)
        {
            if (_animator)
                _animator.SetTrigger(triggerName);
        }

        public void SetBool(string boolName, bool value)
        {
            if (_animator)
                _animator.SetBool(boolName, value);
        }

        private void UpdateAnimatorParameters()
        {
            if (!_animator)
                return;

            Vector3 horizontalVelocity = Vector3.zero;
            if (_navAgent)
            {
                // NavMeshAgent.velocity에는 y성분이 있을 수 있으므로 평면 성분만 사용
                Vector3 vel = _navAgent.velocity;
                vel.y = 0f;
                horizontalVelocity = vel;
            }

            float targetSpeed = horizontalVelocity.magnitude;

            // Speed smoothing
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedVelocity, speedSmoothTime);

            // Turn: 현재 forward와 이동 방향의 각도 (signed)
            float targetTurn = 0f;
            if (horizontalVelocity.sqrMagnitude > 0.0001f)
            {
                Vector3 moveDir = horizontalVelocity.normalized;
                // 로컬 좌표에서의 각도(좌/우 구분)
                float signedAngle = Vector3.SignedAngle(transform.forward, moveDir, Vector3.up);
                // 정규화(예: -1 .. 1)하거나 그대로 각도로 전달. 여기서는 각도 기반으로 보간 후 /180으로 정규화.
                targetTurn = Mathf.Clamp(signedAngle / 180f, -1f, 1f);
            }

            _currentTurn = Mathf.SmoothDamp(_currentTurn, targetTurn, ref _turnVelocity, turnSmoothTime);

            // Apply to animator
            _animator.SetFloat(SpeedParamHash, _currentSpeed);
            _animator.SetFloat(TurnParamHash, _currentTurn);
            _animator.SetBool(IsMovingParamHash, targetSpeed > 0.05f);
        }

        /// <summary>
        /// 지정한 상태(stateName)가 재생되기 시작하고 한 사이클(또는 적어도 1.0 이상의 normalizedTime) 완료될 때까지 기다립니다.
        /// Animator 컴포넌트 접근은 이 클래스 내부에서만 수행됩니다.
        /// stateName은 Animator 컨트롤러에 등록된 상태(State) 이름과 정확히 일치해야 합니다.
        /// </summary>
        public IEnumerator WaitForAnimation(string stateName, int layer = 0)
        {
            if (!_animator)
                yield break;

            const float timeout = 5f;
            var timer = 0f;

            // 먼저 해당 상태로 전환될 때까지 대기
            yield return new WaitUntil(() =>
            {
                timer += Time.deltaTime;
                if (timer > timeout) return true;

                var info = _animator.GetCurrentAnimatorStateInfo(layer);
                return info.IsName(stateName);
            });

            timer = 0f;
            // 재생이 시작되면, normalizedTime이 1.0 이상이 될 때까지(한사이클 완료) 대기
            yield return new WaitUntil(() =>
            {
                timer += Time.deltaTime;
                if (timer > timeout) return true;

                var info = _animator.GetCurrentAnimatorStateInfo(layer);
                return info.IsName(stateName) && info.normalizedTime >= 1f;
            });
        }
    }
}