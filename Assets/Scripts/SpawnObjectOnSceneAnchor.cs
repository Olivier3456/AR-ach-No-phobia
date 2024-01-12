using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class SpawnObjectOnSceneAnchor : MonoBehaviour
{
    [SerializeField] private OVRSceneManager ovrSceneManager;

    [SerializeField] private GameObject prefabToSpawn;

    private OVRSceneAnchor tableAnchor;

    private OVRSceneVolume tableVolume;

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
                Debug.Log("[SpawnObjectOnSceneAnchor] OVRSemanticClassification found on scene anchor.");

                if (cla.Contains("TABLE") || cla.Contains("DESK"))
                {
                    tableAnchor = sceneAnchors[i];
                    tableVolume = sceneAnchors[i].transform.GetComponent<OVRSceneVolume>();

                    Debug.Log("[SpawnObjectOnSceneAnchor] Table or desk found.");

                    break;
                }
            }
        }

        if (tableAnchor != null)
        {
            Vector3 offsetY = new Vector3(0, tableVolume.Height * 0.5f, 0);


            offsetY = Vector3.zero;
            Vector3 positionToSpawn = tableVolume.transform.position + offsetY;

            Instantiate(prefabToSpawn, positionToSpawn, Quaternion.identity);       // TODO: rotation, in front of the player.
        }


    }


}
