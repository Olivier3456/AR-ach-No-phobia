using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System.Linq;
using System;
using UnityEngine.Events;

public class NavMeshHandler : MonoBehaviour
{
    [SerializeField] private GameObject navMeshSurfacePrefab;
    [Space(15)]
    //[SerializeField] private float timeBetweenEachNavMeshBuild = 1f;
    //private float timer = 0;
    //private int index = -1;


    private static List<NavMeshSurface> navMeshSurfaces = new List<NavMeshSurface>();

    private void Awake()
    {
        MainManager.Instance.OnExerciseQuitted.AddListener(MainManager_OnExerciseQuitted);
    }
    private void OnDisable()
    {
        MainManager.Instance.OnExerciseQuitted.RemoveListener(MainManager_OnExerciseQuitted);
    }

    private void MainManager_OnExerciseQuitted()
    {
        for (int i = 0; i < navMeshSurfaces.Count; i++)
        {
            Destroy(navMeshSurfaces[i].gameObject);
        }

        navMeshSurfaces.Clear();
        Debug.Log("Exercise ended: all NavMesh surfaces have been destroyed");
    }


    // ===================== Apparently, their is no need to rebuild nav meshes periodically. =====================
    //public void Update()
    //{
    //    timer += Time.deltaTime;

    //    if (timer >= timeBetweenEachNavMeshBuild)
    //    {
    //        Debug.Log("Time to build a NavMesh.");

    //        timer = 0;



    //        if (navMeshSurfaces.Count == 0)
    //        {
    //            Debug.Log("navMeshSurfaces list is empty. Can't build NavMeshes");
    //        }
    //        else
    //        {
    //            Debug.Log($"{navMeshSurfaces.Count} elements in navMeshSurfaces List");
    //            index = index < navMeshSurfaces.Count - 1 ? ++index : 0;
    //            //navMeshSurfaces[index].BuildNavMesh();

    //            Debug.Log($"Built NavMesh for element {index} of navMeshSurfaces List");
    //        }
    //    }
    //}


    public void AddNavMeshSurface(OVRSceneAnchor sceneAnchor)
    {
        //NavMeshSurface navMeshSurface = sceneAnchor.gameObject.GetComponent<NavMeshSurface>();

        NavMeshSurface navMeshSurface = sceneAnchor.gameObject.GetComponentInChildren<NavMeshSurface>();

        if (navMeshSurface == null)
        {
            //navMeshSurface = sceneAnchor.gameObject.AddComponent<NavMeshSurface>();
            //navMeshSurface.minRegionArea = 0.25f;
            //navMeshSurface.collectObjects = CollectObjects.Children;   // TODO: TEST.


            NavMeshSurfaceAnchor anchor = sceneAnchor.transform.GetComponentInChildren<NavMeshSurfaceAnchor>();
            GameObject navMeshSurfaceGameObject = Instantiate(navMeshSurfacePrefab, anchor.transform);
            

            if (SceneAnchorHelper.GetAnchorType(sceneAnchor) == AnchorTypes.CEILING)
            {
                Debug.Log("Ceiling anchor type for nav mesh surface: inversing up axis");
                navMeshSurfaceGameObject.transform.up = -anchor.transform.forward;
            }
            else
            {
                navMeshSurfaceGameObject.transform.up = anchor.transform.forward;
                navMeshSurfaceGameObject.transform.forward = anchor.transform.up;
            }

            navMeshSurface = navMeshSurfaceGameObject.GetComponent<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
            navMeshSurfaces.Add(navMeshSurface);

            Debug.Log($"NavMesh surface has been added to anchor {sceneAnchor.name}");
        }
        else
        {
            Debug.Log($"Scene anchor {sceneAnchor.name} already has a NavMesh surface");
        }
    }


    //public void RemoveNavMeshSurface(OVRSceneAnchor sceneAnchor)
    //{
    //    NavMeshSurface navMeshSurface = sceneAnchor.gameObject.GetComponentInChildren<NavMeshSurface>();

    //    if (navMeshSurface != null)
    //    {
    //        navMeshSurfaces.Remove(navMeshSurface);
    //        Destroy(navMeshSurface);

    //        Debug.Log($"NavMesh surface has been removed from scene anchor {sceneAnchor.name}");
    //    }
    //    else
    //    {
    //        Debug.Log($"No NavMesh surface to remove from scene anchor {sceneAnchor.name}");
    //    }
    //}
}
