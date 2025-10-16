using UnityEngine;

namespace Customer
{
    enum CustomerState
    {
        Waiting,
        Shopping,
        Checking,
        Leaving,
    }

    public class Customer : MonoBehaviour
    {
        public CustomerStateMachine StateMachine { get; private set; }
        public CustomerMovement Movement { get; private set; }

        private void Awake()
        {
            StateMachine = GetComponent<CustomerStateMachine>();
            Movement = GetComponent<CustomerMovement>();
        }
    }
}