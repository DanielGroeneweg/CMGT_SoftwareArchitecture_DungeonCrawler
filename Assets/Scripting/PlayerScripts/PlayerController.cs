using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Reflection;

namespace CMGTSA
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        #region Inspector
        [Header("Stats")]
        [SerializeField] private float groundedRange;
        [SerializeField] private float jumpForce;
        [SerializeField] private float attackDuration;
        [Header("References")]
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rb;
        #endregion

        #region Public

        #endregion

        #region Private
        Vector2 move;
        bool pressedJump = false;
        bool pressedAttack = false;
        bool isAttacking = false;
        bool isJumping = false;
        bool grounded = false;
        bool isRunning = false;
        bool isIdling = false;
        #endregion
        #endregion
        private void FixedUpdate()
        {
            CheckGrounded();

            Jumping();

            Attacking();

            Movement();

            ResetInput();
        }
        private void Jumping()
        {
            // Reset Jumping
            if (grounded && isJumping)
            {
                isJumping = false;
            }

            // Start Jumping
            if (grounded && pressedJump && !isAttacking)
            {
                isIdling = false;
                isRunning = false;
                isJumping = true;
                rb.AddForceY(jumpForce, ForceMode2D.Force);
            }
        }
        private void Attacking()
        {
            if (pressedAttack && !isAttacking)
            {
                isRunning = false;
                isIdling = false;
                isAttacking = true;
                SetAnimation(Animations.Attack);
                StartCoroutine(DisableBool(nameof(isAttacking), false, attackDuration));
            }
        }
        private void Movement()
        {
            if (!grounded || isAttacking || isJumping) return;

            bool isMoving = move != Vector2.zero;

            if (isMoving)
            {
                if (!isRunning)
                {
                    isRunning = true;
                    isIdling = false;
                    SetAnimation(Animations.Running);
                }

                sprite.flipX = move.x < 0;
            }
            else
            {
                if (!isIdling)
                {
                    isRunning = false;
                    isIdling = true;
                    StartCoroutine(DelayedIdle());
                }
            }
        }
        private void CheckGrounded()
        {
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, Vector2.down, groundedRange))
            {
                if (hit.transform.tag == "Ground")
                {
                    isJumping = false;
                    grounded = true;
                    return;
                }
            }
            grounded = false;
        }
        #region UtilityMethods
        private void SetAnimation(Animations animation)
        {
            animator.SetTrigger(animation.ToString());
        }
        private IEnumerator DisableBool(string boolName, bool value, float time)
        {
            yield return new WaitForSeconds(time);

            // Find a field with the given name (public or private)
            FieldInfo field = GetType().GetField(boolName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null && field.FieldType == typeof(bool))
            {
                field.SetValue(this, value);
                Debug.Log("value set!");
            }
            else
            {
                Debug.LogWarning($"No bool field named '{boolName}' found on {name}.");
            }
        }
        private IEnumerator DelayedIdle()
        {
            yield return new WaitForSeconds(0.1f);
            if (isIdling) SetAnimation(Animations.Idling);
        }
        #endregion

        #region Input
        public void OnMove(InputValue _Input)
        {
            move = _Input.Get<Vector2>();
        }
        public void OnJump(InputValue _Input)
        {
            pressedJump = true;
        }
        public void OnAttack(InputValue _Input)
        {
            pressedAttack = true;
        }
        private void ResetInput()
        {
            pressedJump = false;
            pressedAttack = false;
        }
        #endregion
    }
}