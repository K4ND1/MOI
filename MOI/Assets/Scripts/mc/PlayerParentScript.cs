using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParentScript : MonoBehaviour
{
    #region References
    [Header("Scripts")]
    [SerializeField] internal PlayerMovementScript _movementScript;
    [SerializeField] internal PlayerInputScript _inputScript;
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
    #endregion


    internal bool isFacingRight = true;


    private void Awake()
    {
        if (_movementScript == null) _movementScript = GetComponent<PlayerMovementScript>();
        if (_inputScript == null) _inputScript = GetComponent<PlayerInputScript>();
        if (_animScript == null) _animScript = GetComponent<PlayerAnimScript>();

        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_groundCheckTransform == null) _groundCheckTransform = gameObject.transform.Find("GroundCheck");
    }

    private void Update()
    {
        Flip();
    }

    internal void ChangeAnimState(string newState)
    {
        if (_animScript.CurrentAnimState == newState) return;

        _animScript.CurrentAnimState = newState;
        _animator.Play(newState);
    }

    internal void Flip()
    {
        if (isFacingRight && _inputScript.HorizontalInput < 0 || !isFacingRight && _inputScript.HorizontalInput > 0)
        {
            isFacingRight = !isFacingRight;

            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheckTransform.position, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_wallCheckTransform.position, wallCheckSize);
    }
}
