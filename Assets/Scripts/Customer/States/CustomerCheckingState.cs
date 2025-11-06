using System;
using System.Collections;
using UnityEngine;

namespace Customer
{
    /// <summary>
    /// 계산대에 줄 선 다음 대기하는 State
    /// 대기한 시간에 따라 도주하거나 결제를 수행
    /// </summary>
    public class CustomerCheckingState : ICustomerState
    {
        private Customer _customer;
        private Action<ICustomerState> _stateCallback;
        private Coroutine _coroutine;

        private const float ItemAddingInterval = 0.5f;


        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;
            _customer.hasCompletedPayment = false;

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
            Shared.GameManager.Pos.AddWaitingForPaymentLine(_customer);
            // yield return MoveToCounter();

            yield return new WaitUntil(()=>_customer.HasCompletedWaitngMovement);

            yield return new WaitUntil(() => Shared.GameManager.Pos.GetMyWaitNumber(_customer) == 0);

            yield return Pay();

            _stateCallback.Invoke(new CustomerLeavingState());
        }

        private IEnumerator MoveToCounter()
        {
            bool moveCompleted = false;
            _customer.Movement.MoveTo(CustomerManager.Instance.counterPosition, b =>
            {
                if (!b)
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to counterPosition");
                    _stateCallback?.Invoke(new CustomerLeavingState());
                    return;
                }

                moveCompleted = true;
            });
            yield return new WaitUntil(() => moveCompleted);
        }

        private IEnumerator Pay()
        {
            SetCustomerWaitingUI(true);
            _customer.Animator.SetBool(CustomerAnimator.IsWaitingPaymentBoolParam, true);

            foreach (var product in _customer.Inventory)
            {
                Shared.GameManager.Pos.AddProductToCheckPoint(product);
                yield return new WaitForSeconds(ItemAddingInterval);
            }

            yield return new WaitUntil(() => _customer.hasCompletedPayment);

            _customer.Inventory.Clear();
            _customer.Animator.SetBool(CustomerAnimator.ConfuseTriggerParam, false);
            SetCustomerWaitingUI(false);
        }

        private void SetCustomerWaitingUI(bool b)
        {
            _customer.customerCanvas.SetActive(b);
        }
    }
}