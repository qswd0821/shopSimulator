using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                _coroutine = null;
            }

            _stateCallback = null;
        }

        private IEnumerator Main()
        {
            // 매장에 구매 가능한 상품이 없음
            if (_customer.Wishlist.Count == 0)
            {
                _stateCallback.Invoke(new CustomerLeavingState());
                yield break;
            }


            // 매장 내 모든 선반으로 이동 (랜덤)
            foreach (var shelf in _customer.Shelves)
            {
                yield return MoveToShelf(shelf.transform.position);
                yield return SearchShelfForWishlistItems(shelf);
                if (IsShoppingComplete()) break;
            }

            // 물건을 하나라도 집은 경우
            if (_customer.Inventory.Count > 0)
            {
                _stateCallback.Invoke(new CustomerCheckingState());
            }
            else
            {
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

        private IEnumerator SearchShelfForWishlistItems(Shelf shelf)
        {
            var foundProducts = new List<Product>();
            foreach (var productData in _customer.Wishlist)
            {
            }

            int index = 0;
            while (index < _customer.Wishlist.Count)
            {
                var productData = _customer.Wishlist[index];
                var p = shelf.GetProduct(productData.GetId());
                if (p)
                {
                    foundProducts.Add(p);
                    AddProductToInventory(p);
                    _customer.Wishlist.RemoveAt(index);
                }
                else index++;
            }

            // Wishlist의 아이템이 선반에 있는 경우

            if (foundProducts.Count > 0)
            {
                _customer.Animator.SetTrigger(CustomerAnimator.PickUpTriggerParam);
                yield return _customer.Animator.WaitForAnimation(CustomerAnimator.PickUpStateName);
            }
            // 아예 없는 경우
            else
            {
                _customer.Animator.SetTrigger(CustomerAnimator.ConfuseTriggerParam);
                yield return _customer.Animator.WaitForAnimation(CustomerAnimator.ConfuseStateName);
            }

            yield return null;
        }

        private void AddProductToInventory(Product product)
        {
            product.SetBodyActive(false);
            product.transform.SetParent(_customer.transform, false);
            product.transform.localPosition = Vector3.zero;
            _customer.Inventory.Add(product);
        }
    }
}