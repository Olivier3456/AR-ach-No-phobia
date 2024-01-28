using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderSimpleAnim : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject destinationVisualPrefab;
    [Space(20)]
    [SerializeField] private Animator animator;
    [SerializeField] private float walkAnimationSpeedFactor = 10f;

    private float minRemainingDistance = 0.1f;
    private GameObject destinationVisual;

    private OVRSceneAnchor sceneAnchor = null;

    private bool isPaused = false;

    private float walkSpeed = 0.1f;
    private float scale = 1f;


    public void InitSpider(OVRSceneAnchor sceneAnchor, SpawnSpiderSO spawnSpiderSO)
    {
        walkSpeed = spawnSpiderSO.speed;
        agent.speed = walkSpeed;

        scale = spawnSpiderSO.scale;
        transform.localScale = Vector3.one * scale;

        walkAnimationSpeedFactor /= scale;

        InitNavigation(sceneAnchor);
        StartCoroutine(PauseWalkRepetitively());
    }


    private void InitNavigation(OVRSceneAnchor sceneAnchor)
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
    }


    private void OnDestroy()
    {
        if (destinationVisual != null)
        {
            Destroy(destinationVisual);
        }
    }
}
