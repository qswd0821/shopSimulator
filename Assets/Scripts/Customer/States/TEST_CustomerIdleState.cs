using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Customer
{
    public class TEST_CustomerIdleState : ICustomerState
    {
        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
        }

        public void OnExit()
        {
        }
    }
}