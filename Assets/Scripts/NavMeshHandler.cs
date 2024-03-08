using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public static class NavMeshHandler
{
    private static NavMeshSurface[] sceneNavMeshSurfaces = null;


    public static void BuildNavMesh(OVRSceneAnchor sceneAnchor)
    {
        NavMeshSurface navMeshSurface = sceneAnchor.gameObject.GetComponentInChildren<NavMeshSurface>();

        navMeshSurface.BuildNavMesh();

        Debug.Log($"NavMesh built for anchor {sceneAnchor.name}");
    }

    public static void BuildAllNavMeshes(bool onlyFirstTime)
    {
        if (sceneNavMeshSurfaces == null)
        {
            sceneNavMeshSurfaces = GameObject.FindObjectsOfType<NavMeshSurface>(true);
        }
        else if (onlyFirstTime)
        {
            return;
        }
        
        foreach (NavMeshSurface surface in sceneNavMeshSurfaces)
        {
            surface.BuildNavMesh();
        }
    }
}
