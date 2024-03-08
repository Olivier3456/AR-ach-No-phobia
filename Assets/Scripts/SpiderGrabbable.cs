using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class SpiderGrabbable : BaseSpider
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private HandGrabInteractable handGrabInteractable;

    private bool isThrowed = false;


    protected override void OnDestroy()
    {
        base.OnDestroy();
        handGrabInteractable.WhenStateChanged -= HandGrabInteractable_WhenStateChanged;
    }

    private void HandGrabInteractable_WhenStateChanged(InteractableStateChangeArgs obj)
    {
        if (obj.NewState == InteractableState.Select)
        {
            OnGrab();
        }
        else
        {
            OnThrow();
        }
    }

    public void OnGrab()
    {
        rb.useGravity = false;
        agent.enabled = false;

        Debug.Log("Spider grabbed!");
    }
    public void OnThrow()
    {
        rb.useGravity = true;
        isThrowed = true;
        NavMeshHandler.BuildAllNavMeshes(true);

        Debug.Log("Spider released!");
    }



    public override void InitSpider(OVRSceneAnchor sceneAnchor, SpawnSpiderSO spawnSpiderSO)
    {
        this.sceneAnchor = sceneAnchor;

        walkSpeed = spawnSpiderSO.speed;
        agent.speed = walkSpeed;

        scale = spawnSpiderSO.scale;
        transform.localScale = Vector3.one * scale;

        walkAnimationSpeedFactor /= scale;

        InitNavigation();

        handGrabInteractable.WhenStateChanged += HandGrabInteractable_WhenStateChanged;

        //StartCoroutine(PauseWalkRepetitively());
    }


    protected override void Update()
    {
        if (isThrowed)
        {
            NavMeshHit hit;
            float margin = 0.2f;

            if (NavMesh.SamplePosition(transform.position, out hit, margin, NavMesh.AllAreas))
            {
                isThrowed = false;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                agent.enabled = true;
            }
        }

        if (agent.enabled == true)
        {
            if (agent.remainingDistance > minRemainingDistance)
            {
                hasChangedDestination = false;
            }

            base.Update();
        }
    }
}
