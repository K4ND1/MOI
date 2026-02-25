using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{

    [SerializeField] private PlayerParentScript _parentScript;

    internal float HorizontalInput;
    internal bool JumpInput;

    private void Start()
    {
        if (_parentScript == null)
            _parentScript = GetComponent<PlayerParentScript>();
    }

    private void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && GroundCheck())
        {
            _parentScript._movementScript.Jump();
        }

    }

    internal bool GroundCheck()
    {
        return Physics2D.OverlapCircle(_parentScript._groundCheckTransform.position, _parentScript.groundCheckRadius, _parentScript._groundLayerMask);
    }

    internal bool WallCheck()
    {
        return Physics2D.OverlapBox(_parentScript._wallCheckTransform.position, _parentScript.wallCheckSize, 0f, _parentScript._wallLayerMask);
    }

}
