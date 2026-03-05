using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private PlayerParentScript _parentScript;

    [Header("Movement Settings")]
    private bool canMove = true;
    internal float HorizontalInput;
    [SerializeField] internal float movementSpeed = 5f;
    [SerializeField] internal float jumpForce = 10f;
    private float accelerator = 1f;

    [Header("Wall Movement Settings")]
    [SerializeField] internal float wallSlideSpeed = 2f;
    internal bool isWallSliding = false;

    [Header("Gravity Settings")]
    [SerializeField] internal float baseGravity = 2f;
    [SerializeField] internal float fallMultiplier = 2f;
    [SerializeField] internal float maxFallSpeed = 18f;

    [Header("Wall Jump Settings")]
    internal bool isWallJumping = false;
    internal float wallJumpDirection;
    [SerializeField] internal float wallJumpTime = 0.5f;
    //internal float wallJumpTimer;
    [SerializeField] internal Vector2 wallJumpForce = new Vector2(5f, 10f);
    

    #region Unity Methods
    void Start()
    {
        if (_parentScript == null)
            _parentScript = GetComponent<PlayerParentScript>();
    }

    void Update()
    {
        HorizontalMovement();
        Jump();
        WallSliding();
        GravityHandle();
        WallJumpProcess();
        //Walljump();
    }
    #endregion

    #region Movement Methods
    internal void Jump()
    {
        if (GroundCheck() && Input.GetKeyDown(KeyCode.Space))
        {
            _parentScript._rb.velocity = new Vector2(_parentScript._rb.velocity.x, jumpForce);
            return;
        }

    }

    private void WallJumpProcess()
    {
        if (!GroundCheck() && isWallSliding && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Wall Jump");
            StartCoroutine(WallJump());
        }
    }

    private IEnumerator WallJump()
    {
        isWallJumping = true;
        canMove = false;
        accelerator = 0f;
        wallJumpDirection = _parentScript.isFacingRight ? -1 : 1;
        _parentScript._rb.velocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
        
        ForceFilp();

        yield return new WaitForSeconds(wallJumpTime);

        canMove = true;
        StartCoroutine(ProcessAcceleration(0f, 1f, 2 * wallJumpTime + 0.01f));

        isWallJumping = false;
    }

    private void ForceFilp()
    {
        _parentScript.isFacingRight = !_parentScript.isFacingRight;

        Vector3 ls = transform.localScale;
        ls.x *= -1f;
        transform.localScale = ls;
    }

    private IEnumerator ProcessAcceleration(float start, float end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            accelerator = Mathf.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        accelerator = end;
    }

    private void HorizontalMovement()
    {
        if (!canMove) return;

        HorizontalInput = Input.GetAxisRaw("Horizontal");
        float desiredVelocityX = HorizontalInput * movementSpeed;
        _parentScript._rb.velocity = new Vector2(Mathf.Lerp(_parentScript._rb.velocity.x, desiredVelocityX, accelerator), _parentScript._rb.velocity.y);
    }
    #endregion

    #region Physics Methods
    private void WallSliding()
    {
        if (WallCheck() && !GroundCheck() && HorizontalInput != 0)
        {
            isWallSliding = true;
            _parentScript._rb.velocity = new Vector2(_parentScript._rb.velocity.x, Mathf.Max(_parentScript._rb.velocity.y, -wallSlideSpeed));
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
    #endregion

    #region Contact Checks
    internal bool GroundCheck()
    {
        return Physics2D.OverlapCircle(_parentScript._groundCheckTransform.position, _parentScript.groundCheckRadius, _parentScript._groundLayerMask);
    }

    internal bool WallCheck()
    {
        return Physics2D.OverlapBox(_parentScript._wallCheckTransform.position, _parentScript.wallCheckSize, 0f, _parentScript._wallLayerMask);
    }
    #endregion

}
