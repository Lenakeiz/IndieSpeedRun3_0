using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    //[RequireComponent(typeof (CapsuleCollider))]
	public class RigidbodyFirstPersonController : ReshrikingEntity
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
	        public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

            private bool m_Running;
			public enum MovementEnum{Idle, Strafe, Backwards, Forwards}
			public MovementEnum currEnum = MovementEnum.Idle;

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					currEnum = MovementEnum.Strafe;
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					currEnum = MovementEnum.Backwards;
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					currEnum = MovementEnum.Forwards;
					CurrentTargetSpeed = ForwardSpeed;
				}

				if (Input.GetKey(RunKey))
	            {
		            CurrentTargetSpeed *= RunMultiplier;
		            m_Running = true;
	            }
	            else
	            {
		            m_Running = false;
	            }
            }

            public bool Running
            {
                get { return m_Running; }
            }
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air

			public float massStepDecrease = 0.52f;

			[Range(0,1)]
			public float cobwebMultiplier = 0.5f;

		}

        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded, m_IscobWeb;

		private GunController m_gunController;
		private Animator m_animator;
		private bool m_isMine;

		public bool Ownership {
			get {return m_isMine;}
			set{m_isMine = value;}
		}

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
				return movementSettings.Running;
            }
        }

		public override void Reshrink(){
			base.Reshrink();
			UpdateParameters();
		}

		private void UpdateParameters(){
			m_Jump = false;
			m_Jumping = true;
			if(m_RigidBody.mass - advancedSettings.massStepDecrease > 0)
				m_RigidBody.mass -= advancedSettings.massStepDecrease;
		}

		public override void Awake()
        {
			base.Awake();
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
			m_gunController = GetComponent<GunController>();
			m_animator = GetComponent<Animator>();
            mouseLook.Init (transform, cam.transform);
			OnDeath += HandleDeath;
			m_isMine = false;

		}

		private void HandleDeath()
		{
			if (m_isMine) {
				cam.enabled = false;
				cam.GetComponent<AudioListener> ().enabled = false;
				GameObject deathCam = GameObject.FindGameObjectWithTag ("DeadCamera");
				deathCam.GetComponent<Camera> ().enabled = true;
				deathCam.GetComponent<AudioListener> ().enabled = true;

				GameObject.Find ("GlobalScripts").GetComponent<NetworkManager> ().OnPlayerDeath (PhotonNetwork.playerName);
				PhotonNetwork.Destroy (this.gameObject);
			}
		}

        public override void Update()
        {
			base.Update();

			if(!isDead){

				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				if(m_isMine)
				{

	            	RotateView();

								
					if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
					{
						m_animator.SetBool("isJumping", true);
						m_Jump = true;
					}
					
					if(Input.GetMouseButton(0))
					{
						m_animator.SetTrigger("shoot");
						m_gunController.Shoot(multiplier);
					}
				}
			}
        }


        private void FixedUpdate()
        {
			if (!isDead) {
				GroundCheck ();
				Vector2 input = m_isMine ? GetInput () : Vector2.zero;

				if ((Mathf.Abs (input.x) > float.Epsilon || Mathf.Abs (input.y) > float.Epsilon) && (m_IsGrounded)) {
					// always move along the camera forward as it is the direction that it being aimed at
					Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
					desiredMove = Vector3.ProjectOnPlane (desiredMove, m_GroundContactNormal).normalized;

					float adjustedMultiplier = m_IscobWeb ? advancedSettings.cobwebMultiplier * (multiplier -0.1f): 1.0f;
					adjustedMultiplier = Mathf.Clamp(adjustedMultiplier,0.4f, 1.0f);

					desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed * adjustedMultiplier;
					desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed * adjustedMultiplier;
					desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed * adjustedMultiplier;
					if (m_RigidBody.velocity.sqrMagnitude <
						(movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed)) {

						switch (movementSettings.currEnum) {
						case MovementSettings.MovementEnum.Forwards:
							m_animator.SetBool("isRunning", true);
							m_animator.SetBool("isBack", false);
							break;
						case MovementSettings.MovementEnum.Backwards:
							m_animator.SetBool("isRunning", false);
							m_animator.SetBool("isBack", true);
							break;
						case MovementSettings.MovementEnum.Strafe:
							m_animator.SetBool("isRunning", true);
							m_animator.SetBool("isBack", false);
							break;
						default:
						break;
						}


						m_RigidBody.AddForce (desiredMove * SlopeMultiplier (), ForceMode.Impulse);
					}
				
				}

				if (m_IsGrounded) {
					m_RigidBody.drag = 5f;

					if (m_Jump) {
						m_RigidBody.drag = 0f;
						m_RigidBody.velocity = new Vector3 (m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
						m_RigidBody.AddForce (new Vector3 (0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
						m_Jumping = true;
					}

					if (!m_Jumping && Mathf.Abs (input.x) < float.Epsilon && Mathf.Abs (input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f) {
						m_animator.SetBool("isRunning", false);
						m_animator.SetBool("isBack", false);
						m_RigidBody.Sleep ();
					}
				} else {
					m_RigidBody.drag = 0f;
					if (m_PreviouslyGrounded && !m_Jumping) {
						StickToGroundHelper ();
					}
				}
				m_Jump = false;

				Debug.DrawRay (transform.position, Vector3.down * (m_Capsule.height / 2f - m_Capsule.radius) * transform.localScale.y, m_IsGrounded ? Color.red : Color.green);
				Debug.DrawRay (transform.position, Vector3.up * (m_Capsule.height / 2f), Color.magenta);
			}
			
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
			if (Physics.SphereCast(transform.position, m_Capsule.radius * transform.localScale.x, Vector3.down, out hitInfo,
                                   (((m_Capsule.height/2f) * transform.localScale.y - m_Capsule.radius * transform.localScale.z) +
                                   advancedSettings.stickToGroundHelperDistance)))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            
            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
			movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
				m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }


        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;

            if (Physics.SphereCast(transform.position, m_Capsule.radius * transform.localScale.x, Vector3.down, out hitInfo,
			                       (((m_Capsule.height/2f) * transform.localScale.y - m_Capsule.radius * transform.localScale.x) + advancedSettings.groundCheckDistance )))
            {
                m_IsGrounded = true;
				m_IscobWeb = hitInfo.collider.tag == "TileTrap";
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
				m_animator.SetBool("isJumping", false);
                m_Jumping = false;
            }
        }
    }
}
