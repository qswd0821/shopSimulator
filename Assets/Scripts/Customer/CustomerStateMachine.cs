using Customer.States;
using UnityEngine;

namespace Customer
{
    public class CustomerStateMachine : MonoBehaviour
    {
        public ICustomerState CurrentState { get; private set; }

        private Customer _customer;
        private string _currentState;

        private void Awake()
        {
            _customer = GetComponent<Customer>();
        }

        private void Start()
        {
            // 손님 정보 초기 세팅 후 진입
            ChangeState(new CustomerEnteringState());
        }

        private void Update()
        {
            _currentState = CurrentState.GetType().Name;
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