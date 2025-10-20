using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Customer
{
    // 매장 내 탐색 및 물건 픽업
    public class CustomerShoppingState : ICustomerState
    {
        private Customer _customer;
        private Coroutine _coroutine;
        private Action<ICustomerState> _stateCallback;

        public class ShoppingYieldInstruction : CustomYieldInstruction
        {
            private bool _isDone;
            public override bool keepWaiting => _isDone;
            public void Complete() => _isDone = true;
        }

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _stateCallback = callback;
            _coroutine = _customer.StartCoroutine(MainLoop());
        }

        private IEnumerator MainLoop()
        {
            Queue<GameObject> shelves = new();
            foreach (var shelf in _customer.shelves)
            {
                shelves.Enqueue(shelf);
            }

            bool isFinished = false;
            while (!isFinished && shelves.Count > 0)
            {
                GameObject shelf = shelves.Dequeue();
                yield return MoveToNextShelf(shelf.transform.position);
                yield return PickUpItem(shelf, null); // should wait
                isFinished = HasAllItem();
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

        private ShoppingYieldInstruction MoveToNextShelf(Vector3 shelfPosition)
        {
            ShoppingYieldInstruction yieldInstruction = new ShoppingYieldInstruction();
            _customer.Movement.MoveTo(shelfPosition, result =>
            {
                if (result) yieldInstruction.Complete();
                else
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to next shelf");
                    _stateCallback(new CustomerLeavingState());
                }
            });

            return yieldInstruction;
        }

        private ShoppingYieldInstruction PickUpItem(GameObject shelf, params GameObject[] items)
        {
            // TEMP
            ShoppingYieldInstruction yieldInstruction = new ShoppingYieldInstruction();
            _customer.Movement.MoveTo(shelf.transform.position, result =>
            {
                if (result) yieldInstruction.Complete();
                else
                {
                    Debug.LogWarning($"{_customer.name}: Failed to move to next shelf");
                    _stateCallback(new CustomerLeavingState());
                }
            });

            return yieldInstruction;
        }

        private bool HasAllItem()
        {
            return true;
        }

        public void OnExit()
        {
            if (_coroutine != null)
            {
                _customer.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }
}