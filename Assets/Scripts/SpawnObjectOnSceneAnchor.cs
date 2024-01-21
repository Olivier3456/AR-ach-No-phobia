using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnSceneAnchor : MonoBehaviour
{
    [SerializeField] private OVRSceneManager ovrSceneManager;

    private OVRSceneVolume tableVolume;
    private List<OVRScenePlane> walls = new List<OVRScenePlane>();

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
                //Debug.Log("[SpawnObjectOnSceneAnchor] OVRSemanticClassification found on scene anchor.");

                if (cla.Contains("TABLE") || cla.Contains("DESK"))
                {
                    tableVolume = sceneAnchors[i].transform.GetComponent<OVRSceneVolume>();

                    Debug.Log("[SpawnObjectOnSceneAnchor] Table or desk found in OVR scene anchors list.");

                    break;
                }
                else if (cla.Contains("WALL_FACE"))
                {
                    walls.Add(sceneAnchors[i].transform.GetComponent<OVRScenePlane>());

                    Debug.Log("[SpawnObjectOnSceneAnchor] Wall found in OVR scene anchors list.");
                }
            }
        }
    }


    public GameObject SpawnObjectOnAnchorOfType(GameObject obj, AnchorTypes anchorType, SpawnSituation spawnSituation)
    {
        GameObject result = null;

        if (anchorType == AnchorTypes.TABLE)
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on the table.");

            if (tableVolume == null)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] No table in the room! Can't spawn this object.");
                return null;
            }


            //Vector3 offsetY = Vector3.zero;
            Vector3 positionToSpawn = Vector3.zero;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = tableVolume.transform.position; // + offsetY;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                float randomX = Random.Range(0, tableVolume.Width) - (tableVolume.Width * 0.5f);
                float randomZ = Random.Range(0, tableVolume.Depth) - (tableVolume.Depth * 0.5f);

                Vector3 offsetX = randomX * tableVolume.transform.right;
                Vector3 offsetZ = randomZ * tableVolume.transform.up;

                Vector3 offset = offsetX + offsetZ;

                positionToSpawn = tableVolume.transform.position + offset;
            }

            Quaternion rotation = Quaternion.Euler(tableVolume.transform.forward);

            result = Instantiate(obj, positionToSpawn, rotation);

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }


        else if (anchorType == AnchorTypes.RANDOM_WALL)
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on a random wall.");

            if (walls.Count == 0)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] Walls list is empty! Can't spawn this object.");
                return null;
            }


            int randomWallIndex = Random.Range(0, walls.Count);
            OVRScenePlane wall = walls[randomWallIndex];

            Vector3 positionToSpawn = Vector3.zero;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = wall.transform.position;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                float randomX = Random.Range(0, wall.Width) - (wall.Width * 0.5f);
                float randomZ = Random.Range(0, wall.Height) - (wall.Height * 0.5f);

                Vector3 offsetX = randomX * wall.transform.up;
                Vector3 offsetZ = randomZ * wall.transform.right;

                Vector3 offset = offsetX + offsetZ;

                positionToSpawn = wall.transform.position + offset;
            }

            Quaternion rotation = Quaternion.Euler(wall.transform.forward);

            result = Instantiate(obj, positionToSpawn, rotation);

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }



        return result;
    }
}
