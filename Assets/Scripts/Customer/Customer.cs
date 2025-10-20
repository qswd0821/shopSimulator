using UnityEngine;

namespace Customer
{
    public class Customer : MonoBehaviour
    {
        public CustomerMovement Movement { get; private set; }

        public Vector3 startPosition;
        public Vector3 exitPosition;
        public Vector3 entrancePosition;
        public GameObject[] shelves;
        public GameObject checkout;

        private void Awake()
        {
            Movement = GetComponent<CustomerMovement>();
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