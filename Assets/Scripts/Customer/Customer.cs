using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Customer
{
    /// <summary>
    /// Customer의 가장 최상위 클래스
    /// </summary>
    [RequireComponent(typeof(CustomerMovement), typeof(CustomerAnimator))]
    public class Customer : MonoBehaviour
    {
        public CustomerMovement Movement { get; private set; }
        public CustomerAnimator Animator { get; private set; }

        [Header("Customer")] public GameObject customerModel;

        public AnimatorOverrideController animatorOverrideController;
        public GameObject customerCanvas;
        public Text canvasText;


        [Header("Wishlist")] public readonly List<Product> Wishlist = new(); // 찾을 상품 리스트

        public readonly List<Product> Inventory = new(); // 찾은 상품 리스트
        public float patientTime;
        public float wishlistInitSize;


        // Environment
        public readonly Queue<Shelf> Shelves = new();
        public GameObject checkout;

        [Header("Position")] public Vector3 startPosition;

        public Vector3 exitPosition;
        public Vector3 entrancePosition;
        public Vector3 coffeeMachinePosition;

        private void Awake()
        {
            Movement = GetComponent<CustomerMovement>();
            Animator = GetComponent<CustomerAnimator>();
            FindShelves();
            SetWishlist();
            Init();
        }

        private void SetWishlist()
        {
            // 최소한의 최적화 없이 세팅
            // 추후 게임 매니저 측에서 하루 시작 시 세팅
            var productsInGame = FindObjectsByType<Product>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (productsInGame.Length == 0) return;

            var shuffledProducts = new List<Product>(productsInGame);
            for (var i = shuffledProducts.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (shuffledProducts[i], shuffledProducts[randomIndex]) =
                    (shuffledProducts[randomIndex], shuffledProducts[i]);
            }

            var itemsToAdd = Mathf.Min(shuffledProducts.Count, (int)wishlistInitSize);
            for (var i = 0; i < itemsToAdd; i++)
            {
                Wishlist.Add(shuffledProducts[i]);
            }
        }

        private void FindShelves()
        {
            var shelves = FindObjectsByType<Shelf>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            // 랜덤한 순서로 섞기
            var shuffledShelves = new List<Shelf>(shelves);
            for (int i = shuffledShelves.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (shuffledShelves[i], shuffledShelves[randomIndex]) = (shuffledShelves[randomIndex], shuffledShelves[i]);
            }

            // Queue에 추가
            foreach (var shelf in shuffledShelves)
            {
                Shelves.Enqueue(shelf);
            }
        }

        private void Init()
        {
            startPosition.y = transform.localPosition.y;
            exitPosition.y = transform.localPosition.y;
            entrancePosition.y = transform.localPosition.y;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(startPosition, Vector3.one * 1.5f);
            Gizmos.DrawWireCube(exitPosition, Vector3.one * 1.5f);
            Gizmos.DrawWireCube(entrancePosition, Vector3.one * 1.5f);
            Gizmos.DrawWireCube(checkout.transform.position, Vector3.one * 1.5f);

            Gizmos.color = Color.blue;
            foreach (var shelf in Shelves)
            {
                Gizmos.DrawWireCube(shelf.transform.position, Vector3.one * 1.5f);
            }
        }
    }
}