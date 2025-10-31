using System;
using UnityEngine;

namespace Customer
{
    /// <summary>
    /// 모든 행동을 마쳤거나 오류가 난 경우 전이하는 최종 State
    /// 문으로 향한 다음 마지막으로 ExitPosition까지 이동한 후 비활성화
    /// </summary>
    public class CustomerLeavingState : ICustomerState
    {
        private Customer _customer;
        private Action<ICustomerState> _stateCallback;

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;

            _customer.Movement.MoveTo(_customer.entrancePosition, b =>
            {
                _customer.Movement.MoveTo(_customer.exitPosition, b =>
                {
                    // end of life
                    _stateCallback.Invoke(null);
                });
            });
        }

        public void OnExit()
        {
        }
    }
}