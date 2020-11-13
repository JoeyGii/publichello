using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Events;
using System;


public class PoliceBossScript : BossBase
{

    //vars
    public float bossSpeed = 5f;
    private bool attacking;
    

    //delegates
    public delegate void RollingPinEvent();
    public static event RollingPinEvent OnRollingPin;


    //loot
    public GameObject lunchBox, spawnPoint;

    //cache
    Vector2 dodgeVelocity;
    public Transform positionProtector;
    protected MobHealth healthScript;





    //frames
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<MobHealth>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
     CurrentlyAttacking();
        PlayerDistance();
        
    }





    //methods////////////////////////////////////
    protected void Die()
    {
        myAnimator.SetBool("Die", true);
        Instantiate(lunchBox, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }


    public void PositionStabler()
    {
       
            transform.position = positionProtector.position;
     
    }

    protected void Kick()
    {
        myAnimator.SetBool("Run", false);
        myAnimator.SetTrigger("PBKick");

    }

 

   

    protected void RollingPinAttack()
    {
        OnRollingPin?.Invoke();
    }

    protected void Idle()
    {
        myAnimator.SetBool("Idle", true);
        myAnimator.SetBool("Run", false);
    }

    protected void Dodge()
    {
        myRigidbody2D.velocity = -dodgeVelocity;
        myAnimator.SetTrigger("ProjectileDodge");
    }

    protected void Run1()
    {
            myAnimator.SetBool("Run", true);
       
      if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
           Vector2 target = new Vector2(player.position.x, myRigidbody2D.position.y);
            Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, target, bossSpeed * Time.fixedDeltaTime);
            myRigidbody2D.MovePosition(newPos);
        } 
    }


    protected bool CurrentlyAttacking()
    {
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            attacking = true;
        }
        else
        {
            attacking = false;
        }
        return attacking;
    }

}
