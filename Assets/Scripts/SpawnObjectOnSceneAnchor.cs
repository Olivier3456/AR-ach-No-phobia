using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class SpawnObjectOnSceneAnchor : MonoBehaviour
{
    [SerializeField] private OVRSceneManager ovrSceneManager;

    private OVRSceneVolume tableVolume;

    public enum AnchorTypes { TABLE, WALL, FLOOR, CEILING }

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

    public GameObject SpawnObject(GameObject obj, AnchorTypes anchorType)
    {
        GameObject result = null;

        if (anchorType == AnchorTypes.TABLE && tableVolume != null)
        {
            Vector3 offsetY = Vector3.zero;
            Vector3 positionToSpawn = tableVolume.transform.position + offsetY;
            Quaternion rotation = Quaternion.Euler(tableVolume.transform.forward);  // Or right? Up?

            result = Instantiate(obj, positionToSpawn, rotation);

            //A décommenter après le test de la rotation correcte ci-dessus.
            //Vector3 fromObjectToCamera = Camera.main.transform.position - obj.transform.position;
            //float dot = Vector3.Dot(fromObjectToCamera, obj.transform.forward);
            //if (dot < 0) // The object is not facing camera. We neet to flip it.
            //{
            //    obj.transform.Rotate(Vector3.up, 180);
            //}
        }

        return result;
    }
}
