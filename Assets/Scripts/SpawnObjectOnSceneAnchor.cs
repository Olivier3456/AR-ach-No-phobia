using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class SpawnObjectOnSceneAnchor : MonoBehaviour
{
    [SerializeField] private OVRSceneManager ovrSceneManager;

    private OVRSceneVolume tableVolume;

    public enum AnchorTypes { TABLE, WALL, FLOOR, CEILING }
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

                    Debug.Log("[SpawnObjectOnSceneAnchor] Table or desk found.");

                    break;
                }
            }
        }
    }

    public GameObject SpawnObjectOnAnchorOfType(GameObject obj, AnchorTypes anchorType, SpawnSituation spawnSituation)
    {
        Debug.Log("[SpawnObjectOnSceneAnchor] SpawnObjectOnAnchorOfType() called");


        GameObject result = null;

        if (anchorType == AnchorTypes.TABLE && tableVolume != null)
        {
            Debug.Log("[SpawnObjectOnSceneAnchor] Desired anchor type for object spawn is TABLE");


            Vector3 offsetY = Vector3.zero;
            Vector3 positionToSpawn = Vector3.zero;

            if (spawnSituation == SpawnSituation.SurfaceCenter)
            {
                positionToSpawn = tableVolume.transform.position + offsetY;
            }
            else
            {
                // TODO: define a random point on the anchor object.



            }

            Quaternion rotation = Quaternion.Euler(tableVolume.transform.forward);

            result = Instantiate(obj, positionToSpawn, rotation);

            Debug.Log($"[SpawnObjectOnSceneAnchor] Object {result} instantiated at position {positionToSpawn}.");


            //Vector3 fromObjectToCamera = Camera.main.transform.position - obj.transform.position;
            //float dot = Vector3.Dot(fromObjectToCamera, obj.transform.forward);

            //Debug.Log("[SpawnObjectOnSceneAnchor] dot = " + dot);

            //if (dot < 0) // The object is not facing camera. We neet to flip it.
            //{
            //    Debug.Log("[SpawnObjectOnSceneAnchor] dot < 0: rotate game obejct.");

            //    obj.transform.Rotate(Vector3.up, 180);
            //}




        }

        return result;
    }
}
