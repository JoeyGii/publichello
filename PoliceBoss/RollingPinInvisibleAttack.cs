using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

internal class RollingPinInvisibleAttack : MonoBehaviour
{
    public Transform[] wayPoints;
    public PBDissappear dissappearScript;
   [SerializeField] float attackSpeed = 10;
    SpriteRenderer sprRend;
    Rigidbody2D myRigidbody2D;
    Vector2 attackTarget;
    Vector2 moveTarget;
    RollingPinParried parryScript;
  [SerializeField]  int wayPointTally = 0;
    CapsuleCollider2D capsuleCollider;
    ParticleSystem pinParticles;

    public event System.EventHandler EndInvisibilityEvent;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    private void Start()
    {
        pinParticles = GetComponentInChildren<ParticleSystem>();
        sprRend = GetComponent<SpriteRenderer>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        moveTarget = wayPoints[Random.Range(0, wayPoints.Length)].position;
    }

    private void Update()
    {
        if (dissappearScript.invisibility)
        {
            switch (wayPointTally)
            {
                case 0: RollingInvisibleFindWayPoint(); break;
                case 1: RollingPinInvisibleAttackMovement(); ; break;
                case 2: RollingInvisibleFindWayPoint(); break;
                case 3: RollingPinInvisibleAttackMovement(); ; break;
                case 4: EndInvisibility(); ; break;
            }
        }
    }

    private void EndInvisibility()
    {
        dissappearScript.invisibility = false;
        CollisionsTurnedOn();
        wayPointTally = 0;
        EndInvisibilityEvent?.Invoke(this, System.EventArgs.Empty);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ForegroundObstacle"))
        {
            StartCoroutine(MoveToNextWayPoint());
        }
    }

    private IEnumerator MoveToNextWayPoint()
    {
        yield return new WaitForSeconds(2f);
        wayPointTally++;

    }
    private void CollisionsTurnedOff() => Physics2D.IgnoreLayerCollision(9, 16, true);

    private void CollisionsTurnedOn() => Physics2D.IgnoreLayerCollision(9, 16, false);


    public void RollingInvisibleFindWayPoint()
    {
        sprRend.enabled = false;
        pinParticles.Stop();
        CollisionsTurnedOff();
            Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, moveTarget, attackSpeed * Time.fixedDeltaTime);
            myRigidbody2D.MovePosition(newPos);

    }

    public void RollingPinInvisibleAttackMovement()
    {
        pinParticles.Play();
        CollisionsTurnedOn();
        sprRend.enabled = true;
        attackTarget = moveTarget !=  new Vector2(wayPoints[0].position.x,wayPoints[0].position.y) ? wayPoints[0].position : wayPoints[1].position;
        Vector2 target = attackTarget;
        Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, target, attackSpeed * Time.fixedDeltaTime);
        myRigidbody2D.MovePosition(newPos);

    }
}
