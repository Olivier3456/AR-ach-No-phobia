using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public static class NavMeshHandler
{
    public static void BuildNavMesh(OVRSceneAnchor sceneAnchor)
    {
        NavMeshSurface navMeshSurface = sceneAnchor.gameObject.GetComponentInChildren<NavMeshSurface>();

        navMeshSurface.BuildNavMesh();

        Debug.Log($"NavMesh built for anchor {sceneAnchor.name}");
    }
}
