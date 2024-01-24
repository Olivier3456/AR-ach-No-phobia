using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnObjectOnSceneAnchor : MonoBehaviour
{
    public GameObject SpawnObjectOnAnchorOfType(GameObject obj, AnchorTypes anchorType, SpawnSituation spawnSituation, out OVRSceneAnchor sceneAnchor)
    {
        GameObject result = null;

        if (anchorType == AnchorTypes.TABLE)
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on the table.");

            if (SceneAnchorHelper.TableSceneAnchor == null)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] No table in the room! Can't spawn this object.");
                sceneAnchor = null;
                return null;
            }

            Vector3 positionToSpawn = Vector3.zero;

            sceneAnchor = SceneAnchorHelper.TableSceneAnchor;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = SceneAnchorHelper.TableSceneAnchor.transform.position; // + offsetY;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                positionToSpawn = SceneAnchorHelper.FindRandomPointOnAnchor(sceneAnchor);
            }

            result = Instantiate(obj, positionToSpawn, Quaternion.identity); //, tableSceneAnchor.transform);

            result.transform.up = SceneAnchorHelper.TableSceneAnchor.transform.forward;
            result.transform.forward = SceneAnchorHelper.TableSceneAnchor.transform.up;

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }


        else // anchorType = AnchorTypes.RANDOM_WALL
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on a random wall.");

            if (SceneAnchorHelper.WallsSceneAnchors.Count == 0)
            {
                Debug.LogError("[SpawnObjectOnSceneAnchor] Walls list is empty! Can't spawn this object.");
                sceneAnchor = null;
                return null;
            }


            int randomWallIndex = Random.Range(0, SceneAnchorHelper.WallsSceneAnchors.Count);
            OVRSceneAnchor wallSceneAnchor = SceneAnchorHelper.WallsSceneAnchors[randomWallIndex];
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
