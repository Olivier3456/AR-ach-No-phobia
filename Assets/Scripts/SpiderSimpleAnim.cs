using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderSimpleAnim : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject destinationVisualPrefab;
    [SerializeField] private float walkSpeed = 0.1f;
    [Space(20)]
    [SerializeField] private Animator animator;
    [SerializeField] private float walkAnimationSpeedFactor = 10f;

    private float minRemainingDistance = 0.1f;
    private GameObject destinationVisual;

    private OVRSceneAnchor sceneAnchor = null;


    private void Start()
    {
        agent.speed = walkSpeed;


        // DEBUG
        InitNavigationDebug();
    }



    // DEBUG
    [SerializeField] private Transform DEBUG_Destination;
    private void InitNavigationDebug()
    {
        agent.destination = DEBUG_Destination.position;
    }



    public void InitNavigation(OVRSceneAnchor sceneAnchor)
    {
        this.sceneAnchor = sceneAnchor;
        NavMeshHandler.BuildNavMesh(sceneAnchor);

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
        float currentVelocity = agent.velocity.magnitude;
        //Debug.Log($"Current velocity of spider agent: {currentVelocity}");
        animator.speed = currentVelocity * walkAnimationSpeedFactor;

        if (sceneAnchor != null)
        {
            if (!isPaused && agent.remainingDistance < minRemainingDistance)
            {
                Debug.Log("Setting new destination for Spider");
                SetRandomDestinationOnAnchorSurface();
                StartCoroutine(PauseAndResumeWalk());
            }
        }
    }


    private bool isPaused = false;
    private IEnumerator PauseAndResumeWalk()
    {
        isPaused = true;
        agent.speed = 0;
        float randomPauseTime = Random.Range(0.5f, 3f);
        yield return new WaitForSeconds(randomPauseTime);
        agent.speed = walkSpeed;
        isPaused = false;
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


    public void SetWalkSpeed(float value)
    {
        walkSpeed = value;
        agent.speed = value;
    }


    private void OnDestroy()
    {
        if (destinationVisual != null)
        {
            Destroy(destinationVisual);
        }
    }
}
