using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParentScript : MonoBehaviour
{
    #region References
    [Header("Scripts")]
    [SerializeField] internal PlayerMovementScript _movementScript;
    [SerializeField] internal PlayerAnimScript _animScript;

    [Header("Components")]
    [SerializeField] internal Rigidbody2D _rb;
    [SerializeField] internal Animator _animator;
    [SerializeField] internal SpriteRenderer _spriteRenderer;
    #endregion

    #region Checks
    [Header("Ground Check")]
    [SerializeField] internal Transform _groundCheckTransform;
    [SerializeField] internal LayerMask _groundLayerMask;
    [SerializeField] internal float groundCheckRadius = 0.1f;

    [Header("WallCheck")]
    [SerializeField] internal Transform _wallCheckTransform;
    [SerializeField] internal LayerMask _wallLayerMask;
    [SerializeField] internal Vector2 wallCheckSize = new Vector2(0.5f, 0.5f);
    
    internal bool isFacingRight = true;
    #endregion



    #region Unity Methods
    private void Awake()
    {
        #region Component References
        if (_movementScript == null) _movementScript = GetComponent<PlayerMovementScript>();
        if (_animScript == null) _animScript = GetComponent<PlayerAnimScript>();

        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        #endregion

        #region Child Transforms
        if (_groundCheckTransform == null) _groundCheckTransform = gameObject.transform.Find("GroundCheck");
        if (_wallCheckTransform == null) _wallCheckTransform = gameObject.transform.Find("WallCheck");
        #endregion


    }

    private void Update()
    {
        ProcessFlip();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_wallCheckTransform.position, Vector2.right * (isFacingRight ? 1 : -1) * wallCheckSize.x);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the ground check sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckTransform.position, 0.1f);

        // Draw the wall check box
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_wallCheckTransform.position, wallCheckSize);

        // Draw attack range circles for debugging purposes
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_movementScript.attackUpPosition.position, _movementScript.attackRange);
        Gizmos.DrawWireSphere(_movementScript.attackDownPosition.position, _movementScript.attackRange);
        Gizmos.DrawWireSphere(_movementScript.attackRightPosition.position, _movementScript.attackRange);
        Gizmos.DrawWireSphere(_movementScript.attackLeftPosition.position, _movementScript.attackRange);

    }
    #endregion


    internal void ProcessFlip()
    {

        if (_movementScript.canMove == false) return; // Don't flip when player can't move

        if (isFacingRight && _movementScript.HorizontalInput < 0 || !isFacingRight && _movementScript.HorizontalInput > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }

    }

}
