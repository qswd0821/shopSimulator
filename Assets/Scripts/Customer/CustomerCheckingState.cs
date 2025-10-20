using System;
using System.Collections;
using UnityEngine;

namespace Customer
{
    public class CustomerCheckingState : ICustomerState
    {
        private Customer _customer;
        private Action<ICustomerState> _stateCallback;
        private Coroutine _coroutine;

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            // 줄 서기 + 대기(콜백)
            // 계산대와 Interact
            // ChangeState(CustomerLeavingState)
            _customer = customer;
            _stateCallback = callback;

            _coroutine = _customer.StartCoroutine(Main());
        }

        private IEnumerator Main()
        {
            yield return null;
            // Wait checkout line..
            // if WaitTime < 2 min Interact with calculator
            // else ChangeState(CustomerLeavingState) < RUN

            // Pay

            _stateCallback.Invoke(new CustomerLeavingState());
        }

        public void OnExit()
        {
            if (_coroutine != null)
            {
                _customer.StopCoroutine(_coroutine);
            }
        }
    }
}