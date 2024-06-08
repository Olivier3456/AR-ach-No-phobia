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

    private float verticalOffsetFromHandAnchor = 0.04f;

    private static bool canSpiderBeReleased;
    private bool isSpiderReleased;

    public static UnityEvent SpiderOnHand = new UnityEvent();
    public static UnityEvent SpiderReleased = new UnityEvent();

    public override void InitSpider(OVRSceneAnchor sceneAnchor, SpawnSpiderSO spawnSpiderSO)
    {
        // =========> Same as base.InitSpider:

        base.sceneAnchor = sceneAnchor;

        walkSpeed = spawnSpiderSO.speed;
        agent.speed = 0;

        scale = spawnSpiderSO.scale;
        transform.localScale = Vector3.one * scale;

        walkAnimationSpeedFactor /= scale;



        // =========> Different from base.InitSpider:

        leftHandAnchor = MainManager.Instance.LeftPalmCenterMarker;
        rightHandAnchor = MainManager.Instance.RightPalmCenterMarker;

        InitNavigation();

        minRemainingDistance = 0.005f;

        canSpiderBeReleased = false;
    }


    protected override void InitNavigation()
    {
        NavMeshHandler.BuildNavMesh(sceneAnchor);
    }

    protected override void Update()
    {
        float currentVelocity = agent.velocity.magnitude;
        animator.speed = currentVelocity * walkAnimationSpeedFactor;

        if (!isSpiderOnHand && !isSpiderReleased)
        {
            if (currentAnchor != null && agent.remainingDistance < minRemainingDistance)
            {
                agent.speed = 0;
                agent.enabled = false;
                isSpiderOnHand = true;
                SpiderOnHand.Invoke();

                //Debug.Log("Spider is on hand!");
            }

            NavMeshHit hit;
            float margin = 0.05f;

            if (NavMesh.SamplePosition(leftHandAnchor.position, out hit, margin, NavMesh.AllAreas)) // Left hand is on the table: spider can walk towards it.
            {
                agent.SetDestination(hit.position);
                currentAnchor = leftHandAnchor;
                agent.speed = walkSpeed;
            }
            else if (NavMesh.SamplePosition(rightHandAnchor.position, out hit, margin, NavMesh.AllAreas)) // Right hand is on the table: spider can walk towards it.
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
        else
        {
            if (canSpiderBeReleased && !isSpiderReleased)   // Spider is on a hand and can be released: we must detect if the hand is near the table, for the spider to return on the table.
            {
                NavMeshHit hit;
                float margin = 0.05f;
                if (NavMesh.SamplePosition(currentAnchor.position, out hit, margin, NavMesh.AllAreas))
                {
                    agent.enabled = true;
                    transform.position = hit.position;  // Probably not necessary
                    agent.SetDestination(sceneAnchor.transform.position);
                    isSpiderReleased = true;
                    isSpiderOnHand = false;
                    SpiderReleased.Invoke();

                    //Debug.Log("Spider returned on the table");
                }
            }
        }

        if (isSpiderOnHand && !isSpiderReleased)    // Spider is on a hand: we place the spider manually on the correct hand anchor.
        {
            if (currentAnchor == rightHandAnchor) // Because left and right palm center don't have the same rotation.
            {
                transform.position = currentAnchor.position + (currentAnchor.up * verticalOffsetFromHandAnchor);

                Quaternion rotationOffset = Quaternion.Euler(0, 90, 0);
                transform.rotation = currentAnchor.rotation * rotationOffset;
            }
            else
            {
                transform.position = currentAnchor.position + (-currentAnchor.up * verticalOffsetFromHandAnchor);

                Quaternion invertedRotation = Quaternion.Euler(180, 90, 0);
                transform.rotation = currentAnchor.rotation * invertedRotation;
            }
        }
    }

    public static void CanBeReleasedByUser()
    {
        canSpiderBeReleased = true;
    }
}
