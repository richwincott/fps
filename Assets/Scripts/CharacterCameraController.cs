using Photon.Pun;
using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float lookSpeed = 10f;
    [SerializeField]
    private float smoothing = 3f;

    private float camRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!player.GetComponent<PhotonView>().IsMine)
        {
            return;
        }

        float y = -Input.GetAxis("Mouse Y");
        float x = Input.GetAxis("Mouse X");

        camRotation += y * lookSpeed * Time.deltaTime * 10f;
        camRotation = Mathf.Clamp(camRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(camRotation, 0f, 0f);

        player.transform.Rotate(x * lookSpeed * Time.deltaTime * 10f * Vector3.up);

        if (Input.GetMouseButton(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}