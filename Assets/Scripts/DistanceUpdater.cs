using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceUpdater : MonoBehaviour
{
    private Transform camTransform;
    private static int frameCount = 0;
    private static List<float> distanceFromCam = new List<float>();
    private static int distanceUpdatersInstantiated = 0;
    public static int DistanceUpdatersInstantiated { get => distanceUpdatersInstantiated; }

    private static float minDistanceFromCam = Mathf.Infinity;   // The shortest distance from cam for this frame (reinitialized each frame)
    public static float MinDistanceFromCam { get => minDistanceFromCam; }

    private static float minDistanceFromCamEver = Mathf.Infinity;    // The min distance since the beginning of the exercise
    public static float MinDistanceFromCamEver { get => minDistanceFromCamEver; }

    private static float averageDistanceFromCam = 0f;   // Do you really need a comment here?
    public static float AverageDistanceFromCam { get => averageDistanceFromCam; }


    void Start()
    {
        distanceUpdatersInstantiated++;
        camTransform = Camera.main.transform;
    }


    void Update()
    {
        UpdateDistanceFromCamera();
    }


    private void UpdateDistanceFromCamera()
    {
        float currentDistanceFromCam = Vector3.Distance(transform.position, camTransform.position);

        distanceFromCam.Add(currentDistanceFromCam);

        if (distanceFromCam.Count == distanceUpdatersInstantiated)
        {
            frameCount++;
            minDistanceFromCam = Mathf.Infinity;

            foreach (float distance in distanceFromCam)
            {
                if (distance < minDistanceFromCam)
                {
                    minDistanceFromCam = distance;
                }
            }

            if (minDistanceFromCamEver > minDistanceFromCam)
            {
                minDistanceFromCamEver = minDistanceFromCam;
            }

            averageDistanceFromCam = ((averageDistanceFromCam * (frameCount - 1)) + minDistanceFromCam) / frameCount;

            distanceFromCam.Clear();
        }
    }

    private void OnDestroy()
    {
        distanceUpdatersInstantiated--;

        if (distanceUpdatersInstantiated == 0)   // Reinitialize static variables for the next exercise
        {
            frameCount = 0;
            distanceFromCam.Clear();
            minDistanceFromCam = Mathf.Infinity;
            minDistanceFromCamEver = Mathf.Infinity;
            averageDistanceFromCam = 0f;
        }
    }
}
