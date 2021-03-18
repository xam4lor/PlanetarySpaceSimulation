using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour {
    [Header("Movement settings")]
    public float walkSpeed = 8f;
    public float runSpeed = 14f;
    public float jumpForce = 20f;
    public float vSmoothTime = 0.1f;
    public float airSmoothTime = 0.5f;
    public float stickToGroundForce = 8f;


    [Header("Character data")]
    public float mass = 70f;
	public Universe universe;
	public float gravitationalConstant = 0.0001f;
    public Transform feetPosition;

	private Rigidbody rb;
	private Vector3 velocity;
    private Vector3 smoothVRef;


    private void Awake() {
        this.initRigidbody();
    }

    private void initRigidbody() {
        this.rb = GetComponent<Rigidbody>();
        this.rb.interpolation = RigidbodyInterpolation.Interpolate;
        this.rb.useGravity = false;
        this.rb.isKinematic = false;
        this.rb.mass = this.mass;
    }


    private void Update() {
        HandleMovement();
    }



	private void FixedUpdate() {
		this.updatePhysics();

        // Move
        this.rb.MovePosition(this.rb.position + velocity * Time.fixedDeltaTime);
	}


    // Calculate forces and rotation and Update acceleration
    private void updatePhysics() {
        Vector3 acceleration = new Vector3(0, 0, 0);

        Body[] bodys = universe.getBodys();
        for (int i = 0; i < bodys.Length; i++) {
            // Gravity
            float sqrtDistance = (bodys[i].transform.position - this.rb.position).sqrMagnitude;
            Vector3 forceDir = (bodys[i].transform.position - this.rb.position).normalized;

            acceleration += forceDir * gravitationalConstant * bodys[i].mass / sqrtDistance;
        }

        // Global acceleration
        this.rb.AddForce(acceleration, ForceMode.Acceleration);
        Vector3 gravityUp = -acceleration.normalized;
        this.rb.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * this.rb.rotation;
    }




    private void HandleMovement() {
        if (Time.timeScale == 0) {
            return;
        }

        // Movement
        bool isGrounded = this.isGrounded();

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        bool running = Input.GetKey(KeyCode.LeftShift);
        Vector3 targetVelocity = transform.TransformDirection(input.normalized) * ((running) ? runSpeed : walkSpeed);
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothVRef, (isGrounded) ? vSmoothTime : airSmoothTime);


		// Is Grounded
        if (isGrounded) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                this.rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
                isGrounded = false;
            }
            else {
                // Apply small downward force to prevent player from bouncing when going down slopes
                this.rb.AddForce(-transform.up * stickToGroundForce, ForceMode.VelocityChange);
            }
        }
    }



    private bool isGrounded() {
        // Sphere must not overlay terrain at origin otherwise no collision will be detected
        // so rayRadius should not be larger than controller's capsule collider radius
        const float rayRadius = 0.3f;
        const float groundedRayDst = 0.2f;
        bool grounded = false;

        Vector3 relativeVelocity = this.GetComponent<Rigidbody>().velocity; //  - planet.velocity
		// Don't cast ray down if player is jumping up from surface
		if (relativeVelocity.y <= jumpForce * 0.5f) {
			RaycastHit hit;
			Vector3 offsetToFeet = (feetPosition.position - transform.position);
			Vector3 rayOrigin = rb.position + offsetToFeet + transform.up * rayRadius;
			Vector3 rayDir = -transform.up;

			grounded = Physics.SphereCast(rayOrigin, rayRadius, rayDir, out hit, groundedRayDst);
		}

        return grounded;
    }
}
