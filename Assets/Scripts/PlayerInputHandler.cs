﻿#define DEBUGMODE
using UnityEngine;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour
{
    public float playerSpeed = 20f;
    public float jumpHeight = 3f;
    public float groundDamping = 10f;
    public float airDamping = 5f;

    public bool gripActive { get; set; }
    public bool handleActive { get; set; }

    public Vector3 playerGravity;
    private Vector3 _velocity;
    private Vector3 moveDirection;

    private Collider[] hitColliders;

    private PrototypeCharacterControllerv2 _controller;

#if DEBUGMODE
    private Renderer _renderer;
#endif

    #region MonoBehaviour
    private void Awake()
    {
        _controller = GetComponent<PrototypeCharacterControllerv2>();

#if DEBUGMODE
        _renderer = GetComponent<Renderer>();
#endif
    }

    private void Update()
    {
        _velocity = _controller.velocity;
        Time.timeScale = Input.GetKey(KeyCode.Space) ? 0.1f : 1f;

#if DEBUGMODE
        _renderer.material.color = _controller.isGrounded ? Color.green : Color.red;
#endif

        float horizontalAxis = Input.GetAxis("Horizontal");

        bool jumpKeys = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        if (jumpKeys && _controller.isGrounded && !gripActive || jumpKeys && _controller.isGrounded && !handleActive)
            _velocity.y = Mathf.Sqrt(2f * jumpHeight * -playerGravity.y);

        float movementDamping = _controller.isGrounded ? groundDamping : airDamping;

        _velocity.x = Mathf.Lerp(_velocity.x, horizontalAxis * playerSpeed, Time.deltaTime * movementDamping);
        _velocity += playerGravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

        if (horizontalAxis != 0 && !gripActive)
            this.transform.forward = Vector3.Normalize(new Vector3(horizontalAxis, 0, 0));
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.K))
        {
            hitColliders = Physics.OverlapBox(this.transform.position + transform.forward, Vector3.one * 1f);
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject.CompareTag("Box"))
                {
                    gripActive = true;
                    col.gameObject.transform.SetParent(this.transform);
                    col.gameObject.layer = 9; // Layer MoveableObject
                }

                if (col.gameObject.CompareTag("Handle")) // Lever
                {
                    handleActive = true;
                    col.gameObject.transform.SetParent(this.transform);
                    col.gameObject.layer = 9;
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject.CompareTag("Box"))
                {
                    gripActive = false;
                    col.gameObject.transform.SetParent(null);
                    col.gameObject.layer = 0;
                }

                if (col.gameObject.CompareTag("Handle"))
                {
                    handleActive = false;
                    col.gameObject.transform.SetParent(null);
                    col.gameObject.layer = 0;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position + transform.forward, Vector3.one * 2f);
    }
    #endregion
}