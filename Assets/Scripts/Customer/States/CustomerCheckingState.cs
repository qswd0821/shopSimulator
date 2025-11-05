using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Customer.States
{
    /// <summary>
    /// 물건을 1개 이상 고른 경우 계산을 처리하는 State
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

            _customer = null;
            _stateCallback = null;
        }

        private IEnumerator Main()
        {
            yield return MoveToCounter();

            yield return Pay();

            // 퇴장
            _stateCallback.Invoke(new CustomerLeavingState());
        }

        private IEnumerator MoveToCounter()
        {
            bool moveCompleted = false;
            _customer.Movement.MoveTo(CustomerManager.Instance.counter.transform.position, b =>
            {
                if (!b)
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to counter");
                    _stateCallback?.Invoke(new CustomerLeavingState());
                    return;
                }

                moveCompleted = true;
            });
            yield return new WaitUntil(() => moveCompleted);
        }

        private IEnumerator Pay()
        {
            // 물건 나열 및 가격 계산
            int totalPrice = 0;
            foreach (var product in _customer.Inventory)
            {
                product.transform.position = CustomerManager.Instance.counter.transform.position + Vector3.up;
                product.gameObject.SetActive(true);
                totalPrice = product.GetPrice();
            }

            // 계산 대기
            SetCustomerWaitingUI(true);
            _customer.Animator.SetBool(CustomerAnimator.IsWaitingBoolParam, true);

            // 계산 상호작용 설정
            bool paymentCompleted = false;
            _customer.Interactor.SetPaymentInteraction(totalPrice, (b) =>
            {
                paymentCompleted = b;
                // if (!b) { } // TODO: some animations 
            });

            float timer = 0;
            yield return new WaitUntil(() =>
            {
                timer += Time.deltaTime;
                return timer > _customer.paymentPatientTime || paymentCompleted;
            });

            // 결제를 성공한 경우 (시간 초과 X) 
            if (paymentCompleted)
            {
                // 돈 지급(손님이? 게임매니저가? 직접?)
            }

            // 정리
            foreach (var product in _customer.Inventory)
            {
                Object.Destroy(product.gameObject);
            }

            _customer.Inventory.Clear();
            _customer.Animator.SetBool(CustomerAnimator.IsWaitingBoolParam, false);
            SetCustomerWaitingUI(false);
        }

        private void SetCustomerWaitingUI(bool b)
        {
            _customer.customerCanvas.SetActive(b);
        }
    }
}