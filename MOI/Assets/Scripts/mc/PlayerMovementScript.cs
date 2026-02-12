using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private PlayerParentScript _parentScript;

    [SerializeField] internal float movementSpeed = 5f;
    [SerializeField] internal float jumpForce = 10f;

    void Start()
    {
        if (_parentScript == null)
            _parentScript = GetComponent<PlayerParentScript>();
    }

    void Update()
    {
        _parentScript._rb.velocity = new Vector2(_parentScript._inputScript.HorizontalInput * movementSpeed, _parentScript._rb.velocity.y);
    }

    internal void Jump()
    {
        _parentScript._rb.velocity = new Vector2(_parentScript._rb.velocity.x, jumpForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_parentScript._groundCheckTransform.position, 0.1f);
    }
}
