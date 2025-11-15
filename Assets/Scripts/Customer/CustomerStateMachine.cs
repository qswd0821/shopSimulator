using System;
using UnityEngine;

namespace Customer
{
    public class CustomerStateMachine : MonoBehaviour
    {
        private ICustomerState CurrentState { get; set; } = null;

        private Customer _customer;
        public string _currentState;

        private void Awake()
        {
            _customer = GetComponent<Customer>();
            enabled = false;
        }

        private void Update()
        {
            _currentState = CurrentState?.GetType().Name;
        }

        private void OnDisable()
        {
            CurrentState?.OnExit();
            CurrentState = null;
        }

        // 한 번만 호출 가능
        public void StartState(ICustomerState state)
        {
            if (CurrentState != null) return;
            ChangeState(state);
        }

        private void ChangeState(ICustomerState next)
        {
            if (next == null || CurrentState?.GetType() == next?.GetType())
            {
                Debug.Log($"{gameObject.name}: End of state");
                _customer.DestroyCustomer();
                return;
            }

            var prev = CurrentState;
            if (prev != null)
            {
                next.OnExit();
            }

            CurrentState = next;
            next.OnEnter(_customer, ChangeState);
        }
    }
}