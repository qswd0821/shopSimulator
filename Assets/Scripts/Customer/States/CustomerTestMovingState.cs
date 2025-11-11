using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Customer
{
    public class CustomerTestMovingState : ICustomerState
    {
        private Customer _customer;
        private Shelf[] _shelve;

        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
            _customer = customer;
            _shelve = Object.FindObjectsByType<Shelf>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            customer.Movement.MoveTo(_shelve[0].transform.position, OnArrived);
        }

        private void OnArrived(bool _)
        {
            _customer.Movement.MoveTo(_shelve[Random.Range(0, _shelve.Length)].transform.position, OnArrived);
        }

        public void OnExit()
        {
        }
    }
}