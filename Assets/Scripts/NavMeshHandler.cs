using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public static class NavMeshHandler
{
    private static NavMeshSurface[] sceneNavMeshSurfaces = null;

    public static List<GameObject> objectsToDisableWhenBakingNavMeshes = new List<GameObject>();


    public static void BuildNavMesh(OVRSceneAnchor sceneAnchor)
    {
        NavMeshSurface navMeshSurface = sceneAnchor.gameObject.GetComponentInChildren<NavMeshSurface>();

        DeactivateNoNavMeshObjects();
        navMeshSurface.BuildNavMesh();
        ActivateNoNavMeshObjects();

        //Debug.Log($"NavMesh built for anchor {sceneAnchor.name}");
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
            DeactivateNoNavMeshObjects();
            surface.BuildNavMesh();
            ActivateNoNavMeshObjects();
        }
    }




    // Related to No Nav Mesh Objects
    public static void AddObjectToNoNavMeshList(GameObject go)
    {
        if (!objectsToDisableWhenBakingNavMeshes.Contains(go))
        {
            objectsToDisableWhenBakingNavMeshes.Add(go);
            //Debug.Log($"{go.name} added to no nav mesh objects");
        }
        else
        {
            //Debug.Log("The list already contains this object: no need to add it again.");
        }
    }

    public static void RemoveObjectFromNoNavMeshList(GameObject go)
    {
        if (objectsToDisableWhenBakingNavMeshes.Contains(go))
        {
            objectsToDisableWhenBakingNavMeshes.Remove(go);
            //Debug.Log($"{go.name} removed from no nav mesh objects");
        }
        else
        {
            //Debug.Log("The list does not contains this object: cant remove it.");
        }
    }

    private static void DeactivateNoNavMeshObjects()
    {
        foreach (GameObject go in objectsToDisableWhenBakingNavMeshes)
        {
            go.SetActive(false);
        }
    }

    private static void ActivateNoNavMeshObjects()
    {
        foreach (GameObject go in objectsToDisableWhenBakingNavMeshes)
        {
            go.SetActive(true);
        }
    }
}
