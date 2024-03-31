using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciceGrabSpider : BaseExercise
{
    [SerializeField] private GameObject spiderBoxPrefab;
    [SerializeField] private BaseExerciseEventSO playClipSpiderInBox;



    protected override void Awake()
    {
        base.Awake();

        SpiderGrabbable.SpiderInBox.AddListener(SpiderInBox);
    }
    


    private void SpiderInBox()
    {
        FulfillConditionForExerciseEvent(playClipSpiderInBox);
    }



    protected override void OnDestroy()
    {
        base.OnDestroy();
        SpiderGrabbable.SpiderInBox.RemoveListener(SpiderInBox);
    }
}
