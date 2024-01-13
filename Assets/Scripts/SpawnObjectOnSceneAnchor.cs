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

    public GameObject SpawnObject(GameObject obj, AnchorTypes anchorTypes)
    {
        GameObject result = null;

        if (anchorTypes == AnchorTypes.TABLE && tableVolume != null)
        {
            Vector3 offsetY = Vector3.zero;
            Vector3 positionToSpawn = tableVolume.transform.position + offsetY;
            result = Instantiate(obj, positionToSpawn, tableVolume.transform.rotation);       // TODO: for rotation, verify that it is in front of the player.
        }

        return result;
    }
}
