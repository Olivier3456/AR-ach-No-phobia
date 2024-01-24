using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum AnchorTypes { TABLE, RANDOM_WALL, FLOOR, CEILING, RANDOM_WALL_AND_CEILING, RANDOM_WALL_AND_FLOOR, RANDOM_WALL_AND_FLOOR_AND_CEILING }
public enum SpawnSituation { SurfaceCenter, RandomPointOnSurface }

public class SceneAnchorHelper : MonoBehaviour
{
    [SerializeField] private OVRSceneManager sceneManager;

    
    private static OVRSceneAnchor tableSceneAnchor = null;
    public static OVRSceneAnchor TableSceneAnchor { get { return tableSceneAnchor; } }

    
    private static List<OVRSceneAnchor> wallsSceneAnchors = new List<OVRSceneAnchor>();
    public static List<OVRSceneAnchor> WallsSceneAnchors { get { return wallsSceneAnchors; } }


    private static OVRSceneAnchor ceilingSceneAnchor = null;
    public static OVRSceneAnchor CeilingSceneAnchor { get { return ceilingSceneAnchor; } }


    private static OVRSceneAnchor floorSceneAnchor = null;
    public static OVRSceneAnchor FloorSceneAnchor { get { return floorSceneAnchor; } }


    private void Awake()
    {
        sceneManager.SceneModelLoadedSuccessfully += OnSceneLoaded;
    }

    private void OnSceneLoaded()
    {
        List<OVRSceneAnchor> sceneAnchors = new List<OVRSceneAnchor>();
        OVRSceneAnchor.GetSceneAnchors(sceneAnchors);

        for (int i = 0; i < sceneAnchors.Count; i++)
        {
            OVRSemanticClassification cla = sceneAnchors[i].transform.GetComponent<OVRSemanticClassification>();

            if (cla != null)
            {
                if (cla.Contains("TABLE") || cla.Contains("DESK"))
                {
                    tableSceneAnchor = sceneAnchors[i];

                    Debug.Log("[SpawnObjectOnSceneAnchor] Table or desk found in OVR scene anchors list.");
                }
                else if (cla.Contains("WALL_FACE"))
                {
                    wallsSceneAnchors.Add(sceneAnchors[i]);

                    Debug.Log("[SpawnObjectOnSceneAnchor] Wall found in OVR scene anchors list.");
                }
                else if (cla.Contains("CEILING"))
                {
                    ceilingSceneAnchor = sceneAnchors[i];

                    Debug.Log("[SpawnObjectOnSceneAnchor] Ceiling found in OVR scene anchors list.");
                }
                else if (cla.Contains("FLOOR"))
                {
                    floorSceneAnchor = sceneAnchors[i];

                    Debug.Log("[SpawnObjectOnSceneAnchor] Floor found in OVR scene anchors list.");
                }
            }
        }
    }


    // Static methods

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
