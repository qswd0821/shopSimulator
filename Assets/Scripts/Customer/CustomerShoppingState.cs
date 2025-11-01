using System;
using System.Collections;
using System.Collections.Generic;
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

            _coroutine = _customer.StartCoroutine(Main());
        }

        public void OnExit()
        {
            if (_coroutine != null)
            {
                _customer.StopCoroutine(_coroutine);
            }
        }

        private IEnumerator Main()
        {
            while (_customer.Shelves.Count > 0)
            {
                var shelf = _customer.Shelves.Dequeue();

                // 선반까지 이동할 때까지 대기
                yield return MoveToShelf(shelf.transform.position);

                // 상품 픽업 애니메이션이 끝날 때까지 대기
                yield return PickUpProduct(shelf);

                if (IsShoppingComplete()) break;
            }

            if (IsShoppingComplete())
            {
                _stateCallback.Invoke(new CustomerCheckingState());
            }
            else
            {
                // 물건을 다 구매하지 못한 경우 도둑질 (안 잡으면 손해)
                _stateCallback.Invoke(new CustomerLeavingState());
            }
        }

        private bool IsShoppingComplete()
            => _customer.Wishlist.Count == 0;


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
            var pickedProducts = new List<Product>();
            for (int i = 0; i < _customer.Wishlist.Count; i++)
            {
                // if (shelf.HasProduct(_customer.Wishlist[i]))
                // {
                //     pickedProducts.Add(shelf.Get());
                //     break;
                // }
            }

            if (pickedProducts.Count > 0)
            {
                foreach (var product in pickedProducts)
                {
                    _customer.Wishlist.Remove(product);
                    _customer.Inventory.Add(product);
                }

                _customer.Animator.SetTrigger(CustomerAnimator.PickUpTriggerParam);
                yield return _customer.Animator.WaitForAnimation(CustomerAnimator.PickUpStateName);
            }
            // DEBUG
#if UNITY_EDITOR
            else
            {
                Debug.Log($"No product in shelf({shelf.name}), just execute pickup animation");
                _customer.Animator.SetTrigger(CustomerAnimator.PickUpTriggerParam);
                yield return _customer.Animator.WaitForAnimation(CustomerAnimator.PickUpStateName);
            }
#endif

            yield return null;
        }
    }
}