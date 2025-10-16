using UnityEngine;

namespace Customer
{
    public class CustomerEnteringState : ICustomerState
    {
        private Customer _customer;
        private Vector3 _entrancePosition;

        public void OnEnter(Customer customer)
        {
            _customer = customer;

            // 손님 정보 세팅 (모델, 목표 구매 리스트 등)

            customer.Movement.MoveTo(_entrancePosition, OnArrived);
        }

        private void OnArrived(bool succeed)
        {
            if (succeed) _customer.StateMachine.ChangeState(new CustomerShoppingState());
            else
            {
                Debug.LogWarning($"{_customer.name}: Failed to move to entrance");
                _customer.gameObject.SetActive(false);
            }
        }

        public void OnExit()
        {
        }
    }
}