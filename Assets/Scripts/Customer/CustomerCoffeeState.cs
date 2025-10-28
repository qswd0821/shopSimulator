using System;
using System.Collections;
using UnityEngine;

namespace Customer
{
    public class CustomerCoffeeState : ICustomerState
    {
        private Customer _customer;
        private Action<ICustomerState> _stateCallback;
        private Coroutine _coroutine;

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;

            _coroutine = _customer.StartCoroutine(Main());
        }

        public IEnumerator Main()
        {
            // 1. Move to the coffee machine
            bool moveCompleted = false;
            _customer.Movement.MoveTo(_customer.coffeeMachinePosition, b =>
            {
                if (b) moveCompleted = true;
                else
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to coffee machine");
                    _stateCallback.Invoke(new CustomerLeavingState());
                }
            });
            yield return new WaitUntil(() => moveCompleted);

            // 1.5 Waiting in line

            // 2. Pay (wait for payment)
            yield return null;

            // 3. Wait for coffee
            yield return null;

            // 4. End
            _stateCallback.Invoke(new CustomerLeavingState());
        }

        public void OnExit()
        {
            if (_coroutine != null)
            {
                _customer.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }
}