using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshHandler : MonoBehaviour
{
    public enum SurfaceType { Ceiling, Floor_And_Table, Walls, All_Surfaces }

    [SerializeField] private OVRSceneManager ovrSceneManager;
    [Space(20), Tooltip("NavMesh Surfaces")]
    [SerializeField] private NavMeshSurface ceiling;
    [SerializeField] private NavMeshSurface floorAndTable;
    [SerializeField] private NavMeshSurface[] walls;

    private List<NavMeshSurface> allNavMeshSurfaces = new List<NavMeshSurface>();

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
        // ==================== DEBUG ======================
        StartCoroutine(WaitALittleAndBuildNavMesh());
        // =================================================
    }

    private IEnumerator WaitALittleAndBuildNavMesh()
    {
        yield return new WaitForSeconds(2);

        BuildNavMesh(SurfaceType.All_Surfaces);
    }


    public void BuildNavMesh(SurfaceType surfaceType)
    {
        switch (surfaceType)
        {
            case SurfaceType.Ceiling:
                ceiling.BuildNavMesh();
                Debug.Log("NavMesh built for ceiling");
                break;
            case SurfaceType.Floor_And_Table:
                floorAndTable.BuildNavMesh();
                Debug.Log("NavMesh built for floor and table");
                break;
            case SurfaceType.Walls:
                foreach (NavMeshSurface surface in walls)
                {
                    surface.BuildNavMesh();
                    Debug.Log("NavMesh built for wall");
                }
                break;
            case SurfaceType.All_Surfaces:
                foreach (NavMeshSurface surface in allNavMeshSurfaces)
                {
                    surface.BuildNavMesh();
                }
                Debug.Log("NavMeshs builts for all surfaces orientations");
                break;
        }
    }
}
