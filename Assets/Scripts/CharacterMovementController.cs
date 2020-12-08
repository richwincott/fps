using Photon.Pun;
using UnityEngine;

public class CharacterMovementController : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float sprintSpeedMultiplier = 2f;
    [SerializeField]
    private float jumpHeight = 3f;
    [SerializeField]
    private WeaponHolderController weaponHolderController;

    private WeaponController currentWeaponController;
    private float gravity = -10f;
    private float yAxisVelocity;

    private bool grounded = true;

    private Vector3 movement;
    private float horizontal;
    private float vertical;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        currentWeaponController = weaponHolderController.GetCurrentWeapon().GetComponent<WeaponController>();

        if (!PV.IsMine)
        {
            // exit from update if this is not the local player
            return;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            vertical *= sprintSpeedMultiplier;
            currentWeaponController.animator.Play("Sprint");
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentWeaponController.animator.Play("Idle");
        }

        

        if (characterController.isGrounded)
            yAxisVelocity = -0.5f;


        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            yAxisVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            grounded = false;
        }

        

        

        if (!grounded)
            checkIfGrounded();
    }

    void FixedUpdate()
    {
        movement = horizontal * moveSpeed * Time.deltaTime * transform.right +
                            vertical * moveSpeed * Time.deltaTime * transform.forward;

        yAxisVelocity += gravity * Time.deltaTime;
        movement.y = yAxisVelocity * Time.deltaTime;

        characterController.Move(movement);
    }

    private void checkIfGrounded()
    {
        grounded = Physics.Raycast(transform.position, -transform.up, 1);
    }
}