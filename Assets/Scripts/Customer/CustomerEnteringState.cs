using UnityEngine;

namespace Customer
{
    /// <summary>
    /// NPC 생성 후 출입문으로 진입하는 상태
    /// </summary>
    public class CustomerEnteringState : ICustomerState
    {
        private Customer _customer;
        private System.Action<ICustomerState> _stateCallback;

        public void OnEnter(Customer customer, System.Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;

            // 손님 정보 세팅 (모델, 목표 구매 리스트 등)
            customer.transform.position = _customer.startPosition;
            customer.Movement.MoveTo(_customer.entrancePosition, OnArrived);
        }

        private void OnArrived(bool succeed)
        {
            if (succeed)
            {
                if (_stateCallback == null) Debug.LogWarning($"{_customer.name}: State callback is null");
                else _stateCallback.Invoke(new CustomerShoppingState());
            }
            else
            {
                Debug.LogWarning($"{_customer.name}: Failed to move to entrance");
                _stateCallback.Invoke(new CustomerLeavingState());
            }
        }

        public void OnExit()
        {
        }
    }
}