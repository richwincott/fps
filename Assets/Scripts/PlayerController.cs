using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject cameraHolder;
    [SerializeField]
    float mouseSensitivity, walkSpeedMultiplier, sprintSpeedMultiplier, jumpForce, smoothTime;
    [SerializeField]
    WeaponHolderController weaponHolderController;

    private WeaponController currentWeaponController;
    private float horizontal;
    private float vertical;
    private float mouseX;
    private float mouseY;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView PV;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            Destroy(rb);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        GetInputs();
        LookY();
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
        LookX();
    }

    void GetInputs()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        currentWeaponController = weaponHolderController.GetCurrentWeapon().GetComponent<WeaponController>();

        if (Input.GetKey(KeyCode.LeftShift))
            currentWeaponController.animator.Play("Sprint");
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            currentWeaponController.animator.Play("Idle");
    }

    void LookY()
    {
        verticalLookRotation += mouseY * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void LookX()
    {
        Quaternion deltaRotation = Quaternion.Euler(-Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);

    }

    void Move()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeedMultiplier : walkSpeedMultiplier;
        Vector3 deltaMovement = new Vector3(horizontal, 0, vertical).normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + transform.TransformDirection(deltaMovement));
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
}