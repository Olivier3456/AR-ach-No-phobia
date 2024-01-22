using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshHandler : MonoBehaviour
{
    [SerializeField] private OVRSceneManager ovrSceneManager;

    [SerializeField] private NavMeshSurface[] navMeshSurfaces;


    private void Awake()
    {
        ovrSceneManager.SceneModelLoadedSuccessfully += OnSceneLoaded;
    }


    private void OnSceneLoaded()
    {
        //foreach (NavMeshSurface surface in navMeshSurfaces)
        //{
        //    surface.BuildNavMesh();
        //}

        StartCoroutine(WaitALittleBeforeBuildNavMesh());
    }
    private IEnumerator WaitALittleBeforeBuildNavMesh()
    {
        yield return new WaitForSeconds(1);

        foreach (NavMeshSurface surface in navMeshSurfaces)
        {
            surface.BuildNavMesh();
        }
    }
}
