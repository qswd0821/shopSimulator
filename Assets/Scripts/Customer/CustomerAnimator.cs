using UnityEngine;
using UnityEngine.AI;

namespace Customer
{
    public class CustomerAnimator : MonoBehaviour
    {
        [Header("Parameters")] [SerializeField] private string speedParam = "Speed";
        [SerializeField] private string isMovingParam = "IsMoving";
        [SerializeField] private string turnParam = "Turn";

        [Header("Smoothing")] [SerializeField, Range(0f, 1f)] private float speedSmoothTime = 0.15f;
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

        private void UpdateAnimatorParameters()
        {
            float deltaTime = Time.deltaTime;
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
            _animator.SetFloat(speedParam, _currentSpeed);
            _animator.SetFloat(turnParam, _currentTurn);
            _animator.SetBool(isMovingParam, targetSpeed > 0.05f);
        }
    }
}