using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class PoliceDissapearBoss : MonoBehaviour
{

    public event System.EventHandler PinAttackNow;
    //delegates
    public delegate void PowderEvent();
    public static event PowderEvent OnPowder;
    private UnityAction InvisibleFunctions;
    private UnityAction VisibilityFunctions;
    public Transform [] wayPoints;
    internal Transform wayPoint, wayPoint2;

    //vars
    internal bool invisibility = false;
    public bool powderAttacking { get; private set; } = false;
    private float timeToGoInvisible = 2f;
    float bossSpeed;
    float fade = 1;
    bool wayPointCreated = false;
    bool wayPointReached = false;
  [SerializeField]  int wayPointTally = 0;



    //cache
    Light2D lightComponent;
    Rigidbody2D myRigidbody2D;
    Animator myAnimator;
    SpriteRenderer spriteR;
    Transform player;
    public GameObject rollingPin;
    Material mat;



    //frames
    void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        lightComponent = GetComponentInChildren<Light2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        bossSpeed = GetComponent<PoliceBossScript>().bossSpeed;
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        mat = spriteR.material;

        //invisibility functions
        InvisibleFunctions += CollisionsTurnedOff;
        InvisibleFunctions += MoveToWaypoint;
       // InvisibleFunctions += TurnOffRollingPin;
       InvisibleFunctions += ShaderFadeOut;

        //visibility functions
        VisibilityFunctions += CollisionsTurnedOn;
       // VisibilityFunctions += TurnOnRollingPin;

      
    }

    void Update()
    {
        WayPointCreation();
        if (invisibility == true)
        {
            InvisibleFunctions();
        } else
        {
            VisibilityFunctions();
        }
    }


    //methods
    public void PowderFunction()
    {
        if (powderAttacking == false)
        {
            powderAttacking = true;
            myAnimator.SetTrigger("PowderDisappear");
            StartCoroutine(GoInvisible());
            if (OnPowder != null)
            {
                OnPowder();
            }
        }

    }

    private void WayPointCreation()
    {
      
        if (!wayPointCreated)
        {
            wayPoint = wayPoints[UnityEngine.Random.Range(0, wayPoints.Length)];
            wayPointCreated = true;
        }
    }

    //coroutine go visible needs to go into a method that happens after each waypoint has reached;
    //if the rolling pin attack is parried in invibisibilty knock boss out of invisibility


    private void PowderLights()
    {

        lightComponent = GetComponentInChildren<Light2D>();
        lightComponent.intensity = Mathf.Lerp(2f, 0f, myAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void FollowPlayer()
    {

            Vector2 target = new Vector2(player.position.x, myRigidbody2D.position.y);
            Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, target, bossSpeed * Time.fixedDeltaTime);
            myRigidbody2D.MovePosition(newPos);
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ForegroundObstacle"))
        {
            StartCoroutine(MoveToNextWayPoint());
            PinAttackNow?.Invoke(this, System.EventArgs.Empty);
        }
    }

    private void MoveToWaypoint()
    {
        if (wayPointTally == 0)
        {
            Vector2 target = new Vector2(wayPoint.position.x, myRigidbody2D.position.y);
            Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, target, bossSpeed * Time.fixedDeltaTime);
            myRigidbody2D.MovePosition(newPos);   
        } else if(wayPointTally == 1)
        {
            wayPoint2 = wayPoint == wayPoints[0] ? wayPoints[1] : wayPoints[0];
            Vector2 target = new Vector2(wayPoint2.position.x, myRigidbody2D.position.y);
            Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, target, bossSpeed * Time.fixedDeltaTime);
            myRigidbody2D.MovePosition(newPos);
        } else if(wayPointTally == 2)
        {
            FollowPlayer();
            GoVisible();
        }
    }

    private IEnumerator MoveToNextWayPoint()
    {
       
        yield return new WaitForSeconds(4f);
        wayPointTally++;

    }

    protected void TurnOffRollingPin() => rollingPin.SetActive(false);


    protected void TurnOnRollingPin() => rollingPin.SetActive(true);

    protected void CollisionsTurnedOff() => Physics2D.IgnoreLayerCollision(9, 10, true);

    protected void CollisionsTurnedOn() => Physics2D.IgnoreLayerCollision(9, 10, false);

    protected void ShaderFadeOut()
    {
        fade -= Time.deltaTime;
        mat.SetFloat("_Fade", fade);
    }

    protected void ShaderFadeIn()
    {
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


    private IEnumerator GoVisible()
    {
       
        yield return new WaitForSeconds(Random.Range(5, 10f));
        ShaderFadeIn();
        powderAttacking = false;
        invisibility = false;
    }
}
