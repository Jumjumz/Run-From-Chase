using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody playerRigidbody;
    public Transform cam;
    public float speed;
    public float jumpSpeed;
    public float jumpHeight;
    public float rotationSpeed;
    public float boostSpeed;
	float normalSpeed;
    float boostDuration;
    Vector3 jumpHeightAmount;
    Vector3 velocity;
    Vector3 input;
    Vector3 slopeMoveDirection;
	bool inGround;
    bool boostStatus;
	RaycastHit groundDetected;
	// int coinCount;

	private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        jumpHeightAmount = new Vector3(0, jumpHeight, 0); // new vector for jumping where jumpHeight is the y value
        boostStatus = false; // set status of player is boosting
        boostDuration = 0; // duration of boost
        normalSpeed = speed; // initialize the normal speed into the original speed

    }

    private void OnCollisionEnter(Collision checkCollision) // check collision for the ground to avoid double jumping issue
    {
        if(checkCollision.gameObject.CompareTag("Ground")) // collision requires gameObject and comparetag to locate tags
        {
            inGround = true; // set inGround to true
        }

        if (checkCollision.gameObject.CompareTag("Wall") && boostStatus) // check if we hit Wall and we are currently boosting
        {
            playerRigidbody.AddForce(-velocity, ForceMode.Impulse); // add a bounce effect when player hits the wall
            Boost(); // execute this function
        }
    }

    private void OnTriggerEnter(Collider triggerBoost) // collider function. This is what it looks all the time except triggerBoost this shit is made up
    {
        if (triggerBoost.tag.StartsWith("Boost")) // check the tag of the collider (make sure you create a tag first before doing this shit)
        {
            speed = boostSpeed; // assign speed value to boostSpeed in order to change the velocity
            boostStatus = true; // set true as we are boosting 
        }
    }

    void Boost() // upgraded since I dont want to repeat same code. POG move just call this function to execute whatever the condition is
    {
        speed = normalSpeed;// return the speed to its original value which was assigned to normalSpeed                       
        boostDuration = 0; // set to 0 to restart the value
        boostStatus = false; // set to false as we are not boosting and to not have continouos boosting in game
    }

    void DownRayCast()
    {
        bool detectGround = Physics.Raycast(transform.position, Vector3.down, out groundDetected);

        if(detectGround)
        {
            Quaternion targetLook = Quaternion.FromToRotation(Vector3.up, groundDetected.normal);
            //Quaternion rotateHere = Quaternion.LookRotation(velocity, groundDetected.normal);
            //Quaternion lookRamp = Quaternion.Slerp(transform.rotation, targetLook, Time.deltaTime * 5f);
            print(groundDetected.normal);

            if(groundDetected.normal != Vector3.up)
            {
                //transform.rotation = Quaternion.RotateTowards(rotateHere, targetLook, rotationSpeed * Time.deltaTime);
                WhereToLook(targetLook);
			}	
 
        }

    }

	void WhereToLook(Quaternion lookTowards) // separated a method for look rotation
	{
		//Quaternion toRotation = Quaternion.LookRotation(velocity, Vector3.up); // remember! as this is the code to rotate

		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookTowards, rotationSpeed * Time.deltaTime); // rotate towards the button press
	}

    void moveDirection() // created a method for movement direction
    {
		Quaternion toRotation = Quaternion.LookRotation(velocity, groundDetected.normal);// remember! as this is the code to rotate

		WhereToLook(toRotation); // call the method
	}

	// Update is called once per frame
	void Update()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		input = Quaternion.AngleAxis(cam.rotation.eulerAngles.y, Vector3.up) * input; // change the input to mouse
		velocity = input * speed;
        Vector3 jumpTotal = jumpHeightAmount * jumpSpeed;
        // Vector3 moveAmount = velocity * Time.deltaTime; // movement not in update instead in fixupdate

        // transform.position += moveAmount;
        // transform.Translate(moveAmount); // the same for movement as the above code

        if (Input.GetKeyDown(KeyCode.Space) && inGround) // check if inGround is colliding with something
        {
            playerRigidbody.AddForce(jumpTotal, ForceMode.Impulse); // to jump 
            inGround = false; // make this variable false since it is not in ground anymore
        }

       DownRayCast();
    }


    void FixedUpdate()
    {
        playerRigidbody.position += velocity * Time.deltaTime; // probably the correct way of movement since it has collission


        // condition for moving towards certain angle of directions
        if (groundDetected.normal == Vector3.up) // used to be velocity != Vector3.zero
        {
            moveDirection(); //call the method
        }

        if (boostStatus) // check boostStatus wether true or false
        {
            boostDuration += Time.deltaTime; // set duration to deltatime
            if (boostDuration >= 1) // boostDuration needs to count from 0 to 1 only as boost only last 1 sec after that we stop boosting
            {
                Boost(); // execute this function
            }
        }

    }

	private void OnApplicationFocus(bool focus) // to track cursor movement
	{
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked; // if focus lock the cam
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // needs to be here to update the state
        }
    }


	/* private void OnTriggerEnter(Collider triggerCollider) // for detecting non colliding objects like coin collecting games
    * {
    *     if (triggerCollider.tag == "Coin")
    *    {
    *       Destroy (triggerCollider.gameObject);
    *       coinCount++;
    *    } 
    } */

}
