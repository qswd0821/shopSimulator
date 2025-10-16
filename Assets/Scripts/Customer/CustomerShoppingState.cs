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
        private bool _isBusy;

        public void OnEnter(Customer customer)
        {
            _customer = customer;
            _isBusy = true; // 해제 필요
            _coroutine = _customer.StartCoroutine(nameof(MainLoop));
        }

        public IEnumerator MainLoop()
        {
            Stack<Vector3> waypoints = new(); // 선반 무작위 순서
            bool isFinished = false;

            while (!isFinished)
            {
                if (_isBusy) yield return null;

                // MoveToShelf()
                // PickUp()
                // isFinished = HasAllItem()
                // if (isFinished) break;
            }

            _customer.StateMachine.ChangeState(new CustomerLeavingState());
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