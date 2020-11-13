using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBKickMovement : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;
    [SerializeField] private float kickmoveSpeed =8f;
 private  float xDirection = 1;
    [SerializeField] private float xPosition = 1;
    private float XPosition { get { return xPosition + PlayerDistance()/4; }  set { xPosition += PlayerDistance()/3; } }

    private void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }


    //private void Update()
    //{
    //    xDirection = transform.localScale.z >= 0 ? xDirection = -XPosition : xDirection = XPosition;
    //}


    protected float PlayerDistance()
    {
        Vector2 my2dPos = transform.position;

        Vector2 target2dPos = GameObject.FindWithTag("Player").transform.position;


        var distanceToTarget = Vector2.Distance(my2dPos, target2dPos);
        return distanceToTarget;
    }

    public void KickMovement()
    {
        xDirection = transform.localScale.z >= 0 ? xDirection = -XPosition : xDirection = XPosition;
        Vector2 targetPosition = new Vector2(myRigidbody2D.position.x + xDirection, myRigidbody2D.position.y);
        Vector2 newPosition = Vector2.MoveTowards(myRigidbody2D.position, targetPosition, kickmoveSpeed * Time.fixedDeltaTime);

        myRigidbody2D.MovePosition(targetPosition);
    }


    public void KickMoveStop()
    {
        myRigidbody2D.velocity = new Vector2(0, 0);
    }
}
