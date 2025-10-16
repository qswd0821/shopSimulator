using UnityEngine;

namespace Customer
{
    public class CustomerLeavingState : ICustomerState
    {
        public void OnEnter(Customer customer)
        {
            // 줄 서기
            throw new System.NotImplementedException();
        }
        
        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}