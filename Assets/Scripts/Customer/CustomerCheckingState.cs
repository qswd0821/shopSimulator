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
            float waitStartTime = Time.time;

            // Move to Line (현재 임시 CHECKOUT)
            _customer.Movement.MoveTo(_customer.checkout.transform.position);

            // CHECKOUT 줄서기
            while (true)
            {
                // 너무 오래 기다리는 경우 도주
                if (Time.time - waitStartTime > 2)
                {
                    _stateCallback.Invoke(new CustomerLeavingState());
                    break;
                }

                yield return null;
            }

            // 이외 Pay
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