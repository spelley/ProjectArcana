using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterMotor : MonoBehaviour 
{
    bool _locked;
    public bool locked {
        get
        {
            return _locked;
        }
        set
        {
            if(value)
            {
                anim.SetBool("Moving", false);
            }
            
            _locked = value;
        }
    }

    [SerializeField]
    bool autoInitializeData = false;

    // Variables
    public float turnSpeed = .1f; 
    public float moveSpeed = 12f;
    public float jumpSpeed = 1f;
    public float gravity = 9.8f;
    bool jump;
    float verticalVelocity;
    float groundedTimer; 
    public UnitData unitData;
    Animator anim; // Reference to the animator component.
    Camera cam;
    CharacterController characterController;
    
    
    float horizontal = 0f;
    float vertical = 0f;

    // Functions
    void Awake()
    {
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;

        if(unitData && autoInitializeData)
        {
            unitData = Instantiate(unitData);
            unitData.Load();
            unitData.unitGO = this.gameObject;
        }
    }

    void Start()
    {
        
    }

    void OnDestroy()
    {
        
    }

    void Update()
    {
        if(locked || gameObject.tag != "Player")
        {
            horizontal = 0;
            vertical = 0;
            jump = false;
            verticalVelocity = 0f;
            return;
        }

        // Cache the inputs.
        /**
        if(Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        **/
    }

    void FixedUpdate()
    {
        if(locked || gameObject.tag != "Player")
        {
            return;
        }
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        MovementManagement(horizontal, vertical);
    }

/////////////////////////////////////////////CHARACTER MOVEMENT/////////////////////////////////////////

    void MovementManagement (float horizontal, float vertical)
    {
        // If there is some axis input...
        if(horizontal != 0f || vertical != 0f)
        {
            // ... set the players rotation and set the speed parameter to 5.3f.
            //Rotate(horizontal, vertical);
            MoveInDirectionOfInput(horizontal, vertical);
            anim.SetBool("Moving", true);
            
        }
        else {
            MoveInDirectionOfInput(horizontal, vertical);
            anim.SetBool("Moving", false);
        }
    }

    public void MoveInDirectionOfInput(float horizontal, float vertical) {
        bool groundedPlayer = characterController.isGrounded;
        if (groundedPlayer)
        {
            // cooldown interval to allow reliable jumping even whem coming down ramps
            groundedTimer = 0.1f;
        }
        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }
 
        // slam into the ground
        if (groundedPlayer && verticalVelocity < 0)
        {
            // hit ground
            verticalVelocity = 0f;
        }

        // apply gravity always, to let us track down ramps properly
        verticalVelocity -= gravity * Time.deltaTime;

        Vector3 dir = new Vector3(horizontal, 0, vertical).normalized * moveSpeed;
        Vector3 camDirection = cam.transform.rotation * dir; //This takes all 3 axes (good for something flying in 3d space)    
        Vector3 targetDirection = new Vector3(camDirection.x, 0, camDirection.z);
            
        if (dir != Vector3.zero) { //turn the character to face the direction of travel when there is input
            transform.rotation = horizontal == 0f && vertical == 0f ? transform.rotation : Quaternion.LookRotation(targetDirection);
        }

        if(jump)
        {
            // must have been grounded recently to allow jump
            if (groundedTimer > 0)
            {
                // no more until we recontact ground
                groundedTimer = 0;
 
                // Physics dynamics formula for calculating jump up velocity based on height and gravity
                verticalVelocity += Mathf.Sqrt(jumpSpeed * 2 * gravity);
            }
            jump = false;
        }

        targetDirection.y = verticalVelocity;

        characterController.Move(targetDirection * Time.deltaTime);
    }
}