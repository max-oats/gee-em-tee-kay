using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 cameraEuler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(cameraEuler.x, Global.cameraController.transform.eulerAngles.y, cameraEuler.z);
    }
}
