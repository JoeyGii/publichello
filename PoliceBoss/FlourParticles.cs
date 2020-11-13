using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlourParticles : MonoBehaviour
{
   public PoliceBossFSM fsmScript;
    ParticleSystem particles;
    public PBDissappear powderScript;
    public GameObject flourParticleDispersionPrefab;
    Animator myAnimator;
    CapsuleCollider2D myCollider;

    int mobAttackLayer = 16;
    int fryingPanLayer = 17;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        myAnimator = GetComponentInParent<Animator>();
        myCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {

        if (powderScript.invisibility)
        {
            myCollider.enabled = false;
        } 
    }



    private void TurnOffCollisions() => Physics2D.IgnoreLayerCollision(mobAttackLayer, fryingPanLayer, true);

    private void TurnOnCollisions() => Physics2D.IgnoreLayerCollision(mobAttackLayer, fryingPanLayer, false);




    internal IEnumerator FlourDispersion()
    {
        myAnimator.SetBool("FlourExplosion", true);
        TurnOffCollisions();
        //Instantiate(flourParticleDispersionPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(7f);
        TurnOnCollisions();
        myAnimator.SetBool("FlourExplosion", false);
    }
}
