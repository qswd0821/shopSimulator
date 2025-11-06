using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Customer
{
    /// <summary>
    /// Customer의 가장 최상위 클래스
    /// </summary>
    [RequireComponent(typeof(CustomerMovement), typeof(CustomerAnimator), typeof(CustomerInteractor))]
    public class Customer : MonoBehaviour
    {
        public CustomerMovement Movement { get; private set; }
        public CustomerAnimator Animator { get; private set; }
        public CustomerInteractor Interactor { get; private set; }

        [Header("Customer")] public GameObject customerModel;
        public GameObject customerCanvas;
        public float paymentPatientTime = 15;
        public int wishlistInitSize = 3;

        // Environment
        public Vector3 startPosition;
        public Vector3 exitPosition;
        public Vector3 entrancePosition;

        public Queue<Shelf> Shelves = new();
        public List<Product> Wishlist = new();
        public List<Product> Inventory = new();

        [HideInInspector] public bool hasCompletedPayment;
        [HideInInspector] public bool HasCompletedWaitngMovement;

        private void Awake()
        {
            Movement = GetComponent<CustomerMovement>();
            Animator = GetComponent<CustomerAnimator>();
            Interactor = GetComponent<CustomerInteractor>();

            Init();
        }

        private void Init()
        {
            SetWishlist();
            FindShelves();
            CustomerManager.Instance.startPosition.y = transform.localPosition.y;
            CustomerManager.Instance.exitPosition.y = transform.localPosition.y;
            CustomerManager.Instance.entrancePosition.y = transform.localPosition.y;
        }

        public void DestroyCustomer()
        {
            Wishlist.Clear();
            foreach (var product in Inventory)
            {
                if (product != null)
                {
                    product.transform.position = transform.position;
                    product.gameObject.SetActive(true);
                }
            }

            Inventory.Clear();
            Destroy(gameObject);
        }

        private void SetWishlist()
        {
            for (var i = 0; i < Mathf.Min(wishlistInitSize, CustomerManager.Instance.AvailableProducts.Count); i++)
            {
                Wishlist.Add(CustomerManager.Instance.AvailableProducts.Dequeue());
            }
        }

        private void FindShelves()
        {
            var foundShelves = Shared.GameManager.ListShelf;

            // 랜덤한 순서로 섞기
            for (int i = foundShelves.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (foundShelves[i], foundShelves[randomIndex]) = (foundShelves[randomIndex], foundShelves[i]);
            }

            foreach (var shelf in foundShelves)
            {
                Shelves.Enqueue(shelf);
            }
        }

        public void OnWaitngLineMoveComplete()
        {
            HasCompletedWaitngMovement = true;
        }
        public void OnPaymentCompleted()
        {
            hasCompletedPayment = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(startPosition, Vector3.one * 1f);
            Gizmos.DrawWireCube(exitPosition, Vector3.one * 1f);
            Gizmos.DrawWireCube(entrancePosition, Vector3.one * 1f);

            Gizmos.color = Color.red;
            foreach (var shelf in Shelves)
            {
                Gizmos.DrawWireCube(shelf.transform.position, Vector3.one * 1.5f);
            }
        }
    }
}