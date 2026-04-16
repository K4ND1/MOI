using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private PlayerParentScript _parentScript;

    #region Movement Variables
    [Header("Horizontal Movement Variables")]
    internal bool canMove = true;
    internal float HorizontalInput;
    [SerializeField] internal float movementSpeed = 5f;
    [SerializeField] internal float jumpForce = 10f;
    private float accelerator = 1f;

    [Header("Jump Variables")]
    internal bool isJumping = false;

    [Header("Wall Movement Variables")]
    [SerializeField] internal float wallSlideSpeed = 2f;
    internal bool isWallSliding = false;

    [Header("Wall Jump Variables")]
    internal float wallJumpDirection;
    [SerializeField] internal float wallJumpTime = 0.5f;
    [SerializeField] internal Vector2 wallJumpForce = new Vector2(5f, 10f);
    #endregion

    #region External Forces Variables
    [Header("Gravity Variables")]
    [SerializeField] internal float baseGravity = 2f;
    [SerializeField] internal float fallMultiplier = 2f;
    [SerializeField] internal float maxFallSpeed = 18f;
    #endregion

    #region Combat Variables
    [Header("Combat Variables")]
    [SerializeField] private LayerMask attackableLayerMask;
    [SerializeField] private float basicAttackTime = 0.2f;
    [SerializeField] internal float attackRange = 0.5f;

    
    [SerializeField] internal Transform attackUpPosition;
    [SerializeField] internal Transform attackDownPosition;
    [SerializeField] internal Transform attackRightPosition;
    [SerializeField] internal Transform attackLeftPosition;


    [SerializeField] internal float pushBackMltp = 5f;
    #endregion


    #region Unity Methods
    void Start()
    {
        if (_parentScript == null) _parentScript = GetComponent<PlayerParentScript>();

    }

    void Update()
    {
        HorizontalMovement();
        Jump();
        WallSliding();
        GravityHandle();
        WallJumpProcess();
        AirBorneCheck();
        ProcessAttacks();
    }
    #endregion

    #region Movement Methods
    internal void Jump()
    {
        // Jumping from the ground
        if (GroundCheck() && Input.GetKeyDown(GameManager.Instance.PlayerJump))
        {
            _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.JumpUp, 0.2f);
            _parentScript._rb.velocity = new Vector2(_parentScript._rb.velocity.x, jumpForce);

        }

    }

    private void WallJumpProcess()
    {
        // Jumping from the wall
        if (!GroundCheck() && isWallSliding && Input.GetKeyDown(GameManager.Instance.PlayerJump))
        {
            _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.WallJump, wallJumpTime);
            // Wall jump is based in a coroutine to take avoid issues with physics right after the jump
            StartCoroutine(WallJump()); 
        }

    }

    private IEnumerator WallJump()
    {
        canMove = false; // Important flag that affects makes a wall jump uninterruptible 
     
        accelerator = 0f;
        wallJumpDirection = _parentScript.isFacingRight ? -1 : 1; // Scale velocity vector based on the direction of the player 
        _parentScript._rb.velocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
        

        ForceFilp(); // Filp the player for moslty animation purposes, but it also makes the wall jump feel better

        yield return new WaitForSeconds(wallJumpTime);
        canMove = true;
        // Regain the ability to control the player after the wall jump, and start accelerating to normal movement speed again.
        StartCoroutine(ProcessAcceleration(0f, 1f, 2 * wallJumpTime + 0.01f)); 

    }

    
    private void ForceFilp()
    {
        _parentScript.isFacingRight = !_parentScript.isFacingRight;

        Vector3 ls = transform.localScale;
        ls.x *= -1f;
        transform.localScale = ls;
    }
    



    /// <summary>
    /// Coroutine ProcessAcceleration() needs to be rewritten into a more general method.
    /// This change should include making the method reusable for any change in acceleration.
    /// 
    /// The new ProccessAcceleration() method should be using a reference of a variable that is being changed, 
    /// instead of directly changing the accelerator variable. Also the idea is to put the whole thing inside of
    /// the GameManager script so it can be accesed through the instance and used by any script.
    /// <summary>
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
    }// REWORK THIS


    private void HorizontalMovement()
    {
        HorizontalInput = 
            (Input.GetKey(GameManager.Instance.MovePlayerRight) ? 1f : 0f) 
            - (Input.GetKey(GameManager.Instance.MovePlayerLeft) ? 1f : 0f);

        // Handle Horizontal input using the custom input system from the GameManager script
        if (!canMove) return;

        // Handle the animation of the player
        if (GroundCheck())
        {
            if (HorizontalInput != 0) 
                _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.Run, 0);
            else
                _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.Idle, 0);
        }

        // Changle players velocity based on input and movement speed
        float desiredVelocityX = HorizontalInput * movementSpeed;
        _parentScript._rb.velocity = new Vector2(Mathf.Lerp(_parentScript._rb.velocity.x, desiredVelocityX, accelerator), _parentScript._rb.velocity.y);

        
    }
    #endregion

    #region Combat Methods
    private void ProcessAttacks()
    {
        List<KeyCode> possibleAttacks = new List<KeyCode>()
        {
            GameManager.Instance.AttackUp,
            GameManager.Instance.AttackDown,
            GameManager.Instance.AttackRight,
            GameManager.Instance.AttackLeft
        };

        foreach (KeyCode possibleInput in possibleAttacks)
        {
            if(Input.GetKeyDown(possibleInput) && canMove)
            {
                StartCoroutine(AttackCoroutine(possibleInput, pushBackMltp));
            }
        }
    }

    private IEnumerator AttackCoroutine(KeyCode keyPressed, float attackPower)
    {
        canMove = false;

        Vector2 recoilDir = Vector2.zero;
        Vector2 attackPos = Vector2.zero;

        // Make the animation time slightly longer than the attack duration to ensure it plays fully even if the attack hits something immediately.
        float animDuration = basicAttackTime + 0.05f; 

        // Determine attack direction, position, and animation
        if (keyPressed == GameManager.Instance.AttackDown)
        {
            recoilDir = Vector2.up;
            attackPos = attackDownPosition.position;
            _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.AttackDown, animDuration);
        }
        else if (keyPressed == GameManager.Instance.AttackUp)
        {
            recoilDir = Vector2.down;
            attackPos = attackUpPosition.position;
            _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.AttackUp, animDuration);
        }
        else
        {
            // Make it possible to compare isFacingRight with attack direction for easier calculation of recoil, direction and animation
            bool isPressingRight = keyPressed == GameManager.Instance.AttackRight;

            if (isPressingRight == _parentScript.isFacingRight)
            {
                attackPos = attackRightPosition.position;
                _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.AttackRight, animDuration);
            }
            else
            {
                attackPos = attackLeftPosition.position;
                _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.AttackLeft, animDuration);
            }
        }

        // Collison check for attackable objects
        bool hitSomething = Physics2D.OverlapCircle(attackPos, attackRange, attackableLayerMask);

        if (hitSomething)
        {

            // Apply recoil to the player
            _parentScript._rb.velocity = recoilDir * attackPower;
            float gravStorage = baseGravity;
            baseGravity = 0f;

            yield return new WaitForSeconds(basicAttackTime);

            baseGravity = gravStorage; // Restore physics
        }
        else
        {
            yield return new WaitForSeconds(basicAttackTime);
        }

        // Restore movement at the very end
        canMove = true;
    }
    #endregion

    #region Physics Methods
    private void WallSliding()
    {
        if (WallCheck() && !GroundCheck() && HorizontalInput != 0)
        {
            // Using math max set a limit to the players falling speed
            isWallSliding = true;
            _parentScript._rb.velocity = new Vector2(_parentScript._rb.velocity.x, Mathf.Max(_parentScript._rb.velocity.y, -wallSlideSpeed));
     
            _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.SlideDown, 0); // Take care of the wall sliding animation
        }
        else
        {
            isWallSliding = false;
        }
    }

    internal void GravityHandle()
    {
        // If the player is falling, apply stronger gravity to make the whole thing feel smoother
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

    #region Checks
    internal bool GeneralContactCheck()
    {
        return GroundCheck() || WallCheck();
    }

    internal bool GroundCheck()
    {
        return Physics2D.OverlapCircle(_parentScript._groundCheckTransform.position, _parentScript.groundCheckRadius, _parentScript._groundLayerMask);
    }

    internal bool WallCheck()
    {
        return Physics2D.OverlapBox(_parentScript._wallCheckTransform.position, _parentScript.wallCheckSize, 0f, _parentScript._wallLayerMask);
    }

    internal bool AirBorneCheck()
    {
        //Check if the player is non stationary in the air
        if (_parentScript._rb.velocity.y != 0 && !GeneralContactCheck())
        {
            _parentScript._animScript.StartGivenAnimation(PlayerAnimScript.JumpDown, 0);
            return true;
        }
        else return false;

    }
    #endregion

}
