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
            
            Vector3 positionToSpawn = Vector3.zero;

            sceneAnchor = SceneAnchorHelper.TableSceneAnchor;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = SceneAnchorHelper.TableSceneAnchor.transform.position; // + offsetY;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                positionToSpawn = SceneAnchorHelper.RandomPointOnAnchorSurface(sceneAnchor);
            }

            result = Instantiate(obj, positionToSpawn, Quaternion.identity); //, tableSceneAnchor.transform);

            result.transform.up = SceneAnchorHelper.TableSceneAnchor.transform.forward;
            result.transform.forward = SceneAnchorHelper.TableSceneAnchor.transform.up;

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }
        else
        {
            OVRSceneAnchor sceneAnchorToSpawnObject = null;

            if (anchorType == AnchorTypes.RANDOM_WALL)
            {
                Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on a random wall.");
                int randomWallIndex = Random.Range(0, SceneAnchorHelper.WallsSceneAnchors.Count);
                sceneAnchorToSpawnObject = SceneAnchorHelper.WallsSceneAnchors[randomWallIndex];
            }
            else if (anchorType == AnchorTypes.CEILING)
            {
                Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on the ceiling.");
                sceneAnchorToSpawnObject = SceneAnchorHelper.CeilingSceneAnchor;
            }
            else if (anchorType == AnchorTypes.FLOOR)
            {
                Debug.Log("[SpawnObjectOnSceneAnchor] Desired spawn place is on the floor.");
                sceneAnchorToSpawnObject = SceneAnchorHelper.FloorSceneAnchor;
            }

            sceneAnchor = sceneAnchorToSpawnObject;

            Vector3 positionToSpawn = Vector3.zero;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = sceneAnchorToSpawnObject.transform.position;
            }
            else if (spawnSituation == SpawnSituation.RandomPointOnSurface)
            {
                positionToSpawn = SceneAnchorHelper.RandomPointOnAnchorSurface(sceneAnchorToSpawnObject);
            }

            result = Instantiate(obj, positionToSpawn, Quaternion.identity); //, sceneAnchorToSpawnObject.transform);

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");
        }

        return result;
    }
}
