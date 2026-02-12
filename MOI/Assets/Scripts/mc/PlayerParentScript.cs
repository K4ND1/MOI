using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParentScript : MonoBehaviour
{
    #region References
    [SerializeField] internal PlayerMovementScript _movementScript;
    [SerializeField] internal PlayerInputScript _inputScript;
    [SerializeField] internal PlayerAnimScript _animScript;

    [SerializeField] internal Rigidbody2D _rb;
    [SerializeField] internal Animator _animator;
    [SerializeField] internal SpriteRenderer _spriteRenderer;

    [SerializeField] internal Transform _groundCheckTransform;
    [SerializeField] internal LayerMask _groundLayerMask;

    #endregion 


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

    internal void ChangeAnimState(string newState)
    {
        if (_animScript.CurrentAnimState == newState) return;

        _animScript.CurrentAnimState = newState;
        _animator.Play(newState);
    }
}
