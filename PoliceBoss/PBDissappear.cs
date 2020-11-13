using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class PBDissappear : MonoBehaviour
{

    public event System.EventHandler PinAttackNow;
    //delegates
    public delegate void PowderEvent();
    public static event PowderEvent OnPowder;
    private UnityAction InvisibleFunctions;
    private UnityAction VisibilityFunctions;

    //vars
    internal bool invisibility = false;
    public bool powderAttacking { get; private set; } = false;
    private float timeToGoInvisible = 2f;
    float bossSpeed;
    float fade = 1;




    //cache
    Light2D lightComponent;
    Rigidbody2D myRigidbody2D;
    Animator myAnimator;
    SpriteRenderer spriteR;
    Material mat;
    //frames
    void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        lightComponent = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        bossSpeed = GetComponent<PoliceBossScript>().bossSpeed;
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        mat = spriteR.material;

        //invisibility functions
        InvisibleFunctions += CollisionsTurnedOff;
        InvisibleFunctions += ShaderFadeOut;

        //visibility functions
        VisibilityFunctions += CollisionsTurnedOn;
        VisibilityFunctions += GoVisible;


    }

    void Update()
    {

        if (invisibility == true)
        {
            InvisibleFunctions();
        }
        else
        {
            VisibilityFunctions();
        }
    }


    //methods
    public void PowderFunction()
    {
        if (powderAttacking == false)
        {
            invisibility = true;
            powderAttacking = true;
            myAnimator.SetTrigger("PowderDisappear");
            StartCoroutine(GoInvisible());
            if (OnPowder != null)
            {
                OnPowder();
            }
        }

    }
    //coroutine go visible needs to go into a method that happens after each waypoint has reached;
    //if the rolling pin attack is parried in invibisibilty knock boss out of invisibility


    private void PowderLights()
    {

        lightComponent = GetComponentInChildren<Light2D>();
        lightComponent.intensity = Mathf.Lerp(2f, 0f, myAnimator.GetCurrentAnimatorStateInfo(0).length);
    }



    protected void CollisionsTurnedOff() => gameObject.layer = LayerMask.NameToLayer("Particles");

    protected void CollisionsTurnedOn() => gameObject.layer = LayerMask.NameToLayer("Enemies");

    protected void ShaderFadeOut()
    {
        fade -= Time.deltaTime;
        mat.SetFloat("_Fade", fade);
    }

    protected void ShaderFadeIn()
    {
       if(powderAttacking)
            mat.SetFloat("_Fade", 1);
        
    }

    //coroutines
    private IEnumerator GoInvisible()
    {
        invisibility = true;
        myRigidbody2D.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(timeToGoInvisible);
        lightComponent.intensity = 0;
    }


    private void GoVisible()
    {
        ShaderFadeIn();
        powderAttacking = false;
     
    }
}