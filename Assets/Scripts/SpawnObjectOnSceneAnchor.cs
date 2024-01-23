using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnObjectOnSceneAnchor : MonoBehaviour
{
    [SerializeField] private OVRSceneManager ovrSceneManager;

    private OVRSceneAnchor tableSceneAnchor;
    //private OVRSceneVolume tableSceneVolume;

    private List<OVRSceneAnchor> wallsSceneAnchor = new List<OVRSceneAnchor>();
    //private List<OVRScenePlane> wallsScenePlane = new List<OVRScenePlane>();

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
                    tableSceneAnchor = sceneAnchors[i];
                    //tableSceneVolume = sceneAnchors[i].transform.GetComponent<OVRSceneVolume>();

                    Debug.Log("[SpawnObjectOnSceneAnchor] Table or desk found in OVR scene anchors list.");

                    break;
                }
                else if (cla.Contains("WALL_FACE"))
                {
                    wallsSceneAnchor.Add(sceneAnchors[i]);
                    //wallsScenePlane.Add(sceneAnchors[i].transform.GetComponent<OVRScenePlane>());

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

            //if (tableSceneVolume == null)
            if (tableSceneAnchor == null)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] No table in the room! Can't spawn this object.");
                return null;
            }


            //Vector3 offsetY = Vector3.zero;
            Vector3 positionToSpawn = Vector3.zero;

            OVRSceneVolume tableSceneVolume = tableSceneAnchor.transform.GetComponent<OVRSceneVolume>();

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = tableSceneVolume.transform.position; // + offsetY;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                float randomX = Random.Range(0, tableSceneVolume.Width) - (tableSceneVolume.Width * 0.5f);
                float randomZ = Random.Range(0, tableSceneVolume.Depth) - (tableSceneVolume.Depth * 0.5f);

                Vector3 offsetX = randomX * tableSceneVolume.transform.right;
                Vector3 offsetZ = randomZ * tableSceneVolume.transform.up;

                Vector3 offset = offsetX + offsetZ;

                positionToSpawn = tableSceneVolume.transform.position + offset;
            }

            Quaternion rotation = Quaternion.Euler(tableSceneAnchor.transform.forward);

            result = Instantiate(obj, positionToSpawn, rotation); //, tableSceneAnchor.transform);

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }


        else if (anchorType == AnchorTypes.RANDOM_WALL)
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on a random wall.");

            if (wallsSceneAnchor.Count == 0)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] Walls list is empty! Can't spawn this object.");
                return null;
            }


            int randomWallIndex = Random.Range(0, wallsSceneAnchor.Count);
            OVRSceneAnchor wallSceneAnchor = wallsSceneAnchor[randomWallIndex];
            OVRScenePlane wall = wallSceneAnchor.transform.GetComponent<OVRScenePlane>();

            Vector3 positionToSpawn = Vector3.zero;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = wall.transform.position;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                float randomX = Random.Range(0, wall.Width) - (wall.Width * 0.5f);
                float randomZ = Random.Range(0, wall.Height) - (wall.Height * 0.5f);

                Vector3 offsetX = randomX * wall.transform.right;
                Vector3 offsetZ = randomZ * wall.transform.up;

                Vector3 offset = offsetX + offsetZ;

                positionToSpawn = wall.transform.position + offset;
            }

            Quaternion rotation = Quaternion.Euler(wallSceneAnchor.transform.forward);

            result = Instantiate(obj, positionToSpawn, rotation); //, wallSceneAnchor.transform);            

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }



        return result;
    }
}
