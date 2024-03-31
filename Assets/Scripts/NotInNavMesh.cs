using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotInNavMesh : MonoBehaviour
{
    void Start()
    {
        NavMeshHandler.AddObjectToNoNavMeshList(gameObject);
    }


    private void OnDestroy()
    {
        NavMeshHandler.RemoveObjectFromNoNavMeshList(gameObject);
    }
}
