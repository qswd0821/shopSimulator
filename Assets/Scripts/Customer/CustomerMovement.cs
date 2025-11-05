using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Customer
{
    public class CustomerMovement : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Action<bool> _movementCallback;
        [SerializeField] private Vector3 testPosition;
        private float _moveSpeed;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _movementCallback = null;
        }

        public void MoveTo(Vector3 destination, Action<bool> onArrived = null)
        {
            _movementCallback = onArrived;
            bool succeed = _navMeshAgent.SetDestination(destination);

            // 목적지 설정 불가능
            if (!succeed || !_navMeshAgent.isOnNavMesh)
            {
                InvokeAndReset(false);
                return;
            }

            // 이미 충분히 가까우면 성공 처리
            if (!_navMeshAgent.pathPending && HasArrivedAtDestination())
            {
                InvokeAndReset(true);
            }
        }

        private void Update()
        {
            CheckCallback();
        }

        private void CheckCallback()
        {
            if (_movementCallback == null || _navMeshAgent.pathPending) return;

            if (HasArrivedAtDestination())
            {
                // 멈추었을 때만 콜백 호출
                bool success = _navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude <= 0.001f)
                {
                    InvokeAndReset(success);
                }
            }
            else if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                InvokeAndReset(false);
            }
        }

        private bool HasArrivedAtDestination()
            => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;


        private void InvokeAndReset(bool result)
        {
            _navMeshAgent.ResetPath();

            var callback = _movementCallback;
            _movementCallback = null;
            callback?.Invoke(result);
        }
    }
}