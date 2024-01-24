using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnObjectOnSceneAnchor : MonoBehaviour
{
    [SerializeField] private OVRSceneManager ovrSceneManager;

    private OVRSceneAnchor tableSceneAnchor;

    private List<OVRSceneAnchor> wallsSceneAnchor = new List<OVRSceneAnchor>();

    public enum AnchorTypes { TABLE, RANDOM_WALL, FLOOR, CEILING, RANDOM_WALL_AND_CEILING, RANDOM_WALL_AND_FLOOR, RANDOM_WALL_AND_FLOOR_AND_CEILING }
    public enum SpawnSituation { SurfaceCenter, RandomPointOnSurface }

    private void Awake()
    {
        ovrSceneManager.SceneModelLoadedSuccessfully += OnSceneLoaded;
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

                    break;
                }
                else if (cla.Contains("WALL_FACE"))
                {
                    wallsSceneAnchor.Add(sceneAnchors[i]);

                    Debug.Log("[SpawnObjectOnSceneAnchor] Wall found in OVR scene anchors list.");
                }
            }
        }
    }


    public GameObject SpawnObjectOnAnchorOfType(GameObject obj, AnchorTypes anchorType, SpawnSituation spawnSituation, out OVRSceneAnchor sceneAnchor)
    {
        GameObject result = null;

        if (anchorType == AnchorTypes.TABLE)
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on the table.");

            if (tableSceneAnchor == null)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] No table in the room! Can't spawn this object.");
                sceneAnchor = null;
                return null;
            }

            Vector3 positionToSpawn = Vector3.zero;

            sceneAnchor = tableSceneAnchor;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = tableSceneAnchor.transform.position; // + offsetY;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                positionToSpawn = SceneAnchorHelper.FindRandomPointOnAnchor(sceneAnchor);
            }

            result = Instantiate(obj, positionToSpawn, Quaternion.identity); //, tableSceneAnchor.transform);

            result.transform.up = tableSceneAnchor.transform.forward;
            result.transform.forward = tableSceneAnchor.transform.up;

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }


        else // anchorType = AnchorTypes.RANDOM_WALL
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on a random wall.");

            if (wallsSceneAnchor.Count == 0)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] Walls list is empty! Can't spawn this object.");
                sceneAnchor = null;
                return null;
            }


            int randomWallIndex = Random.Range(0, wallsSceneAnchor.Count);
            OVRSceneAnchor wallSceneAnchor = wallsSceneAnchor[randomWallIndex];
            sceneAnchor = wallSceneAnchor;

            Vector3 positionToSpawn = Vector3.zero;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = wallSceneAnchor.transform.position;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                positionToSpawn = SceneAnchorHelper.FindRandomPointOnAnchor(wallSceneAnchor);
            }

            result = Instantiate(obj, positionToSpawn, Quaternion.identity); //, wallSceneAnchor.transform);

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }



        return result;
    }
}
