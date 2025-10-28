using System;

namespace Customer
{
    public class BadCustomerDancingState : ICustomerState
    {
        public void OnEnter(Customer customer, Action<ICustomerState> callback)
        {
        }

        public void OnExit()
        {
        }
    }
}