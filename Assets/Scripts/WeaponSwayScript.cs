using UnityEngine;

public class WeaponSwayScript : MonoBehaviour
{
    [SerializeField]
    public bool isEnabled = false;
    [SerializeField]
    private float mouseSensitivity = 10f;
    [SerializeField]
    private float maxMoveAmount = 0.5f;
    [SerializeField]
    private float smoothSpeed = 3f;
    [SerializeField]
    private bool bRotate;
    [SerializeField]
    private float smoothRotationSpeed = 2f;
    [SerializeField]
    private float rotateAmount = 45f;
    [SerializeField]
    private GameObject player;

    private Vector3 startPos;
    private Quaternion startRot;
    private float factorX;
    private float factorY;

    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    private void Update()
    {
        if (isEnabled)
        {
            // STORE MOUSE MOVEMENT AMOUNTS, SMOOTH WITH LERP
            factorX = Mathf.Lerp(factorX, -Input.GetAxis("Mouse X") * mouseSensitivity, Time.deltaTime * 10f);
            factorY = Mathf.Lerp(factorY, -Input.GetAxis("Mouse Y") * mouseSensitivity, Time.deltaTime * 10f);

            // CLAMP LIMITS
            if (factorX > maxMoveAmount)
                factorX = maxMoveAmount;

            if (factorX < -maxMoveAmount)
                factorX = -maxMoveAmount;

            if (factorY > maxMoveAmount)
                factorY = maxMoveAmount;

            if (factorY < -maxMoveAmount)
                factorY = -maxMoveAmount;

            // SET TARGET POSITION (START POSITION + MOUSE MOVEMENT)
            Vector3 targetPos = new Vector3(startPos.x + factorX, startPos.y + factorY, startPos.z);

            // APPLY POSITION TO WEAPON, SMOOTH WITH LERP
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smoothSpeed * Time.deltaTime);

            // ROTATION
            if (bRotate)
            {
                float tiltAroundZ = Input.GetAxis("Mouse X") * rotateAmount;
                float tiltAroundX = Input.GetAxis("Mouse Y") * rotateAmount;
                Vector3 target = new Vector3(tiltAroundX, 0f, tiltAroundZ);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(target), Time.deltaTime * smoothRotationSpeed);
            }
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, startPos, Time.deltaTime * smoothSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRot, Time.deltaTime * smoothRotationSpeed);
        }
    }
}