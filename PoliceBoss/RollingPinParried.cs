using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RollingPinParried : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;

    [SerializeField] private int parryDmgToBoss = 30;
   [SerializeField] private int rotations = 400;
    public Transform bossTransform;
    [SerializeField] int attackSpeed = 7;
    internal bool commenceParryAttack = false;
    private UnityAction ParryActions;
   HitPlayer parryEvent;
    Collider2D myCollider;
    public PBDissappear dissappearScript;
    // Start is called before the first frame update
    void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        parryEvent = GetComponent<HitPlayer>();
        myCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        parryEvent.ParryEfxEvent += AttackCommencer;
        ParryActions += MoveToBoss;
        ParryActions += Rotate;
        ParryActions += ChangeLayerToPlayer;
        ParryActions += TurnOnCollider;
    }

    // Update is called once per frame
    void Update()
    {
            if (commenceParryAttack && !dissappearScript.invisibility)
            {
                ParryActions();
            }
            else
            {
                return;
            }
    }

    private void TurnOnCollider() => myCollider.enabled = true;

    private void AttackCommencer(object sender, System.EventArgs e)
    {
        if (!dissappearScript.invisibility)
        {
            commenceParryAttack = true;
        }
    }

    private void MoveToBoss()
    {
        Vector2 target = bossTransform.position;
        Vector2 newPos = Vector2.MoveTowards(myRigidbody2D.position, target, attackSpeed* Time.deltaTime);
        myRigidbody2D.MovePosition(newPos);
    }

    private void Rotate() { transform.Rotate(0, 0, rotations * Time.deltaTime); }


    private void ChangeLayerToPlayer() => gameObject.layer = LayerMask.NameToLayer("Player");

    private void ChangeLayerToEnemy() => gameObject.layer = LayerMask.NameToLayer("Mob Attack");


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Mob") && commenceParryAttack)
        {
            Debug.Log("ATTACKA");
            collision.GetComponent<MobHealth>().TakeDamage(parryDmgToBoss);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            ChangeLayerToEnemy();
            myCollider.enabled = false;
            commenceParryAttack = false;
        }
    }
}
