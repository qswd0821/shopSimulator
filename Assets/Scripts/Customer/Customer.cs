using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Customer
{
    /// <summary>
    /// Customer의 가장 최상위 클래스
    /// </summary>
    [RequireComponent(typeof(CustomerMovement), typeof(CustomerAnimator)), RequireComponent(
         typeof(CustomerStateMachine))]
    public class Customer : MonoBehaviour, ICustomerInteraction
    {
        public CustomerMovement Movement { get; private set; }
        public CustomerAnimator Animator { get; private set; }
        public CustomerStateMachine StateMachine { get; private set; }

        [Header("Customer")] public GameObject customerCanvas;
        public int wishlistInitSize = 3;
        public float ragdollDelayBeforeDestroyed = 3f;

        public ICustomerState DefaultState = new CustomerEnteringState();
        public Queue<Shelf> Shelves = new();
        public List<Product> Wishlist = new();
        public List<Product> Inventory = new();

        [HideInInspector] public bool hasCompletedPayment;
        [HideInInspector] public bool HasCompletedWaitngMovement;

        private void Awake()
        {
            Movement = GetComponent<CustomerMovement>();
            Animator = GetComponent<CustomerAnimator>();
            StateMachine = GetComponent<CustomerStateMachine>();

            Movement.enabled = false;
            Animator.enabled = false;
            StateMachine.enabled = false;
        }

        public void Init(GameObject model, ICustomerState initState = null)
        {
            SetModel(model);
            SetWishlist();
            FindShelves();

            Movement.enabled = true;
            Animator.enabled = true;
            StateMachine.enabled = true;

            StateMachine.StartState(initState ?? DefaultState);
        }

        private void SetModel(GameObject model)
        {
            model.transform.SetParent(transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
            model.SetActive(true);
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
            Gizmos.color = Color.red;
            foreach (var shelf in Shelves)
            {
                Gizmos.DrawWireCube(shelf.transform.position, Vector3.one * 1.5f);
            }
        }

        public void Attack()
        {
            // Enable ragdoll physics
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(Vector3.one * Random.Range(-1f, 1f));
            }

            Animator.DisableAnimator();

            Movement.enabled = false;
            Animator.enabled = false;
            StateMachine.enabled = false;

            Invoke(nameof(DestroyCustomer), ragdollDelayBeforeDestroyed);
        }
    }
}