using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class TEST_BuildNavMeshesAtStart : MonoBehaviour
{
    [SerializeField] private NavMeshHandler navMeshHandler;

    [SerializeField] private bool ceiling;
    [SerializeField] private bool floor;
    [SerializeField] private bool walls;
    [SerializeField] private bool table;


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
        Debug.Log("TEST Adding NavMesh surfaces to scene anchors");

        if (ceiling && SceneAnchorHelper.CeilingSceneAnchor != null)
        {
            navMeshHandler.AddNavMeshSurface(SceneAnchorHelper.CeilingSceneAnchor);
        }
        if (floor && SceneAnchorHelper.FloorSceneAnchor != null)
        {
            navMeshHandler.AddNavMeshSurface(SceneAnchorHelper.FloorSceneAnchor);
        }
        if (walls && SceneAnchorHelper.WallsSceneAnchors.Count > 0)
        {
            foreach (OVRSceneAnchor sceneAnchor in SceneAnchorHelper.WallsSceneAnchors)
            {
                navMeshHandler.AddNavMeshSurface(sceneAnchor);
            }
        }
        if (table && SceneAnchorHelper.TableSceneAnchor != null)
        {
            navMeshHandler.AddNavMeshSurface(SceneAnchorHelper.TableSceneAnchor);
        }
    }
}
