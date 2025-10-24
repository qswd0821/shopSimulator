using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Customer
{
    // 매장 내 탐색 및 물건 픽업
    public class CustomerShoppingState : ICustomerState
    {
        private Customer _customer;
        private Coroutine _coroutine;
        private Action<ICustomerState> _stateCallback;

        private class ShoppingYieldInstruction : CustomYieldInstruction
        {
            private bool _isDone;
            public override bool keepWaiting => !_isDone;
            public void Complete() => _isDone = true;
        }

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
            bool isFinished = false;
            while (!isFinished && _customer.Shelves.Count > 0)
            {
                var shelf = _customer.Shelves.Dequeue();

                // 선반으로 이동할 때까지 대기
                yield return MoveToShelf(shelf.transform.position);

                // 아이템 픽업할 때까지 대기
                yield return PickUpProduct(shelf.gameObject, null);
                isFinished = IsShoppingComplete();
            }

            if (isFinished)
            {
                _stateCallback.Invoke(new CustomerCheckingState());
            }
            else
            {
                // 손님이 원하는 아이템이 매장에 없을 수 있는가? 이 때 행동은 어떡할 것인가
                // 1. 물건을 다시 원상복구 한 뒤 퇴장
                // 2. 도둑질 (현재)
                // 3. 가지고 있는 것만 결제
                _stateCallback.Invoke(new CustomerLeavingState());
            }
        }

        private bool IsShoppingComplete()
        {
            // 가지고 있는 아이템 비교
            return true;
        }

        private ShoppingYieldInstruction MoveToShelf(Vector3 position)
        {
            ShoppingYieldInstruction yieldInstruction = new ShoppingYieldInstruction();
            _customer.Movement.MoveTo(position, result =>
            {
                if (!result)
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to shelf");
                }

                yieldInstruction.Complete();
            });

            return yieldInstruction;
        }

        private ShoppingYieldInstruction PickUpProduct(GameObject shelf, params GameObject[] products)
        {
            // TEMP
            ShoppingYieldInstruction yieldInstruction = new ShoppingYieldInstruction();

            // Pickup animation clip's callback
            yieldInstruction.Complete();

            return yieldInstruction;
        }
    }
}