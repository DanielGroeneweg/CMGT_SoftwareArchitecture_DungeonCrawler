using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
namespace CMGTSA
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        #region Inspector
        [Serializable]
        private class InputControl
        {
            public ActionTypes action;
            public InputTypes input;
            [ShowIf("input", ComparisonTypes.Equals, InputTypes.Keyboard)]
            public KeyCode keyCode;
            [ShowIf("input", ComparisonTypes.Equals, InputTypes.Mouse)]
            public MouseButton button;
        }
        [Header("Stats")]
        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private float maxMoveSpeed = 10;
        [SerializeField] private float groundedMaxRange = 0.2f;
        [Tooltip("Top control takes top priority in actions the player takes")]
        [SerializeField] private List<InputControl> controls = new List<InputControl>();
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer sprite;
        #endregion

        #region Internal
        private bool grounded = false;
        private bool jumping = false;
        private bool attacking = false;
        private Directions moveDirection;
        #endregion
        #endregion
        private void Update()
        {
            grounded = CheckGrounded();

            foreach (InputControl control in controls)
            {
                bool input = false;

                if (control.input == InputTypes.Keyboard && Input.GetKey(control.keyCode)) input = true;
                else if (control.input == InputTypes.Mouse && Input.GetMouseButtonDown(0)) input = true;

                if (input)
                {
                    switch (control.action)
                    {
                        case ActionTypes.Attack:
                            if (!attacking && !jumping) attacking = true;
                            break;
                        case ActionTypes.Jump:
                            if (grounded && !attacking) jumping = true;
                            break;
                        case ActionTypes.MoveLeft:
                            if (grounded && !attacking) moveDirection = Directions.Left;
                            break;
                        case ActionTypes.MoveRight:
                            if (grounded && !attacking) moveDirection = Directions.Right;
                            break;
                    }
                }
            }
        }
        private void FixedUpdate()
        {

        }
        private bool CheckGrounded()
        {
            foreach (RaycastHit hit in Physics.RaycastAll(transform.position, Vector3.down, groundedMaxRange))
            {
                if (hit.transform.tag == "Ground") return true;
            }

            return false;
        }
        #region additional
        private void OnValidate()
        {
            foreach (ActionTypes actionType in Enum.GetValues(typeof(ActionTypes)))
            {
                // Example: Ensure each InputType has an entry in your controls list
                if (!controls.Exists(c => c.action == actionType))
                {
                    controls.Add(new InputControl { action = actionType, input = InputTypes.Keyboard });
                }
            }
        }
        #endregion
    }
}