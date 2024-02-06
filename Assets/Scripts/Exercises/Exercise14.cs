using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise14 : BaseExercise
{
    [SerializeField] private BaseExerciseEventSO playClipSpiderOnHandEvent;

    protected override void Awake()
    {
        base.Awake();
        SpiderHandInteract.SpiderOnHand.AddListener(SpiderOnHand);
    }

    private void SpiderOnHand()
    {
        FulfillConditionForExerciseEvent(playClipSpiderOnHandEvent);
        SpiderHandInteract.SpiderOnHand.RemoveListener(SpiderOnHand);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SpiderHandInteract.SpiderOnHand.RemoveListener(SpiderOnHand);
    }
}
