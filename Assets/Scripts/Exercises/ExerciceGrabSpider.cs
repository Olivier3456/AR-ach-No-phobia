using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciceGrabSpider : BaseExercise
{
    [SerializeField] private GameObject spiderBoxPrefab;
    [SerializeField] private BaseExerciseEventSO playClipSpiderGrabbed;
    [SerializeField] private BaseExerciseEventSO playClipSpiderInBox;



    protected override void Awake()
    {
        base.Awake();

        SpiderGrabbable.SpiderInBox.AddListener(SpiderInBox);
        SpiderGrabbable.SpiderGrabbed.AddListener(SpiderGrabbed);
    }




    private void SpiderGrabbed()
    {
        FulfillConditionForExerciseEvent(playClipSpiderGrabbed);
    }



    private void SpiderInBox()
    {
        FulfillConditionForExerciseEvent(playClipSpiderInBox);
    }



    protected override void OnDestroy()
    {
        base.OnDestroy();
        SpiderGrabbable.SpiderInBox.RemoveListener(SpiderInBox);
        SpiderGrabbable.SpiderGrabbed.RemoveListener(SpiderGrabbed);
    }
}
