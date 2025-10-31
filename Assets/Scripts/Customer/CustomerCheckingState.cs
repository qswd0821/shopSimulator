using System;
using System.Collections;
using UnityEngine;

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

        private IEnumerator Main()
        {
            float waitStartTime = Time.time;
            yield return MoveToLine();

            // 체크아웃 줄 대기
            yield return WaitLine();

            yield return Pay();

            // 퇴장
            _stateCallback.Invoke(new CustomerLeavingState());
        }

        private IEnumerator MoveToLine()
        {
            // 줄 서는 로직 구현 필요 (임시로 CHECKOUT까지 이동)
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

            return null;
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