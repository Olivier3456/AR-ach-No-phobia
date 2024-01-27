using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_SpawnObjectsAtStart : MonoBehaviour
{
    public OVRSceneManager sceneManager;
    public SpawnObjectOnSceneAnchor spawner;

    public GameObject objectToSpawn;
    public int numberToSpawn = 10;

    private void OnEnable()
    {
        SceneAnchorHelper.OnSceneAnchorsFound.AddListener(OnSceneAnchorsFound);
    }

    private void OnDisable()
    {
        SceneAnchorHelper.OnSceneAnchorsFound.RemoveListener(OnSceneAnchorsFound);
    }

    private void OnSceneAnchorsFound()
    {
        StartCoroutine(WaitAndSpawnObjects());
    }


    IEnumerator WaitAndSpawnObjects()
    {
        yield return new WaitForSeconds(2);

        for (int i = 0; i < numberToSpawn; i++)
        {
            spawner.SpawnObjectOnAnchorOfType(objectToSpawn, AnchorTypes.RANDOM_WALL, SpawnSituation.RandomPointOnSurface, out OVRSceneAnchor sceneAnchor);
        }
    }
}
