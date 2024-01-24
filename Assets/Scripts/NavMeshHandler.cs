using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshHandler : MonoBehaviour
{
    public enum SurfaceType { Ceiling, Floor_And_Table, Walls, All_Surfaces }

    public bool DEBUG_BuildAllNavMeshsAtStart = false;
    [Space(20)]
    [SerializeField] private OVRSceneManager ovrSceneManager;
    [Space(20), Tooltip("NavMesh Surfaces")]
    [SerializeField] private NavMeshSurface ceiling;
    [SerializeField] private NavMeshSurface floorAndTable;
    [SerializeField] private NavMeshSurface[] walls;
    [Space(20)]
    [SerializeField] private float timeBetweenEachNavMeshBuild = 1f;

    private List<NavMeshSurface> allNavMeshSurfaces = new List<NavMeshSurface>();
    private List<NavMeshSurface> activeNavMeshSurfaces = new List<NavMeshSurface>();

    private OVRSceneAnchor floorSceneAnchor = null;

    private void Awake()
    {
        ovrSceneManager.SceneModelLoadedSuccessfully += OnSceneLoaded;

        allNavMeshSurfaces.Add(ceiling);
        allNavMeshSurfaces.Add(floorAndTable);

        foreach (NavMeshSurface surface in walls)
        {
            allNavMeshSurfaces.Add(surface);
        }
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
                if (cla.Contains("FLOOR"))
                {
                    floorSceneAnchor = sceneAnchors[i];
                    Debug.Log("[NavMeshHandler] Floor scene anchor found in OVR scene anchors list.");
                    break;
                }
            }
        }
        if (floorSceneAnchor == null)
        {
            Debug.LogError("[NavMeshHandler] No floor scene anchor found in OVR scene anchors list!");
        }


        // ==================== DEBUG ======================
        //if (DEBUG_BuildAllNavMeshsAtStart)
        //    StartCoroutine(WaitALittleAndBuildNavMesh());
        // =================================================
    }

    private IEnumerator WaitALittleAndBuildNavMesh()
    {
        yield return new WaitForSeconds(2);

        StartBuildNavMesh(SurfaceType.All_Surfaces);
    }


    public void StartBuildNavMesh(SurfaceType surfaceType)
    {
        if (activeNavMeshSurfaces.Count > 0)
        {
            Debug.LogError("NavMesh build already started!");
            return;
        }

        switch (surfaceType)
        {
            case SurfaceType.Ceiling:
                activeNavMeshSurfaces.Add(ceiling);
                Debug.Log($"NavMesh will be built for ceiling every {timeBetweenEachNavMeshBuild} second");
                break;
            case SurfaceType.Floor_And_Table:
                activeNavMeshSurfaces.Add(floorAndTable);
                Debug.Log($"NavMesh will be built for floor and table every {timeBetweenEachNavMeshBuild} second");
                break;
            case SurfaceType.Walls:
                foreach (NavMeshSurface surface in walls)
                {
                    activeNavMeshSurfaces.Add(surface);
                    Debug.Log($"NavMesh will be built for wall every {timeBetweenEachNavMeshBuild} second");
                }
                break;
            case SurfaceType.All_Surfaces:
                foreach (NavMeshSurface surface in allNavMeshSurfaces)
                {
                    activeNavMeshSurfaces.Add(surface);
                }
                Debug.Log($"NavMesh will be built for all surfaces orientations every {timeBetweenEachNavMeshBuild} second");
                break;
        }

        // Initial build for all active NavMeshes
        foreach (NavMeshSurface surface in activeNavMeshSurfaces)
        {
            surface.BuildNavMesh();
        }

        StartCoroutine(BuildActiveNavMeshSurfacesRepetitively());
    }    

    private IEnumerator BuildActiveNavMeshSurfacesRepetitively()
    {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenEachNavMeshBuild);

        int index = 0;

        while (MainManager.Instance.GetCurrentExercise() != null)
        {
            yield return wait;

            // Align NavMesh surfaces parent object with the orientation of the room before rebuilding NavMeshes.
            transform.up = floorSceneAnchor.transform.forward;

            activeNavMeshSurfaces[index].BuildNavMesh();

            index = index < activeNavMeshSurfaces.Count - 1 ? ++index : 0;
        }

        Debug.Log($"Current exercise quitted: stopping to build nav mesh every {timeBetweenEachNavMeshBuild} second");
        activeNavMeshSurfaces.Clear();
    }
}
