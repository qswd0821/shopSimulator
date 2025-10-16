using System;
using UnityEngine;

namespace Customer
{
    public class CustomerStateMachine : MonoBehaviour
    {
        public ICustomerState CurrentState { get; private set; }
        private ICustomerState _pendingNext; // 같은 프레임 중복 전환 방지

        private Customer _owner;

        private void Awake()
        {
            _owner = GetComponent<Customer>();
        }

        private void Start()
        {
            // Customer 정보 초기 세팅
            SafeEnter(new CustomerEnteringState());
        }

        private void SetInitialState(ICustomerState initialState)
        {
            if (CurrentState != null)
            {
                Debug.LogWarning("Initial state already set. Ignoring.");
                return;
            }

            CurrentState = initialState ?? throw new ArgumentNullException(nameof(initialState));
            SafeEnter(CurrentState);
        }

        // 중첩 전환은 한 프레임에 1회로 제한.
        public void ChangeState(ICustomerState next)
        {
            if (next == null)
            {
                Debug.LogWarning("ChangeState called with null. Ignored.");
                return;
            }

            if (ReferenceEquals(CurrentState, next))
            {
                // 재진입 방지
                return;
            }

            if (_pendingNext != null)
            {
                // 이미 이번 프레임에 전환 진행 중이면 마지막 요청만 반영
                _pendingNext = next;
                return;
            }

            _pendingNext = next;
            DoChange();
            // 추가적인 ChangeState 호출이 Tick 중에 또 들어왔을 수 있음 → 한 번 더 처리
            if (_pendingNext != null && !ReferenceEquals(_pendingNext, CurrentState))
            {
                DoChange();
            }
        }

        private void DoChange()
        {
            var next = _pendingNext;
            _pendingNext = null;

            var prev = CurrentState;
            if (prev != null)
            {
                SafeExit(prev);
            }

            CurrentState = next;
            SafeEnter(CurrentState);
        }

        private void SafeEnter(ICustomerState state)
        {
            try
            {
                state.OnEnter(_owner);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void SafeExit(ICustomerState state)
        {
            try
            {
                state.OnExit();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}