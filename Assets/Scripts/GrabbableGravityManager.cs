using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

using UnityEngine;

public class GrabbableGravityManager : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    //[SerializeField] private SphereCollider col;
    [SerializeField] private HandGrabInteractable handGrabInteractable;

    private void Start()
    {
        handGrabInteractable.WhenStateChanged += HandGrabInteractable_WhenStateChanged;
    }
    private void OnDestroy()
    {
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

    public void OnThrow()
    {
        rb.useGravity = true;
        //col.isTrigger = false;

        //Debug.Log("Object released!");
    }

    public void OnGrab()
    {
        rb.useGravity = false;
        //col.isTrigger = true;

        //Debug.Log("Object grabbed!");
    }
}
