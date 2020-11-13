using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceBossKickBehaviour : StateMachineBehaviour
{
 bool facingLeft;
    Vector3 originalScale;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        originalScale = animator.transform.localScale;
        facingLeft = animator.transform.localScale.z == 1;
        if (facingLeft)
        {
            animator.transform.localScale = new Vector3(-1, 1,1);
        } else
        {
            animator.transform.localScale = new Vector3(1, 1,1);
        }

      //  sprite.flipX = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.localScale = originalScale;
       // sprite.flipX = false;
    }

}


