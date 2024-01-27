using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Oculus.Platform.Models;

public class Spider : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject destinationVisualPrefab;

    private float minRemainingDistance = 0.1f;
    private GameObject destinationVisual;

    private OVRSceneAnchor sceneAnchor = null;

    
    public void InitNavigation(OVRSceneAnchor sceneAnchor)
    {
        this.sceneAnchor = sceneAnchor;
        MainManager.Instance.NavMeshHandler.AddNavMeshSurface(this.sceneAnchor);

        if (destinationVisualPrefab != null)
        {
            destinationVisual = Instantiate(destinationVisualPrefab);
            Debug.Log("Instantiated visual marker for spider's destination");
        }

        SetRandomDestinationOnAnchorSurface();
        Debug.Log("First spider destination is set");
    }


    private void Update()
    {
        if (sceneAnchor != null)
        {
            if (agent.remainingDistance < minRemainingDistance)
            {
                Debug.Log("Setting new destination for Spider");
                SetRandomDestinationOnAnchorSurface();
            }
        }
    }


    private void SetRandomDestinationOnAnchorSurface()
    {
        Vector3 destination = SceneAnchorHelper.RandomPointOnAnchorSurface(sceneAnchor);
        agent.SetDestination(destination);
        if (destinationVisual != null)
        {
            destinationVisual.transform.position = destination;
        }
    }


    private void OnDestroy()
    {
        if (destinationVisual != null)
        {
            Destroy(destinationVisual);
        }
    }
}
