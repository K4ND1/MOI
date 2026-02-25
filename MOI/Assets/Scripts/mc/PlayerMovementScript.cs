using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private PlayerParentScript _parentScript;

    [Header("Movement Settings")]
    [SerializeField] internal float movementSpeed = 5f;
    [SerializeField] internal float jumpForce = 10f;

    [Header("Wall Movement Settings")]
    [SerializeField] internal float wallSlideSpeed = 2f;
    internal bool isWallSliding = false;

    [Header("Gravity Settings")]
    [SerializeField] internal float baseGravity = 2f;
    [SerializeField] internal float fallMultiplier = 2f;
    [SerializeField] internal float maxFallSpeed = 18f;

    void Start()
    {
        if (_parentScript == null)
            _parentScript = GetComponent<PlayerParentScript>();
    }

    void Update()
    {
        HorizontalMovement();
        GravityHandle();
    }

    internal void Jump()
    {
        _parentScript._rb.velocity = new Vector2(_parentScript._rb.velocity.x, jumpForce);
    }

    private void HorizontalMovement()
    {
        _parentScript._rb.velocity = new Vector2(_parentScript._inputScript.HorizontalInput * movementSpeed, _parentScript._rb.velocity.y);
    }

    private void WallSliding()
    {
        if (_parentScript._inputScript.WallCheck() && !_parentScript._inputScript.GroundCheck() && _parentScript._inputScript.HorizontalInput != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    internal void GravityHandle()
    {
        if (_parentScript._rb.velocity.y < 0)
        {
            _parentScript._rb.gravityScale = baseGravity * fallMultiplier;
            _parentScript._rb.velocity = new Vector2(_parentScript._rb.velocity.x, Mathf.Max(_parentScript._rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            _parentScript._rb.gravityScale = baseGravity;
        }
    }

}
