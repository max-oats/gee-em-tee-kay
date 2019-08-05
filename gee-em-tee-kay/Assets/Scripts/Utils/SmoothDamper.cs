using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SmoothDamper
{
    public float smoothTime;

    // FLOAT VERSION
    private float current;

    private float desired;

    public float velocity {get; private set;}

    public float defaultDesired;

    public bool angleSmooth = false;

    // VECTOR VERSION
    private Vector3 currentVector;
    private Vector3 desiredVector;
    private Vector3 velocityVector;
    public Vector3 defaultDesiredVector;

    public void Init()
    {
        current = defaultDesired;
        desired = defaultDesired;
    }

    public void InitVector()
    {
        currentVector = defaultDesiredVector;
        desiredVector = defaultDesiredVector;
    }

    public void SetCurrent(float current)
    {
        this.current = current;
    }

    public void SetCurrent(Vector3 current)
    {
        this.currentVector = current;
    }

    public void SetDesired()
    {
        desired = defaultDesired;
    }

    public void SetDesired(float desired)
    {
        this.desired = desired;
    }

    public void SetDesired(float desired, float delay)
    {
        SetDesiredWithDelay(desired, delay);
    }

    private IEnumerator SetDesiredWithDelay(float desired, float delay)
    {
        yield return new WaitForSeconds(delay);

        SetDesired(desired);
    }

    public void SetDesired(Vector3 desiredVector)
    {
        this.desiredVector = desiredVector;
    }

    public float Smooth()
    {
        if (NeedsSmoothing())
        {
            float newVelocity = velocity;

            if (!angleSmooth)
                current = Mathf.SmoothDamp(current, desired, ref newVelocity, smoothTime);
            else
                current = Mathf.SmoothDampAngle(current, desired, ref newVelocity, smoothTime);
                
            velocity = newVelocity;
        }

        return current;
    }

    public Vector3 SmoothVector()
    {
        Vector3 newVelocity = velocityVector;

        currentVector = Vector3.SmoothDamp(currentVector, desiredVector, ref newVelocity, smoothTime);

        velocityVector = newVelocity;
        return currentVector;
    }

    public bool NeedsSmoothing()
    {
        return !Mathf.Approximately(current, desired);
    }

    public bool NeedsSmoothingVector()
    {
        return !(Mathf.Approximately(currentVector.x, desiredVector.x) 
                && Mathf.Approximately(currentVector.y, desiredVector.y) 
                && Mathf.Approximately(currentVector.z, desiredVector.z));
    }
}
