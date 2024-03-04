using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseSpider : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected GameObject destinationVisualPrefab;
    [Space(20)]
    [SerializeField] protected Animator animator;
    [SerializeField] protected float walkAnimationSpeedFactor = 10f;

    protected float minRemainingDistance = 0.1f;
    protected GameObject destinationVisual;

    protected OVRSceneAnchor sceneAnchor = null;

    protected bool isPaused = false;

    protected float walkSpeed = 0.1f;
    protected float scale = 1f;


    public virtual void InitSpider(OVRSceneAnchor sceneAnchor, SpawnSpiderSO spawnSpiderSO)
    {
        this.sceneAnchor = sceneAnchor;

        walkSpeed = spawnSpiderSO.speed;
        agent.speed = walkSpeed;

        scale = spawnSpiderSO.scale;
        transform.localScale = Vector3.one * scale;

        walkAnimationSpeedFactor /= scale;

        InitNavigation();
        StartCoroutine(PauseWalkRepetitively());
    }


    protected virtual void InitNavigation()
    {
        NavMeshHandler.BuildNavMesh(sceneAnchor);

        if (destinationVisualPrefab != null)
        {
            destinationVisual = Instantiate(destinationVisualPrefab);
            Debug.Log("Instantiated visual marker for spider's destination");
        }

        SetRandomDestinationOnAnchorSurface();
        Debug.Log("First spider destination is set");
    }


    protected virtual void Update()
    {
        float currentVelocity = agent.velocity.magnitude;
        animator.speed = currentVelocity * walkAnimationSpeedFactor;

        if (sceneAnchor != null)
        {
            if (!isPaused && agent.remainingDistance < minRemainingDistance)
            {
                Debug.Log("Setting new destination for Spider");
                SetRandomDestinationOnAnchorSurface();
                isPaused = true;
            }
        }

        if (isPaused)
        {
            agent.speed = 0;
        }
        else
        {
            agent.speed = walkSpeed;
        }
    }


    private IEnumerator PauseWalkRepetitively()
    {
        while (true)
        {
            isPaused = false;
            float randomWalkTime = Random.Range(5, 10);
            float timer = 0;
            while (!isPaused && timer < randomWalkTime)
            {
                yield return null;
                timer += Time.deltaTime;
            }

            isPaused = true;
            float randomPauseTime = Random.Range(1, 3);
            timer = 0;
            while (isPaused && timer < randomPauseTime)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }


    private void SetRandomDestinationOnAnchorSurface()
    {
        bool shouldContinue = true;
        Vector3 destination;
        int iteration = 0;
        int maxIterations = 100;

        do
        {
            iteration++;
            destination = SceneAnchorHelper.RandomPointOnAnchorSurface(sceneAnchor);

            if (Vector3.Distance(destination, transform.position) > minRemainingDistance * 1.5f)
            {
                // We want to limit the angle from last trajectory to 90°:
                Vector3 directionToCurrentTarget = (agent.destination - transform.position).normalized;
                Vector3 directionToNextPossibleTarget = (destination - transform.position).normalized;

                if (Vector3.Dot(directionToCurrentTarget, directionToNextPossibleTarget) > 0)
                {
                    shouldContinue = false;
                }
            }

        } while (shouldContinue && iteration < maxIterations);

        Debug.Log($"New destination found at iteration {iteration}. Maximum allowed was {maxIterations}.");

        agent.SetDestination(destination);

        if (destinationVisual != null)
        {
            destinationVisual.transform.position = destination;
        }
    }


    public virtual void SetWalkSpeed(float value)
    {
        walkSpeed = value;
    }


    protected virtual void OnDestroy()
    {
        if (destinationVisual != null)
        {
            Destroy(destinationVisual);
        }
    }
}
