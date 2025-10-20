using System;
using UnityEngine;

namespace Customer
{
    public class CustomerLeavingState : ICustomerState
    {
        private Customer _customer;
        private Action<ICustomerState> _stateCallback;

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;
            // Go Out
            Act();
        }

        private void Act()
        {
            _customer.Movement.MoveTo(_customer.exitPosition, OnArrived);
            return;

            void OnArrived(bool obj)
            {
                _stateCallback.Invoke(new CustomerCheckingState());
            }
        }

        public void OnExit()
        {
        }
    }
}