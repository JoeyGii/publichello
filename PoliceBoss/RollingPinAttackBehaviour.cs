using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RollingPinAttackBehaviour : StateMachineBehaviour
{
    public delegate void RollingPinEvent();
    public static event RollingPinEvent PinAnimationOver;
    GameObject player;
    Rigidbody2D myRigidbody2D;
    float movementSpeed = 100f;
    Vector2 newPosPlayer;
    CapsuleCollider2D myCollider;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        myCollider = animator.GetComponent<CapsuleCollider2D>();
        myRigidbody2D = animator.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector2 targetPlayer = new Vector2(myRigidbody2D.position.x,myRigidbody2D.position.y -2f);
         newPosPlayer = Vector2.MoveTowards(myRigidbody2D.position, targetPlayer, movementSpeed * Time.fixedDeltaTime);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    
        if (!myCollider.IsTouchingLayers(LayerMask.GetMask("Foreground"))){
            myRigidbody2D.MovePosition(newPosPlayer);
        }
        else {
         
            newPosPlayer = Vector2.MoveTowards(myRigidbody2D.position,new Vector2 (myRigidbody2D.position.x,myRigidbody2D.position.y+1.75f), movementSpeed * Time.fixedDeltaTime);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     
        PinAnimationOver?.Invoke();
    }


}
