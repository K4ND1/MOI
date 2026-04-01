using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimScript : MonoBehaviour
{
    [SerializeField] private PlayerParentScript _parentScript;

    private int currentAnimState;
    private int currentPiro;

    #region Anim Names
    internal static readonly int Idle = Animator.StringToHash("idle");
    internal static readonly int Run = Animator.StringToHash("run");
    internal static readonly int JumpUp = Animator.StringToHash("jump_up");
    internal static readonly int JumpDown = Animator.StringToHash("jump_down");
    internal static readonly int SlideDown = Animator.StringToHash("slide_down");
    internal static readonly int WallJump = Animator.StringToHash("wall_jump");
    [SerializeField] private float WallJump_t = 0.5f;
    #endregion

    private float lockedTill;

    /*
     * List of priorities for animations:
     * Walljump > JumpUp > SlideDown > JumpDown > Run > Idle
     * 
     */




    internal void StartGivenAnimation(int anim_id, float locked_time)
    {
        if (anim_id == currentAnimState || Time.time < lockedTill) return;

        currentAnimState = anim_id;

        _parentScript._animator.CrossFade(LockedState(anim_id, locked_time), 0, 0);

        // Method to lock the animation for a certain time, preventing it from being interrupted by other animations
        int LockedState(int s, float t)
        {
            lockedTill = Time.time + t;
            return s;
        }

    }

}
