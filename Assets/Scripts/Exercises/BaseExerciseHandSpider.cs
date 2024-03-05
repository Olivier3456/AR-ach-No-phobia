using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExerciseHandSpider : BaseExercise
{
    [SerializeField] private BaseExerciseEventSO playClipSpiderOnHandEvent;
    [SerializeField] private BaseExerciseEventSO playClipSpiderCanBeReleased;
    [SerializeField] private BaseExerciseEventSO spiderReleased;

    protected override void Awake()
    {
        base.Awake();
        SpiderHandInteract.SpiderOnHand.AddListener(SpiderOnHand);
        SpiderHandInteract.SpiderCanBeReleased.AddListener(SpiderCanBeReleased);
        SpiderHandInteract.SpiderReleased.AddListener(SpiderReleased);
    }

    private void SpiderOnHand()
    {
        FulfillConditionForExerciseEvent(playClipSpiderOnHandEvent);
        SpiderHandInteract.SpiderOnHand.RemoveListener(SpiderOnHand);
    }

    private void SpiderCanBeReleased()
    {
        FulfillConditionForExerciseEvent(playClipSpiderCanBeReleased);
        SpiderHandInteract.SpiderCanBeReleased.RemoveListener(SpiderCanBeReleased);
    }

    private void SpiderReleased()
    {
        FulfillConditionForExerciseEvent(spiderReleased);
        SpiderHandInteract.SpiderReleased.RemoveListener(SpiderReleased);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SpiderHandInteract.SpiderOnHand.RemoveListener(SpiderOnHand);
        SpiderHandInteract.SpiderCanBeReleased.RemoveListener(SpiderCanBeReleased);
        SpiderHandInteract.SpiderReleased.RemoveListener(SpiderReleased);
    }
}
