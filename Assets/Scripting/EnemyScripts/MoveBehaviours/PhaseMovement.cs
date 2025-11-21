using UnityEngine;
namespace CMGTSA.Enemy
{
    public class PhaseMovement : MoveBehaviour
    {
        public override void Move()
        {
            Vector3 pos = transform.position;
            Vector3 target = player.transform.position;
            Vector3 dir = target - pos;

            pos += dir.normalized * movementSpeed * Time.deltaTime;
            transform.position = pos;
        }
    }
}