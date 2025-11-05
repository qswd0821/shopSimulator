namespace Customer
{
    public interface ICustomerInteraction
    {
        public void Attack();
        public bool ValidatePayment(int price);
    }
}