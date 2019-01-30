﻿#define DEBUGMODE
using UnityEngine;
using System.Collections;

public class PlayerInputHandler : MonoBehaviour
{
    public float playerSpeed = 20f;
    public float jumpHeight = 3f;
    public float groundDamping = 10f;
    public float airDamping = 5f;

    public bool pushGripActive { get; set; }
    public bool pullGripActive { get; set; }
    public bool handleActive { get; set; }

    public bool isJumping;

    public bool canMove;

    public bool playerIsDead { get; set; }

    public bool tirolesaActive;

    public Vector3 playerGravity;
    public Vector3 _velocity;
    private Vector3 moveDirection;

    private Collider[] hitColliders;

    private PrototypeCharacterControllerv2 _controller;

#if DEBUGMODE
    private Renderer _renderer;
#endif

    #region MonoBehaviour
    private void Awake()
    {
        //canMove = true;
        tirolesaActive = false;
        _controller = GetComponent<PrototypeCharacterControllerv2>();

#if DEBUGMODE
        _renderer = GetComponent<Renderer>();
#endif
    }

    private void Update()
    {
        if (_controller.isGrounded)
            isJumping = false;

        Interact();

        _velocity = _controller.velocity;
        Time.timeScale = Input.GetKey(KeyCode.Space) ? 0.1f : 1f;

#if DEBUGMODE
        _renderer.material.color = _controller.isGrounded ? Color.green : Color.red;
#endif

        float horizontalAxis = Input.GetAxis("Horizontal");

        bool jumpKeys = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        if (jumpKeys && _controller.isGrounded && !pushGripActive || jumpKeys && _controller.isGrounded && !handleActive)
        {
            if (!pushGripActive && !pullGripActive)
            {
                isJumping = true;
                _velocity.y = Mathf.Sqrt(2f * jumpHeight * -playerGravity.y);
            }
        }

        float movementDamping = _controller.isGrounded ? groundDamping : airDamping;

        _velocity.x = Mathf.Lerp(_velocity.x, horizontalAxis * playerSpeed, Time.deltaTime * movementDamping/3f); //MUDANÇA AQUI!!! DIVIDI POR 3!!! ASS.: RODRIGO VIEIRA
        _velocity += playerGravity * Time.deltaTime;

        if (canMove)
            _controller.Move(_velocity * Time.deltaTime);

        if (horizontalAxis != 0 && !pushGripActive && !pullGripActive)
            this.transform.forward = Vector3.Normalize(new Vector3(horizontalAxis, 0, 0));
    }

    private void Interact()
    {
        if (Input.GetKey(KeyCode.K))
        {
            float moveDir = Input.GetAxis("Horizontal");
            hitColliders = Physics.OverlapBox(this.transform.position + transform.forward, Vector3.one * 1f);
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject.CompareTag("PushBox")) // caixa de Empurrar
                {
                    Debug.Log("CAIXA DE EMPURRAR");
                    pushGripActive = true;
                    if (moveDir > 0f && col.gameObject.transform.position.x > this.transform.position.x ||
                        moveDir < 0f && col.gameObject.transform.position.x < this.transform.position.x)
                    {
                        playerSpeed = 3f;
                        col.gameObject.transform.SetParent(this.transform);
                        col.gameObject.layer = 9; // Layer MoveableObject
                    }
                    else
                    {
                        pushGripActive = false;
                        col.gameObject.transform.SetParent(null);
                        col.gameObject.layer = 0;
                    }
                    
                }
                if (col.gameObject.CompareTag("PullBox")) // caixa de puxar
                {
                    Debug.Log("CAIXA DE PUXAR");
                    pullGripActive = true;
                    if (moveDir < 0f && col.gameObject.transform.position.x > this.transform.position.x ||
                        moveDir > 0f && col.gameObject.transform.position.x < this.transform.position.x)
                    {
                        playerSpeed = 3f;
                        col.gameObject.transform.SetParent(this.transform);
                        col.gameObject.layer = 9;
                    }
                    else
                    {
                        pullGripActive = false;
                        col.gameObject.transform.SetParent(null);
                        col.gameObject.layer = 0;
                    }
                }
                if (col.gameObject.CompareTag("Handle")) // Alavanca
                {
                    //TODO Ativar alavanca
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            playerSpeed = 8f;
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject.CompareTag("PushBox")) // caixa de empurrar
                {
                    pushGripActive = false;
                    col.gameObject.transform.SetParent(null);
                    col.gameObject.layer = 0;
                }
                if (col.gameObject.CompareTag("PullBox")) // caixa de puxar
                {
                    pullGripActive = false;
                    col.gameObject.transform.SetParent(null);
                    col.gameObject.layer = 0;
                }
            }
        }
    }

    public void KillPlayer()
    {
        playerIsDead = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position + transform.forward, Vector3.one * 2f);
    }
    #endregion
}