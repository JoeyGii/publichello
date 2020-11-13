using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PBossJumpKickBehaviour : MonoBehaviour
{
    Animator myAnimator;
    Rigidbody2D myRigidbody2D;
    [SerializeField] float liftVelocity = 5;
    [SerializeField] float dropVelocity = 10;
    GameObject player;
    Collider2D myCollider;
    Vector2 target;
    Light2D light2D;
    private bool lightLerp = false;
    private float stopTime = 0.5f;
    private bool descend = false;
    private bool downwardsKick = false;
    private bool touchingForeground;
    PBDissappear dissappearScript;

    private void Awake()
    {
        dissappearScript = GetComponent<PBDissappear>();
    }

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        myCollider = GetComponent<Collider2D>();
        light2D = GetComponent<Light2D>();
    }


    private void Update()
    {
        if (dissappearScript.invisibility)
        {
            return;
        }
        TouchingPlayer();
        touchingForeground = myCollider.IsTouchingLayers(LayerMask.GetMask("Foreground"));
        TurnOffJumpKick();
        LightFlash();
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpKickLift") && transform.localPosition.y < 2 && !descend)
        {
            
            downwardsKick = false;
            AscendPhysics();
        }
        else if (!touchingForeground)
        {
            StartCoroutine(StartDownwardsKick());
         
        } else if (touchingForeground)
        {
            downwardsKick = true;
            descend = false;
        }

    }

    private void DescendPhysics( )
    {
        myRigidbody2D.gravityScale = 1;
        Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, target, dropVelocity * Time.fixedDeltaTime);
            myRigidbody2D.MovePosition(newPos);
    }


    private void AscendPhysics()
    {
        myRigidbody2D.gravityScale = 0;
        myRigidbody2D.velocity = new Vector2(0, liftVelocity);
    }


     private void LightFlash()
    {
        if (lightLerp)
        {
            light2D.intensity = Mathf.Lerp(0, 3, stopTime);
        }
    }

    private void TouchingPlayer()
    {
        if (myCollider.IsTouchingLayers(LayerMask.GetMask("Player"))){
            myRigidbody2D.velocity = new Vector2(0, 0);
        }
    }


    private IEnumerator StartDownwardsKick()
    {
        if (!downwardsKick)
        {
            myAnimator.SetBool("JumpKickLift", false);
            lightLerp = true;
            myRigidbody2D.velocity = new Vector2(0, 0);
            target = new Vector2(player.transform.position.x, player.transform.position.y);
            yield return new WaitForSeconds(stopTime);
            DescendPhysics();
            myAnimator.SetBool("JumpKickDrop", true);
            descend = true;
            lightLerp = false;
            light2D.intensity = 0;
        }
    }



    private void TurnOffJumpKick()
    {
        if (myCollider.IsTouchingLayers(LayerMask.GetMask("Foreground")) || myCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
            {
            myAnimator.SetBool("JumpKickDrop", false);
        }
    }

    public void JumpKick()
    {
        myAnimator.SetBool("JumpKickLift", true);
       

    }
}
