using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Rigidbody enemyRigidbody;
    public Transform targetTransform; // target object which is the player
    public float speed;
    public float jumpSpeed;
    public float jumpHeight;
    public float rotationSpeed;
    public float boostSpeed;
    private NavMeshAgent enemyAgent;
    float boostDuration;
    float normalSpeed;
    Vector3 jumpHeightAmount;
    Vector3 jumpTotal;
    Vector3 velocity;
    Vector3 displacementFromTarget;
	bool inGround;
    bool boostStatus;

	/*private void Awake() // used awake for navmesh agent
	{
		enemyAgent = GetComponent<NavMeshAgent>();
	}*/
	void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyRigidbody = GetComponent<Rigidbody>();
        jumpHeightAmount = new Vector3(0, jumpHeight, 0);
        normalSpeed = speed;
    }

    private void OnCollisionEnter(Collision checkCollision)
    {
        if(checkCollision.gameObject.CompareTag("Ground"))
        {
            inGround = true;
            enemyAgent.enabled = true; // navmeshagent is enabled when enemy is in ground.. this has autopathing
        }

        if (checkCollision.gameObject.CompareTag("Wall") && boostStatus) // this needs to be here to avoid this shit from bouncing all the time
        {
            enemyRigidbody.AddForce(-velocity, ForceMode.Impulse);
            Boost();
        }
            
    }

    private void OnTriggerEnter(Collider triggerBoost)
    {
        if (triggerBoost.tag == "Boost")
        {
            speed = boostSpeed;
            enemyAgent.speed = boostSpeed;
            boostStatus = true;
        }

    }

    void Boost()
    {
        speed = normalSpeed;
        enemyAgent.speed = normalSpeed;
        boostDuration = 0;
        boostStatus = false;
    }

    void RayDetectWalls () // this will be change into enemy detecting objects like walls
    {
        float raySize = 2.5f;
        //Vector3 localRayCastZ = new Vector3(0, 0, 2.5f);
        //float capsuleRad = 1f;
       
        bool detectWall = Physics.Raycast(enemyRigidbody.position, transform.forward, out RaycastHit wallDetected, raySize); // this is what raycast looks like
        //bool rayCastZ = Physics.Raycast(enemyRigidbody.position, localRayCastZ, out RaycastHit detectZ, raySize);

        //bool detectWall = Physics.CapsuleCast(enemyRigidbody.position, Vector3.forward, capsuleRad, Vector3.forward, out RaycastHit wallDetected);

        Debug.DrawRay(enemyRigidbody.position, transform.forward, Color.red); // dunno it doesn't work atleast there is a line xD

        if (detectWall) // check if true which is true anyway
        {
           if (wallDetected.collider.gameObject.layer == 6) // check if layer number is 6 since the aim is to avoid this wall and this use to be gameObject.tag == "Wall"
           {
                float velocityX = velocity.x; // take x value of velocuty and set it into velocityX
                float localX = transform.localPosition.x;
                float localZ = transform.localPosition.z;
                //float globalZ = transform.position.z;
                //float velocityZ = velocity.z;
                //velocityX *= -1; // multiply to -1 to change direction
                //velocityZ *= -1;
                wallDetected.distance = localZ + raySize;

                if(wallDetected.distance >= localZ)
                {
                    velocityX *= -1; // multiply to -1 to change direction
                    velocity = new Vector3(localX, 0, wallDetected.distance); // change velocity to local x and walldetected distance
                    enemyRigidbody.position += velocity * Time.fixedDeltaTime; // to move with the new instance of velocity

                    WhereToLook(velocity); // rotate towards the new velocity
                }

                /*velocity = new Vector3(localX, 0, 0); // create a new instance of velocity with the new value of velocity.x which is in velocityX variable

                enemyRigidbody.position += velocity * Time.fixedDeltaTime; // change position with the new velocity

                WhereToLook(velocity); // look direction with the new value of velocity*/
                print("Detected");
            }
            
        }
        
    }

    void WhereToLook (Vector3 lookDirection) // function for rotation -- face towards the direction where the target is moving
    {
        Quaternion whereToLook = Quaternion.LookRotation(lookDirection, Vector3.up);

        enemyRigidbody.rotation = Quaternion.RotateTowards(enemyRigidbody.rotation, whereToLook, rotationSpeed * Time.deltaTime);
    }

    void enemyAIMovement()
    {
        //enemyAgent.speed = speed;
        //enemyAgent.velocity = velocity;
        Vector3 targetWithIngoredY = new Vector3(targetTransform.position.x, 0, targetTransform.position.z);
        enemyAgent.SetDestination(targetWithIngoredY); // set agent destination to the player position

        if (boostStatus)
        {
            boostDuration += Time.deltaTime;
            if (boostDuration >= 1)
            {
                Boost();
            }
        }
        //enemyAgent.destination = targetTransform.position; // auto follow the player
    }

    // Update is called once per frame
    void Update()
    {
		displacementFromTarget = targetTransform.position - transform.position; // rigidbody is still enabled but not used
		Vector3 directionToTarget = displacementFromTarget.normalized; // this still works and calculates
		velocity = directionToTarget * speed; // still work
		jumpTotal = jumpHeightAmount * jumpSpeed; // needed for jumping

		/*float timer = 0;
        float raycastInterval = 1.5f;

        timer += Time.deltaTime;*/
		//RayDetectWalls(); // call this function

		// Vector3 moveAmount = velocity * Time.deltaTime;

		// float distanceToPlayer = displacementFromTarget.magnitude;

		/* if (distanceToPlayer > 1.5f)
         * {
         *      transform.Translate (moveAmount);
         * }
         */

	}

	/*Vector3 MoveDir ()
    {
        enemyRigidbody.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            Quaternion whereToLook = Quaternion.LookRotation(velocity, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, whereToLook, rotationSpeed * Time.deltaTime);
        }

        return enemyRigidbody.position;
    } */

	void FixedUpdate()
    {
        float distanceToPlayer = displacementFromTarget.magnitude; // distance of separation between objects
        //float distanceAI = enemyAgent.destination.magnitude;
        //float distanceAI = targetTransform.position.magnitude;
        float stopDistance = enemyAgent.stoppingDistance;

		enemyRigidbody.position += velocity * Time.deltaTime; // jump works when this is here... dunno why
        WhereToLook(enemyRigidbody.position);

		if (stopDistance > 0f /* distanceToPlayer > 1.5f */) // check if player is away 1.5f. If yes then start chasing else stay 1.5f away
        {
			
			enemyAIMovement(); // call this method where AI movement is 

            if(boostStatus)
            {
                boostDuration += Time.deltaTime;
                if(boostDuration >= 1)
                {
                    Boost();
                }
            }

			if (targetTransform.position.y > 0.1f && inGround && distanceToPlayer <= 3f) // check if player is above ground and is 1.5f away so it only jump at that distance
			{
				inGround = false;
				enemyAgent.enabled = false; // disable navmeshagent since it conflicts with physics

				if (enemyAgent.enabled == false && velocity != Vector3.zero) // if disabled then we use physics and rigidbody to jump
                {
					enemyRigidbody.AddForce(jumpTotal, ForceMode.Impulse); // jump mech 
					//WhereToLook(velocity);
				}
			}

		}


	}
}
