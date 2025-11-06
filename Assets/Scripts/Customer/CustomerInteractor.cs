using System;
using UnityEngine;

namespace Customer
{
    public class CustomerInteractor : MonoBehaviour, ICustomerInteraction
    {
        private bool _isReadyForPayment;
        private Action<bool> _paymentCallback;
        private int _price;

        public void SetPaymentInteraction(int price, Action<bool> callback)
        {
            _isReadyForPayment = true;
            _price = price;
            _paymentCallback = callback;
        }

        public void Attack()
        {
            // Dead Animation
            GetComponent<Customer>().DestroyCustomer();
        }

        public bool ValidatePayment(int price)
        {
            var succeed = _isReadyForPayment && (price == _price);
            _paymentCallback?.Invoke(succeed);
            return succeed;
        }
    }
}