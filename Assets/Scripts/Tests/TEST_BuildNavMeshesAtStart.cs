//using System.Collections;
//using System.Collections.Generic;
//using System.Net.Mail;
//using UnityEngine;

//public class TEST_BuildNavMeshesAtStart : MonoBehaviour
//{
//    [SerializeField] private bool ceiling;
//    [SerializeField] private bool floor;
//    [SerializeField] private bool walls;
//    [SerializeField] private bool table;


//    private void OnEnable()
//    {
//        SceneAnchorHelper.OnSceneAnchorsFound.AddListener(OnSceneAnchorsFound);
//    }

//    private void OnDisable()
//    {
//        SceneAnchorHelper.OnSceneAnchorsFound.RemoveListener(OnSceneAnchorsFound);
//    }

//    private void OnSceneAnchorsFound()
//    {
//        StartCoroutine(WaitAndBuildNavMeshes());
//    }


//    IEnumerator WaitAndBuildNavMeshes()
//    {
//        yield return new WaitForSeconds(1);

//        Debug.Log("TEST Adding NavMesh surfaces to scene anchors");

//        if (ceiling && SceneAnchorHelper.CeilingSceneAnchor != null)
//        {
//            NavMeshHandler.BuildNavMesh(SceneAnchorHelper.CeilingSceneAnchor);
//        }
//        if (floor && SceneAnchorHelper.FloorSceneAnchor != null)
//        {
//            NavMeshHandler.BuildNavMesh(SceneAnchorHelper.FloorSceneAnchor);
//        }
//        if (walls && SceneAnchorHelper.WallsSceneAnchors.Count > 0)
//        {
//            foreach (OVRSceneAnchor sceneAnchor in SceneAnchorHelper.WallsSceneAnchors)
//            {
//                NavMeshHandler.BuildNavMesh(sceneAnchor);
//            }
//        }
//        if (table && SceneAnchorHelper.TableSceneAnchor != null)
//        {
//            NavMeshHandler.BuildNavMesh(SceneAnchorHelper.TableSceneAnchor);
//        }
//    }
//}
