using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class SceneAnchorHelper
{
    public static Vector3 FindRandomPointOnAnchor(OVRSceneAnchor sceneAnchor)
    {
        if (sceneAnchor.TryGetComponent(out OVRScenePlane plane))
        {
            Debug.Log("Scene anchor is a plane (Wall, ceiling or floor)");

            float randomX = Random.Range(0, plane.Width) - (plane.Width * 0.5f);
            float randomZ = Random.Range(0, plane.Height) - (plane.Height * 0.5f);

            Vector3 offsetX = randomX * plane.transform.right;
            Vector3 offsetZ = randomZ * plane.transform.up;

            Vector3 offset = offsetX + offsetZ;

            return plane.transform.position + offset;
        }
        else if (sceneAnchor.TryGetComponent(out OVRSceneVolume volume))
        {
            Debug.Log("Scene anchor is a volume (table/desk)");

            float randomX = Random.Range(0, volume.Width) - (volume.Width * 0.5f);
            float randomZ = Random.Range(0, volume.Depth) - (volume.Depth * 0.5f);

            Vector3 offsetX = randomX * volume.transform.right;
            Vector3 offsetZ = randomZ * volume.transform.up;

            Vector3 offset = offsetX + offsetZ;

            return volume.transform.position + offset;
        }
        else
        {
            Debug.LogError("Scene anchor is neither a plane nor a volume!");
        }

        return Vector3.zero;
    }


    public static Vector3 FindWorldPositionOfPointOnSceneAnchor(OVRSceneAnchor sceneAnchor, float offsetX, float offsetZ)
    {
        Vector3 offsetXInLocalSpace = offsetX * sceneAnchor.transform.right;
        Vector3 offsetZInLocalSpace = offsetZ * sceneAnchor.transform.up;
        Vector3 offset = offsetXInLocalSpace + offsetZInLocalSpace;
        return sceneAnchor.transform.position + offset;
    }
}
