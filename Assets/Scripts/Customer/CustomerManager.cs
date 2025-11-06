using System.Collections.Generic;
using UnityEngine;

namespace Customer
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindFirstObjectByType<CustomerManager>();
                if (_instance) return _instance;

                var go = new GameObject("CustomerManager");
                _instance = go.AddComponent<CustomerManager>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        private static CustomerManager _instance;

        public bool debugMode;
        [Space(10)] [SerializeField] private GameObject customerPrefab;
        [SerializeField] private GameObject[] customerModels;

        [Header("Environment")] [SerializeField]
        private Shelf[] shelves;
        [SerializeField] private GameObject productPrefab;
        [SerializeField] private Transform checkout;

        [Header("Positions")] public Vector3 counterPosition;
        public Vector3 exitPosition;
        public Vector3 entrancePosition;
        public Vector3 startPosition;

        public readonly Queue<Product> AvailableProducts = new(); // Random order


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            // Set available products
            AvailableProducts.Clear();

            var products = FindObjectsByType<Product>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (var i = products.Length - 1; i > 0; i--)
            {
                var randomIndex = UnityEngine.Random.Range(0, i + 1);
                (products[i], products[randomIndex]) = (products[randomIndex], products[i]);
            }

            foreach (var product in products)
            {
                AvailableProducts.Enqueue(product);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(startPosition, Vector3.one * 1f);
            Gizmos.DrawCube(entrancePosition, Vector3.one * 1f);
            Gizmos.DrawCube(exitPosition, Vector3.one * 1f);
            Gizmos.DrawCube(counterPosition, Vector3.one * 1f);
        }

        #region DEBUG

        private string _moneyTextField;

        private void OnGUI()
        {
            if (!debugMode) return;
            Cursor.lockState = CursorLockMode.None;

            if (GUILayout.Button("Init"))
            {
                Init();
            }

            if (GUILayout.Button("Create Product"))
            {
                CreateProductIntoShelf();
            }

            if (GUILayout.Button("Create Customer"))
            {
                CreateCustomer();
            }

            if (GUILayout.Button("Attack Customers"))
            {
                AttackCustomers();
            }

            _moneyTextField = GUILayout.TextArea(_moneyTextField);
            if (GUILayout.Button("Payment"))
            {
                if (int.TryParse(_moneyTextField, out int money))
                {
                    Payment(money);
                }
                else
                {
                    Debug.Log("Invalid Input");
                }
            }
        }

        private void CreateProductIntoShelf()
        {
            var p = Instantiate(productPrefab).GetComponent<Product>();
            shelves[0].AddProduct(p);
        }

        private void AttackCustomers()
        {
            var customers =
                FindObjectsByType<Customer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (customers.Length == 0)
            {
                Debug.Log("No Customer");
                return;
            }

            foreach (var item in customers)
            {
                if (item.TryGetComponent(out ICustomerInteraction interaction))
                {
                    interaction.Attack();
                }
            }
        }

        private void Payment(int money)
        {
            var customers =
                FindObjectsByType<Customer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (customers.Length == 0)
            {
                Debug.Log("No Customer");
                return;
            }

            foreach (var item in customers)
            {
                if (item.TryGetComponent(out ICustomerInteraction interaction))
                {
                    bool succeed = interaction.ValidatePayment(money);
                    Debug.Log($"{item.name}: {succeed}");
                }
            }
        }

        private void CreateCustomer()
        {
            var customer = Instantiate(customerPrefab);
            customer.transform.position = startPosition;
        }

        #endregion
    }
}