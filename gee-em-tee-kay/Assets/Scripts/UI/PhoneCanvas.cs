using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCanvas : MonoBehaviour
{
    [SerializeField] private Transform phone;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = phone.position;
    }
}
