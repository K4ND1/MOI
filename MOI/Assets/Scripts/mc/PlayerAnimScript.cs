using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimScript : MonoBehaviour
{
    [SerializeField] private PlayerParentScript _parentScript;

    internal string CurrentAnimState;

    private string IDLE_ANIM_NAME = "idle";
    private string RUN_ANIM_NAME = "run";
    private string JUMP_UP_ANIM_NAME = "jump_up";
    private string JUMP_DOWN_ANIM_NAME = "jump_down";

    private void Start()
    {
        if (CurrentAnimState == null)
            CurrentAnimState = IDLE_ANIM_NAME;

        if (_parentScript == null)
            _parentScript = GetComponent<PlayerParentScript>();
    }

    private void Update()
    {
        if (_parentScript._inputScript.HorizontalInput > 0)
            _parentScript._spriteRenderer.flipX = false;
        else
            _parentScript._spriteRenderer.flipX = true;


        if (_parentScript._rb.velocity.y != 0 && !_parentScript._inputScript.GroundCheck())
        {
            if (_parentScript._rb.velocity.y > 0)
            {
                _parentScript.ChangeAnimState(JUMP_UP_ANIM_NAME);
                return;
            }
            else
            {
                _parentScript.ChangeAnimState(JUMP_DOWN_ANIM_NAME);
                return;
            }
        }

        if (_parentScript._inputScript.HorizontalInput != 0)
        {
            _parentScript.ChangeAnimState(RUN_ANIM_NAME);
            return;
        }


        _parentScript.ChangeAnimState(IDLE_ANIM_NAME);
    }
}
