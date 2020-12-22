using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayScript : MonoBehaviour
{
    [SerializeField]
    float amount;

    float mouseX;
    float mouseY;

    void Update()
    {
        GetInput();

        Quaternion rotation = Quaternion.Euler(-mouseY, mouseX, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, rotation, amount * Time.deltaTime);
    }

    void GetInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
    }
}
