using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAnchor : MonoBehaviour
{
    private OVRSceneAnchor sceneAnchor;
    private float offsetXfloat;
    private float offsetZfloat;

    private bool isInitDone = false;

    private bool shouldFollowAnchor = true;

    public void Init(OVRSceneAnchor sceneAnchor, float offsetX, float offsetZ)
    {
        this.sceneAnchor = sceneAnchor;
        offsetXfloat = offsetX;
        offsetZfloat = offsetZ;
        isInitDone = true;
    }

    public void ShouldFollowAnchor(bool value)
    {
        shouldFollowAnchor = value;
    }

    private void Update()
    {
        if (!shouldFollowAnchor)
        {
            return;
        }

        if (!isInitDone)
        {
            Debug.LogError("Init is not done! Can't follow anchor.");
            return;
        }        

        Vector3 offsetX = offsetXfloat * sceneAnchor.transform.right;
        Vector3 offsetZ = offsetZfloat * sceneAnchor.transform.up;
        Vector3 offset = offsetX + offsetZ;
        transform.position = sceneAnchor.transform.position + offset;
    }   
}
