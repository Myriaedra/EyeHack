using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Chara_PlayerController : Character {

	Camera cam;
    public  CinemachineFreeLook freeLookCM;
	Rigidbody rb;
    public static bool controlsAble = true;
	public static bool isAiming = false;
    public LayerMask groundLayer;
    public static bool bumped;

    [Header("GroundValues : ")]
    public float groundAcceleration;
    public float groundMaxSpeed;
    public float groundDragValue;

    [Header("AirValues : ")]
    public float airAcceleration;
    public float airMaxSpeed;
    public float airDragValue;

    [Header("OtherValues : ")]
    public float jumpForce;
    public float bufferValue;

    bool landed = false;
    
    float buffer;


	// Use this for initialization
	void Start ()
	{
		cam = Camera.main;
		rb = GetComponent<Rigidbody>();
	}

	void Update()//--------------------------------------------------------------------------------------------------------------------------
	{
        if (Input.GetButtonDown("Jump") && buffer <= 0f)
        {
            buffer = bufferValue;
        }
        else if (buffer > 0f)
        {
            buffer -= Time.deltaTime;
        }
        //JUMP MANAGEMENT
        if (Input.GetButton("Jump") && landed && controlsAble)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
		//print (rb.velocity.y);
	}

	void FixedUpdate ()//-------------------------------------------------------------------------------------------------------------------
	{
        if (controlsAble)
        {
            if (IsGrounded())//MOUVEMENT AU SOL---------------------------------------------------
            {
                GroundMovement();
            }
            else//MOUVEMENT AERIEN---------------------------------------------------------------------
            {
                AirMovement();
            }
        }
        else
            rb.velocity = Vector3.zero;
	}

	void Drag(float drag){
		Vector3 vel = rb.velocity;
		vel.x *= 1 - drag;
		vel.z *= 1 - drag;
		rb.velocity = vel;
	}

	void GroundMovement() //FONCTION MVT SOL
	{
			Move(groundAcceleration);
			LimitVelocity(groundMaxSpeed);
			OrientCharacter(0.2f);
            Drag(groundDragValue);
	}

	void AirMovement()//FONCTION MVT AIR
	{
            Move(airAcceleration);
            GravityMod(3.0f);

            if (!bumped)
                LimitVelocity(airMaxSpeed);

            OrientCharacter(0.05f);
            Drag(airDragValue);
	}

	void LimitVelocity(float speedLimit)//FONCTION MAXSPEED
	{
		Vector2 xzVel = new Vector2(rb.velocity.x, rb.velocity.z);
		if (xzVel.magnitude > speedLimit)
		{
			xzVel = xzVel.normalized * speedLimit;
			rb.velocity = new Vector3(xzVel.x, rb.velocity.y, xzVel.y);
		}
	}

	void OrientCharacter(float lerpValue)//FONCTION ORIENTATION PERSO
	{
		if (!isAiming) 
		{
			//Check joystick input
			float xSpeed = Input.GetAxis ("Horizontal");
			float zSpeed = Input.GetAxis ("Vertical");
			Vector3 velocityAxis = new Vector3 (xSpeed, 0, zSpeed);
			//Convert direction depending on the camera
			velocityAxis = Quaternion.AngleAxis (cam.transform.eulerAngles.y, Vector3.up) * velocityAxis;
			//Actual velocity
			Vector3 hVelocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);

			//If the character is moving
			if (hVelocity.magnitude > 1 && velocityAxis.magnitude > 0) {
				//Vector3 newVelocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
				float wantedAngle = transform.eulerAngles.y; //Declare
				//Checks which side to turn (shortest way)
				float angle = Vector3.Angle (transform.forward, velocityAxis.normalized);
				Vector3 cross = Vector3.Cross (transform.forward, velocityAxis.normalized);
				if (cross.y < 0)
					angle = -angle;
				wantedAngle = transform.eulerAngles.y + angle;
				//Rotate SMOOTHLY to the right direction
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, wantedAngle, 0), lerpValue);
			}
		}
	}

	void GravityMod(float mod)//FONCTION EXTRA/LESS GRAVITY
	{
		Vector3 GravityForce = (Physics.gravity * mod) - Physics.gravity;
		rb.AddForce(GravityForce);
	}

	void Move(float acceleration)//BASIC MOVEMENT (USED IN OTHER FUNCTIONS)
	{
		float xSpeed = Input.GetAxis("Horizontal");
		float zSpeed = Input.GetAxis("Vertical");
		Vector3 velocityAxis = new Vector3(xSpeed, 0, zSpeed);
		velocityAxis = Quaternion.AngleAxis(cam.transform.eulerAngles.y, Vector3.up) * velocityAxis;
        rb.AddForce(velocityAxis * acceleration);
    }

	bool IsGrounded()//VERIFIER SI ON EST AU SOL
	{
        RaycastHit hit;
       	Vector3 position1 = transform.position - transform.forward/10;

        //Raycasts !
        if (/*Physics.Raycast(position1, -transform.up, 1.2f, groundLayer) 
        || Physics.Raycast(position2, -transform.up, 1.2f, groundLayer) 
        || Physics.Raycast(position3, -transform.up, 1.2f, groundLayer)*/
        Physics.SphereCast(position1, 0.5f, -transform.up, out hit, 1.2f, groundLayer))
		{
            if (!landed)
			{
				landed = true;
                if (bumped)
                    bumped = false;
				//rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				//rb.velocity = new Vector3(rb.velocity.x/4, 0, rb.velocity.z/4); //Avoid slipping
			}     
			return true;
		}
		else
		{
            if (landed){
				landed = false;
			}
			return false;
		}
	}

	public override void DeathCall ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
