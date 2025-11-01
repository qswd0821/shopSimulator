using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Customer
{
    /// <summary>
    /// Shopping에서 전이
    /// 물건을 다 고른 후 계산대 줄에 선 다음 계산까지 처리하는 State
    /// </summary>
    public class CustomerCheckingState : ICustomerState
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

        public void OnExit()
        {
            if (_coroutine != null)
            {
                _customer.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator Main()
        {
            float waitStartTime = Time.time;
            yield return MoveToLine();

            // 줄 서는 로직 구현 필요
            yield return WaitLine();

            yield return Pay();

            // 퇴장
            _stateCallback.Invoke(new CustomerLeavingState());
        }

        private IEnumerator MoveToLine()
        {
            bool moveCompleted = false;
            _customer.Movement.MoveTo(_customer.checkout.transform.position, (succeed) =>
            {
                moveCompleted = true;
                if (!succeed)
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to checkout");
                    _stateCallback?.Invoke(new CustomerLeavingState());
                }
            });
            yield return new WaitUntil(() => moveCompleted);
        }

        private IEnumerator WaitLine()
        {
            yield return null;
        }


        private IEnumerator Pay()
        {
            // 물건 나열 -> 계산 대기 및 지불 -> 물건 SetActive(false) 후 종료
            foreach (var product in _customer.Inventory)
            {
                product.gameObject.SetActive(true);
                // product.transform.position = dropPosition
            }

            // 계산 대기
            // ShowCustomerPaymentDialog()
            // yield return new WaitUntil(() => isPaid);

            // 돈 지급
            foreach (var product in _customer.Inventory)
            {
                Object.Destroy(product.gameObject);
            }

            _customer.Inventory.Clear();
            yield return null;
        }
    }
}