using UnityEngine;
namespace CMGTSA.Enemy
{
    public abstract class MoveBehaviour : MonoBehaviour
    {
        [HideInInspector] protected PlayerController player;
        [SerializeField] protected float movementSpeed;
        private void Start()
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
        }
        private void Update()
        {
            Move();
        }
        public abstract void Move();
    }
}