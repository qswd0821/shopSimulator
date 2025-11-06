namespace Customer
{
    public interface ICustomerState
    {
        public void OnEnter(Customer customer, System.Action<ICustomerState> callback);
        public void OnExit();
    }
}