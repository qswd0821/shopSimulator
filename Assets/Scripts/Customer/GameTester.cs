using System;
using UnityEngine;

namespace Customer
{
    public class GameTester : MonoBehaviour
    {
        [SerializeField] private Shelf[] shelves;
        [SerializeField] private GameObject productPrefab;
        [SerializeField] private GameObject customerPrefab;
        [SerializeField] private Vector3 testPosition;
        [SerializeField] private Transform checkout;

        private void Test()
        {
            foreach (var shelf in shelves)
            {
                for (int i = 0; i < 3; i++)
                {
                    var p = Instantiate(productPrefab).GetComponent<Product>();
                    shelf.AddProduct(p);
                }
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(testPosition, Vector3.one * 1f);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("CreateCustomer"))
            {
                var customer = Instantiate(customerPrefab);
                customer.GetComponent<Customer>().checkout = checkout.gameObject;
                customer.transform.position = testPosition;
            }

            if (GUILayout.Button("Test"))
            {
                Test();
            }
        }
    }
}