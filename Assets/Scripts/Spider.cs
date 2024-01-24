using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spider : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private OVRSceneAnchor sceneAnchor;    

    public void SetSceneAnchor(OVRSceneAnchor sceneAnchor)
    {
        this.sceneAnchor = sceneAnchor;       
    }

    private void Start()
    {
        agent.SetDestination(FindRandomDestination());
    }

    private void Update()
    {
        if (agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(FindRandomDestination());
        }
    }

    private Vector3 FindRandomDestination()
    {
        return SceneAnchorHelper.FindRandomPointOnAnchor(sceneAnchor);
    }





    //private Transform parentObject;
    //public bool maintainScaleOf1 = true;


    //private void Start()
    //{
    //    parentObject = transform.parent;

    //    Debug.Log($"parent transform is: {parentObject.name}");
    //}

    //private void LateUpdate()
    //{
    //    if (maintainScaleOf1)
    //    {
    //        transform.localScale = new Vector3(1 / parentObject.localScale.x,
    //                                           1 / parentObject.localScale.y,
    //                                           1 / parentObject.localScale.z);
    //    }
    //}

}
