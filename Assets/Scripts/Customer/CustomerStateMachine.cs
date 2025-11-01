using System;
using UnityEngine;

namespace Customer
{
    public class CustomerStateMachine : MonoBehaviour
    {
        public ICustomerState CurrentState { get; private set; }

        private Customer _owner;
        private string _currentState;

        private void Awake()
        {
            _owner = GetComponent<Customer>();
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
                Debug.Log($"{gameObject.name}: next State is null, Disable");
                Clear();
                return;
            }

            var prev = CurrentState;
            if (prev != null)
            {
                next.OnExit();
            }

            CurrentState = next;
            next.OnEnter(_owner, ChangeState);
        }

        private void Clear()
        {
            if (_owner.Inventory.Count != 0)
            {
                // 도둑질한 경우
                foreach (var product in _owner.Inventory)
                {
                    Destroy(product.gameObject);
                    return;
                }
            }

            _owner.gameObject.SetActive(false);
        }
    }
}