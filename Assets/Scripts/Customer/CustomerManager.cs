using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

        [Header("Debug")] [SerializeField] private bool showDebugUI = false;
        [SerializeField] private Customer currentCustomer = null;

        [Header("Customer")] [SerializeField] private GameObject customerPrefab;
        [SerializeField] private GameObject customerModelPrefab;

        [Header("Positions")] public Transform init;
        public Transform entrance;
        public Transform exit;

        public Vector3 CounterPosition => Shared.GameManager.Pos?.transform.position ?? Vector3.zero;
        public Vector3 ExitPosition => exit?.position ?? Vector3.zero;
        public Vector3 EntrancePosition => entrance?.position ?? Vector3.zero;
        public Vector3 InitPosition => init?.position ?? Vector3.zero;

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

        #region DEBUG

        private void CreateCustomer(ICustomerState state = null)
        {
            currentCustomer = Instantiate(customerPrefab).GetComponent<Customer>();
            var customerModel = Instantiate(customerModelPrefab);
            int modelTypeCount = 19;
            customerModel.transform.GetChild(Random.Range(0, modelTypeCount)).gameObject.SetActive(true);

            currentCustomer.Init(customerModel, state);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(InitPosition, Vector3.one * 1f);
            Gizmos.DrawCube(EntrancePosition, Vector3.one * 1f);
            Gizmos.DrawCube(ExitPosition, Vector3.one * 1f);

            if (!Application.isEditor)
            {
                Gizmos.DrawCube(CounterPosition, Vector3.one * 1f);
            }
        }

        private void OnGUI()
        {
            if (!showDebugUI) return;

            if (GUILayout.Button("Create Customer"))
            {
                CreateCustomer();
            }

            if (GUILayout.Button("Create TEST_IDle Customer"))
            {
                CreateCustomer(new TEST_CustomerIdleState());
            }

            if (currentCustomer)
            {
                if (GUILayout.Button("Attack Customer"))
                {
                    if (currentCustomer.TryGetComponent(out ICustomerInteraction interaction))
                    {
                        interaction.Attack();
                        Debug.Log("Attacked");
                    }

                    currentCustomer = null;
                }
            }
        }

        #endregion
    }
}