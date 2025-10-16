using UnityEngine;

namespace Customer
{
    public class CustomerCheckingState: ICustomerState
    {
        public void OnEnter(Customer customer)
        {
            // 줄 서기 + 대기(콜백)
            // 계산대와 Interact
            // ChangeState(CustomerLeavingState)
            throw new System.NotImplementedException();
        }
        
        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}