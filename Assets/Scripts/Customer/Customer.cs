using System.Collections.Generic;
using UnityEngine;

namespace Customer
{
    public class Customer : MonoBehaviour
    {
        public CustomerMovement Movement { get; private set; }

        public Vector3 startPosition;
        public Vector3 exitPosition;
        public Vector3 entrancePosition;
        public readonly Queue<Shelf> Shelves = new();
        public GameObject checkout;

        private void Awake()
        {
            Movement = GetComponent<CustomerMovement>();
            FindShelves();
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


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(startPosition, Vector3.one * .5f);
            Gizmos.DrawWireCube(exitPosition, Vector3.one * .5f);
            Gizmos.DrawWireCube(entrancePosition, Vector3.one * .5f);
        }
    }
}