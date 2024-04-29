using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SpiderGrabbable : BaseSpider
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private HandGrabInteractable handGrabInteractable;


    public static UnityEvent SpiderGrabbed = new UnityEvent();
    public static UnityEvent SpiderInBox = new UnityEvent();

    private bool isThrowed = false;

    private const string BOX_TAG = "BoxForSpider";

    private bool isInBox = false;


    private bool canEnterInBox = false;

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
        float animatorSpeedMultiplierWhenSpiderIsGrabbed = 5f;   // The spider wriggles when the user catches it.
        animator.speed *= animatorSpeedMultiplierWhenSpiderIsGrabbed;

        SpiderGrabbed.Invoke();

        //Debug.Log("Spider grabbed!");
    }
    public void OnThrow()
    {
        rb.useGravity = true;
        isThrowed = true;

        NavMeshHandler.BuildAllNavMeshes(true, true);

        //Debug.Log("Spider released!");

        StartCoroutine(ManageCanEnterInBoxValue_Coroutine());
    }

    private IEnumerator ManageCanEnterInBoxValue_Coroutine() // We need a small delay before setting canEnterInBox to false, otherwise it may always be false when OnTriggerEnter, even if spider just throwed by player.
    {
        canEnterInBox = true;
        yield return new WaitForSeconds(0.5f);
        canEnterInBox = false;
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
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(BOX_TAG) && canEnterInBox)
        {
            SpiderInBox.Invoke();
            agent.speed = 0f;
            animator.speed = 0f;
            isInBox = true;
            Vector3 verticalOffset = new Vector3(0f, -0.125f, 0f);
            transform.position = other.transform.position + verticalOffset;
            handGrabInteractable.Disable(); // Spider can't be grabbed anymore after been entered in its box.

            //Debug.Log("Spider is in a spider box.");
        }
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

                NavMeshHandler.BuildNavMesh(sceneAnchor, false);    // Rebuilding the table navmesh with the spider box on it, to avoid spider to walk through the box.
            }
        }

        if (agent.enabled == true)
        {
            if (agent.remainingDistance > minRemainingDistance)
            {
                hasChangedDestination = false;
            }

            if (!isInBox)
            {
                base.Update();
            }
        }
    }
}
