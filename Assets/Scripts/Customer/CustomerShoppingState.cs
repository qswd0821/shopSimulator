using System;
using System.Collections;
using UnityEngine;

namespace Customer
{
    /// <summary>
    /// 매장 내에서 위시리스트에 맞춰서 쇼핑을 진행하는 상태.
    /// 물건이 다 채워질 때까지 선반을 둘러보며 다 찾은 경우 상태 전이.
    /// </summary>
    public class CustomerShoppingState : ICustomerState
    {
        private Customer _customer;
        private Coroutine _coroutine;
        private Action<ICustomerState> _stateCallback;

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;

            _coroutine = _customer.StartCoroutine(MainLoop());
        }

        public void OnExit()
        {
            if (_coroutine != null)
            {
                _customer.StopCoroutine(_coroutine);
            }
        }

        private IEnumerator MainLoop()
        {
            while (_customer.Shelves.Count > 0)
            {
                var shelf = _customer.Shelves.Dequeue();

                // 선반까지 이동이 완료될 때까지 대기
                yield return MoveToShelf(shelf.transform.position);

                // 상품 픽업 동작 완료할 때까지 대기
                yield return PickUpProduct(shelf);

                if (IsShoppingComplete()) break;
            }

            if (IsShoppingComplete())
            {
                _stateCallback.Invoke(new CustomerCheckingState());
            }
            else
            {
                // 원하는 아이템이 매장에 없는 경우가 있다면
                // 1. 물건을 다시 원상복구 한 뒤 퇴장
                // 2. 도둑질
                // 3. 가지고 있는 것만 결제(현재)
                _stateCallback.Invoke(new CustomerCheckingState());
            }
        }

        private bool IsShoppingComplete()
            => _customer.wishList.Count == 0;


        private IEnumerator MoveToShelf(Vector3 position)
        {
            bool moveComplete = false;
            _customer.Movement.MoveTo(position, result =>
            {
                moveComplete = true;

                if (!result)
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to shelf");
                    _stateCallback?.Invoke(new CustomerLeavingState());
                }
            });

            return new WaitUntil(() => moveComplete);
        }

        private IEnumerator PickUpProduct(Shelf shelf)
        {
            foreach (var productData in _customer.wishList)
            {
                if (!shelf.HasProduct("id")) continue;
                if (shelf.GetProduct(out var productObject))
                {
                    _customer.inventory.Add(productObject);
                }
            }

            // TODO: 애니메이션 Trigger(pickup) 기다리기
            return null;
        }
    }
}