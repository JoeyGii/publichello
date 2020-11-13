using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoliceBossFSM : PoliceBossScript
{
    [Header("Attack Timers")]
    [SerializeField] private float powderTimer = 5f;
    [SerializeField] private float powderTimerReset = 30f;

    [SerializeField] private float jumpTimer = 6;
    [SerializeField] private float jumpTimerReset= 5f;

    [SerializeField] private float flourDispersionTimer = 5f;
    [SerializeField] private float flourDispersionTimerReset =5f;

    [SerializeField] private float kickTimer = 0;
    [SerializeField] private float kickTimerReset = 2;

    PBDissappear DissappearScript;
    PBossJumpKickBehaviour jumpKickScript;
    internal int phase;
    int originalHealth;
    FlourParticles particleScript;
    [Header("Attack Ranges")]
    [SerializeField] public float kickRange = 1f;
    [SerializeField] private float rollingPinRange = 5f;
    
    private bool aiTrigger;

    public enum PoliceFSMState
    {
        Idle,
        Run,
        RollingPinAttack,
        Kick,
        Dodge,
        PowderDisappear,
        FlourDispersion,
        JumpKick,
        Die,
    }


    public PoliceFSMState currentState;


    // awake
    private void Awake()
    {
        healthScript = GetComponent<MobHealth>();
        visionScript = GetComponentInChildren<VisionEnemy>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        myAnimator = GetComponent<Animator>();
        DissappearScript = GetComponent<PBDissappear>();
        jumpKickScript = GetComponent<PBossJumpKickBehaviour>();
        
    }

    private void Start()
    {
        particleScript = GetComponentInChildren<FlourParticles>();
        originalHealth = healthScript.health;
        visionScript.OnSight += AiInstantiation;
        aiTrigger = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AttackTimers();
        CurrentPhase();
            ChangeStateOne();
     if (!CurrentlyAttacking())
            {
                LookAtPlayer();
            }
        StateSwitcher();
    }

    private void AttackTimers()
    {
        powderTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
        if(PlayerDistance() < 5)
        {
            flourDispersionTimer -= Time.deltaTime;
        }
    }

    private void AiInstantiation(object sender, EventArgs e)
    {

        if (aiTrigger == false)
        {
            aiTrigger = true;
        }
      
    }




    private void StateSwitcher()
    {
        switch (currentState)
        {
            case PoliceFSMState.Idle: Idle(); break;
            case PoliceFSMState.Run: Run1(); break;
            case PoliceFSMState.RollingPinAttack: RollingPinAttack(); break;
            case PoliceFSMState.Kick: Kick(); break;
            case PoliceFSMState.Dodge: Dodge(); break;
            case PoliceFSMState.PowderDisappear: DissappearScript.PowderFunction(); break;
            case PoliceFSMState.FlourDispersion: StartCoroutine(particleScript.FlourDispersion()); break;
            case PoliceFSMState.JumpKick: jumpKickScript.JumpKick(); break;
            case PoliceFSMState.Die: Die(); break;
        }
    }


    internal int CurrentPhase()=> phase = healthScript.health > originalHealth /2 ? phase = 1 : phase = 2;
    


    protected void ChangeStateOne()
    {
        if (aiTrigger == true && !DissappearScript.invisibility)
        {
            //powderdisappear
            if (powderTimer <= 0)
            {
                currentState = PoliceFSMState.PowderDisappear;
                powderTimer = powderTimerReset;
            }
            // jump kick
            else if (jumpTimer <=0 && !DissappearScript.invisibility && PlayerDistance()> kickRange)
            {
                jumpTimerReset = UnityEngine.Random.Range(jumpTimerReset, (jumpTimerReset+5));
                currentState = PoliceFSMState.JumpKick;
                jumpTimer = jumpTimerReset;
            }
          
            //run
            else if (PlayerDistance() > kickRange)
            {
                currentState = PoliceFSMState.Run;
            }
            //kick
            else if (kickTimer <= 0 && PlayerDistance() < kickRange && DissappearScript.powderAttacking == false)
            {
                currentState = PoliceFSMState.Kick;
                kickTimer = kickTimerReset;
            }
            //flour dispersion
            else if (flourDispersionTimer <= 0 && DissappearScript.powderAttacking == false)
            {
                currentState = PoliceFSMState.FlourDispersion;
                flourDispersionTimer = flourDispersionTimerReset;
            }
           
            // rolling pin attack
            else if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("FlourExplosion"))
            {
                currentState = PoliceFSMState.RollingPinAttack;
                //PlayerDistance() < rollingPinRange && rollingTimer <= 0 && DissappearScript.powderAttacking == false
            }

            //idle
            else if (PlayerDistance()<kickRange)
            {
                currentState = PoliceFSMState.Idle;
                //kickTimer > 0 && PlayerDistance() < kickRange && rollingTimer > 0
            }
            //die
            else if (healthScript.health <= 0)
            {
                currentState = PoliceFSMState.Die;
            }
        }
    }
}
