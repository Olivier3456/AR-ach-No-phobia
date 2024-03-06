using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public enum AnchorTypes { TABLE, RANDOM_WALL, FLOOR, CEILING, OTHER, NONE }
public enum SpawnSituation { SurfaceCenter, RandomPointOnSurface }

public class SceneAnchorHelper : MonoBehaviour
{
    [SerializeField] private OVRSceneManager sceneManager;

    public static UnityEvent<bool> OnSceneAnchorsLoaded = new UnityEvent<bool>();


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
                if (GetAnchorType(sceneAnchors[i]) == AnchorTypes.TABLE)
                {
                    tableSceneAnchor = sceneAnchors[i];
                    Debug.Log("Table or desk found in OVR scene anchors list.");
                }
                else if (GetAnchorType(sceneAnchors[i]) == AnchorTypes.RANDOM_WALL)
                {
                    wallsSceneAnchors.Add(sceneAnchors[i]);
                    Debug.Log("Wall found in OVR scene anchors list.");
                }
                else if (GetAnchorType(sceneAnchors[i]) == AnchorTypes.CEILING)
                {
                    ceilingSceneAnchor = sceneAnchors[i];
                    Debug.Log("Ceiling found in OVR scene anchors list.");
                }
                else if (GetAnchorType(sceneAnchors[i]) == AnchorTypes.FLOOR)
                {
                    floorSceneAnchor = sceneAnchors[i];
                    Debug.Log("Floor found in OVR scene anchors list.");
                }
            }
        }

        //if (tableSceneAnchor == null) Debug.LogError("No table found in anchors list!");
        //if (wallsSceneAnchors.Count == 0) Debug.LogError("No Wall found in anchors list!");
        //if (ceilingSceneAnchor == null) Debug.LogError("No ceiling found in anchors list!");
        //if (floorSceneAnchor == null) Debug.LogError("No floor found in anchors list!");

        if (tableSceneAnchor == null || wallsSceneAnchors.Count < 4 || ceilingSceneAnchor == null || floorSceneAnchor == null)
        {
            OnSceneAnchorsLoaded.Invoke(false); // All necessary anchors have NOT been found: MainManager needs to call Meta Space Setup.
        }
        else
        {
            OnSceneAnchorsLoaded.Invoke(true);
        }
    }


    // Static methods

    public static Vector3 RandomPointOnAnchorSurface(OVRSceneAnchor sceneAnchor, float marginDistance = 0.1f)
    {
        Debug.Log($"Finding random point on a scene anchor {sceneAnchor.name}");

        if (sceneAnchor.TryGetComponent(out OVRScenePlane plane))
        {
            Debug.Log("Scene anchor is a plane (Wall, ceiling or floor)");

            float randomX = Random.Range(0, plane.Width - marginDistance) - ((plane.Width - marginDistance) * 0.5f);
            float randomZ = Random.Range(0, plane.Height - marginDistance) - ((plane.Height - marginDistance) * 0.5f);

            Vector3 offsetX = randomX * plane.transform.right;
            Vector3 offsetZ = randomZ * plane.transform.up;

            Vector3 offset = offsetX + offsetZ;

            return plane.transform.position + offset;
        }
        else if (sceneAnchor.TryGetComponent(out OVRSceneVolume volume))
        {
            Debug.Log("Scene anchor is a volume (table/desk)");

            float randomX = Random.Range(0, volume.Width - marginDistance) - ((volume.Width - marginDistance) * 0.5f);
            float randomZ = Random.Range(0, volume.Depth - marginDistance) - ((volume.Depth - marginDistance) * 0.5f);

            Vector3 offsetX = randomX * volume.transform.right;
            Vector3 offsetZ = randomZ * volume.transform.up;

            Vector3 offset = offsetX + offsetZ;

            return volume.transform.position + offset;
        }
        else
        {
            Debug.LogError("Scene anchor is neither a plane nor a volume! Can't find a random point on it. Returning Vector3.zero.");
        }

        return Vector3.zero;
    }


    //public static Vector3 CenterPointOfSurfaceAnchor(OVRSceneAnchor sceneAnchor)
    //{
    //    if (sceneAnchor.TryGetComponent(out OVRScenePlane plane))
    //    {
    //        return plane.transform.position;
    //    }
    //    else if (sceneAnchor.TryGetComponent(out OVRSceneVolume volume))
    //    {
    //        return volume.transform.position;
    //    }
    //    else
    //    {
    //        return Vector3.zero;
    //    }
    //}


    //public static Vector3 WorldPositionOfAPointOnSceneAnchorSurface(OVRSceneAnchor sceneAnchor, float offsetX, float offsetZ)
    //{
    //    Vector3 offsetXInLocalSpace = offsetX * sceneAnchor.transform.right;
    //    Vector3 offsetZInLocalSpace = offsetZ * sceneAnchor.transform.up;
    //    Vector3 offset = offsetXInLocalSpace + offsetZInLocalSpace;
    //    return sceneAnchor.transform.position + offset;
    //}


    public static AnchorTypes GetAnchorType(OVRSceneAnchor sceneAnchor)
    {
        OVRSemanticClassification cla = sceneAnchor.transform.GetComponent<OVRSemanticClassification>();

        if (cla != null)
        {
            if (cla.Contains("TABLE") || cla.Contains("DESK"))
            {
                return AnchorTypes.TABLE;
            }
            else if (cla.Contains("WALL_FACE"))
            {
                return AnchorTypes.RANDOM_WALL;
            }
            else if (cla.Contains("CEILING"))
            {
                return AnchorTypes.CEILING;
            }
            else if (cla.Contains("FLOOR"))
            {
                return AnchorTypes.FLOOR;
            }
            else
            {
                return AnchorTypes.OTHER;
            }
        }
        else
        {
            return AnchorTypes.NONE;
        }
    }
}
