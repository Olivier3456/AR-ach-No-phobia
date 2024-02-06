using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderHandInteract : BaseSpider
{
    private Transform leftHandAnchor;
    private Transform rightHandAnchor;

    private Transform currentAnchor;

    private bool isSpiderOnHand;

    public override void InitSpider(OVRSceneAnchor sceneAnchor, SpawnSpiderSO spawnSpiderSO)
    {
        base.sceneAnchor = sceneAnchor;

        walkSpeed = spawnSpiderSO.speed;
        agent.speed = 0;

        scale = spawnSpiderSO.scale;
        transform.localScale = Vector3.one * scale;

        walkAnimationSpeedFactor /= scale;

        InitNavigation();
    }


    protected override void InitNavigation()
    {
        NavMeshHandler.BuildNavMesh(sceneAnchor);
    }

    protected override void Update()
    {

        if (!isSpiderOnHand)
        {
            NavMeshHit hit;
            float margin = 0.05f;

            if (NavMesh.SamplePosition(leftHandAnchor.position, out hit, margin, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                currentAnchor = leftHandAnchor;
                agent.speed = walkSpeed;
            }
            else if (NavMesh.SamplePosition(rightHandAnchor.position, out hit, margin, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                currentAnchor = rightHandAnchor;
                agent.speed = walkSpeed;
            }
            else
            {
                agent.speed = 0;
            }

            if (agent.remainingDistance < minRemainingDistance)
            {
                transform.position = currentAnchor.position;
                transform.parent = currentAnchor;
                agent.speed = 0;
                agent.isStopped = true;
                isSpiderOnHand = true;
            }
        }
    }
}
