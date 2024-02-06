using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SpiderHandInteract : BaseSpider
{
    private Transform leftHandAnchor;
    private Transform rightHandAnchor;

    private Transform currentAnchor = null;

    private bool isSpiderOnHand;

    public static UnityEvent SpiderOnHand = new UnityEvent();

    public override void InitSpider(OVRSceneAnchor sceneAnchor, SpawnSpiderSO spawnSpiderSO)
    {
        base.sceneAnchor = sceneAnchor;

        walkSpeed = spawnSpiderSO.speed;
        agent.speed = 0;

        scale = spawnSpiderSO.scale;
        transform.localScale = Vector3.one * scale;

        walkAnimationSpeedFactor /= scale;

        leftHandAnchor = MainManager.Instance.LeftPalmCenterMarker;
        rightHandAnchor = MainManager.Instance.RightPalmCenterMarker;

        InitNavigation();

        Debug.Log("Spider init done");
    }


    protected override void InitNavigation()
    {
        NavMeshHandler.BuildNavMesh(sceneAnchor);
    }

    protected override void Update()
    {
        float currentVelocity = agent.velocity.magnitude;
        animator.speed = currentVelocity * walkAnimationSpeedFactor;

        if (!isSpiderOnHand)
        {
            if (currentAnchor != null && agent.remainingDistance < minRemainingDistance)
            {
                agent.speed = 0;
                agent.enabled = false;
                transform.position = currentAnchor.position;
                transform.parent = currentAnchor;
                transform.up = currentAnchor.up;

                isSpiderOnHand = true;
                SpiderOnHand.Invoke();

                Debug.Log("Spider is on hand!");
            }

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
                currentAnchor = null;
            }
        }
    }
}
