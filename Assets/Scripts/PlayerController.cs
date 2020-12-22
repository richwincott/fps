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

    WeaponController currentWeaponController;
    float horizontal;
    float vertical;
    float mouseX;
    float mouseY;
    bool grounded;
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
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            Destroy(rb);
        }
    }

    public override void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        base.OnEnable();
    }

    public override void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        base.OnDisable();
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        GetInputs();
        Move();
        Look();
        Jump();
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

    void Look()
    {
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);
        rb.MoveRotation(transform.rotation * deltaRotation);

        cameraHolder.transform.localRotation *= Quaternion.Euler(Vector3.left * mouseY * mouseSensitivity * Time.deltaTime);
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