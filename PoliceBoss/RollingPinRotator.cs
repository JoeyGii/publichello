using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RollingPinRotator : MonoBehaviour
{

    public event System.EventHandler LightLerpEvent;

    Rigidbody2D myRigidbody2D;
    Animator myAnimator;
    GameObject player;
    Light2D light2D;
    public PBDissappear dissappearScript;
  [SerializeField]  int attackPhase = 0;
    CapsuleCollider2D capsuleCollider;
    RollingPinParried parryScript;


    //new vars
    float movementSpeed = 6f;
    private bool attacking = false;
  [SerializeField]  private float stopTime = 1f;
    bool lightLerp = false;
    private Quaternion originalRotation;

    private void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        light2D = GetComponent<Light2D>();
        originalRotation = transform.rotation;
        parryScript = GetComponent<RollingPinParried>();


    }

    private void Start()
    {
        PoliceBossScript.OnRollingPin += Attack;
        RollingPinAttackBehaviour.PinAnimationOver += AnimationOver;
        player = GameObject.FindGameObjectWithTag("Player");
        LightLerpEvent += LightLerp;
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!dissappearScript.invisibility && !parryScript.commenceParryAttack)
        {
            switch (attackPhase)
            {
                case 0: IdlePosition(); break;
                case 1: FollowPlayer(); break;
                case 2: StartCoroutine(StopAndAttack()); break;
                default: IdlePosition(); break;
            } 

            if (lightLerp)
            {
                light2D.intensity = Mathf.Lerp(0, 3, stopTime);
            }
            if (dissappearScript.invisibility)
            {
                return;
            }
        } else { return; }
    }
    private void LightLerp(object sender, System.EventArgs e) => lightLerp = true;


   private void LookAtPlayer()
    {
   

            if (transform.position.x > player.transform.position.x)
            {
            transform.localScale = new Vector3(1, 1,1);

            }
            else if (transform.position.x < player.transform.position.x )
            {
            transform.localScale = new Vector3(-1, 1,1);
        }
        
    }


    private void IdlePosition()
    {
        Vector2 targetIdle = transform.parent.position;
        Vector2 newPosIdle = Vector2.MoveTowards(myRigidbody2D.position, targetIdle, (movementSpeed+5) * Time.fixedDeltaTime);
        myRigidbody2D.MovePosition(newPosIdle);
   //     transform.Rotate(rotValX, rotValy, 0);
    }

    private void FollowPlayer()
    {
        LookAtPlayer();
        transform.rotation = originalRotation;
        Vector2 targetPlayer = new Vector2(player.transform.position.x +0.85f, myRigidbody2D.position.y) ;
        Vector2 newPosPlayer = Vector2.MoveTowards(myRigidbody2D.position, targetPlayer, movementSpeed * Time.fixedDeltaTime);
        myRigidbody2D.MovePosition(newPosPlayer);
      
    }

 
    private void AnimationOver() =>   attackPhase = 0;
   

    private IEnumerator StopAndAttack()
    {
        if (attacking==true)
        {
            attacking = false;
            LightLerpEvent?.Invoke(this, System.EventArgs.Empty);
            myRigidbody2D.velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(stopTime);
            Animation();
        }
    }



    private IEnumerator StartFollowPlayer()
    {
        attackPhase= 1;
        yield return new WaitForSeconds(5);
        attackPhase=2;
        
    }

 private void Animation()
    {
        if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Rollingpinattack"))
        {
            lightLerp = false;
            light2D.intensity = 0f;
            myAnimator.SetTrigger("Attack");
        }
    }

    private void Attack()
    {
        if (!attacking)
        {
            attacking = true;
            StartCoroutine(StartFollowPlayer());
            
        }

    }

}
