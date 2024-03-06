using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExerciseHandSpider : BaseExercise
{
    [SerializeField] private BaseExerciseEventSO playClipSpiderOnHandEvent;
    [SerializeField] private BaseExerciseEventSO playClipSpiderCanBeReleased;
    [SerializeField] private BaseExerciseEventSO spiderReleased;
    [Space(20)]
    [SerializeField] private float timeBeforeUserCanReleaseSpider = 10f;

    private bool isSpiderOnHand;
    private float spiderOnHandTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        SpiderHandInteract.SpiderOnHand.AddListener(SpiderOnHand);
        SpiderHandInteract.SpiderReleased.AddListener(SpiderReleased);
    }

    protected override void Update()
    {
        base.Update();

        if (isSpiderOnHand)
        {
            spiderOnHandTimer += Time.deltaTime;

            if (spiderOnHandTimer > timeBeforeUserCanReleaseSpider) 
            {
                SpiderCanBeReleased();
                SpiderHandInteract.CanBeReleasedByUser();
            }
        }
    }


    private void SpiderOnHand()
    {
        FulfillConditionForExerciseEvent(playClipSpiderOnHandEvent);
        SpiderHandInteract.SpiderOnHand.RemoveListener(SpiderOnHand);
        isSpiderOnHand = true;
    }

    private void SpiderCanBeReleased()
    {
        FulfillConditionForExerciseEvent(playClipSpiderCanBeReleased);
    }

    private void SpiderReleased()
    {
        FulfillConditionForExerciseEvent(spiderReleased);
        SpiderHandInteract.SpiderReleased.RemoveListener(SpiderReleased);
        isSpiderOnHand = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SpiderHandInteract.SpiderOnHand.RemoveListener(SpiderOnHand);
        SpiderHandInteract.SpiderReleased.RemoveListener(SpiderReleased);
    }
}
