namespace Customer
{
    public interface ICustomerState
    {
        public void OnEnter(Customer customer);
        public void OnExit();
    }
}