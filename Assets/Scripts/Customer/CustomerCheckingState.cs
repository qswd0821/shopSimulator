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
       

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;
            _customer.IsPayMent = false;

            _coroutine = _customer.StartCoroutine(Main());

            Shared.GameManager.Pos.AddWaitingForPayMentLine(_customer);
        }

        private IEnumerator Main()
        {
            float waitStartTime = Time.time;

            while (true)
            {
                if(_customer.Movement.HasArrivedAtDestination())
                {
                    int idx = Shared.GameManager.Pos.GetMyWaitNumber(_customer);
                    if (idx == 0)
                    {
                        yield return Pay();
                        break;
                    }
                }
                yield return null;
            }
            _stateCallback.Invoke(new CustomerLeavingState());
            //yield return MoveToLine();

            // 체크아웃 줄 대기
            //while (true)
            //{
            //    if (Time.time - waitStartTime > _customer.patientTime)
            //    {
            //        // 대기 시간이 긴 경우 도주
            //        _stateCallback.Invoke(new CustomerLeavingState());
            //        break;
            //    }
            //
            //    yield return null;
            //}

            //yield return Pay();
            //_stateCallback.Invoke(new CustomerLeavingState());
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

        private IEnumerator Pay()
        {
            // 물건 나열 -> 계산 대기 및 지불 -> 물건 SetActive(false) 후 종료 
            Debug.Log("Pay");
            while(_customer.inventory.Count != 0)
            {
                //물건 나열
                Product product = _customer.inventory[0];
                Shared.GameManager.Pos.AddProductToCheckPoint(product);
                _customer.inventory.RemoveAt(0);

                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitUntil(() => _customer.IsPayMent);
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