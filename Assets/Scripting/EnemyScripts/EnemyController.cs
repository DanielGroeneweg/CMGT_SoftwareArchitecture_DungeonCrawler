using UnityEngine;
namespace CMGTSA.Enemy
{
    [RequireComponent(typeof(AttackBehaviour))]
    [RequireComponent(typeof(MoveBehaviour))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private AttackBehaviour attackBehaviour;
        [SerializeField] private MoveBehaviour moveBehaviour;
    }
}