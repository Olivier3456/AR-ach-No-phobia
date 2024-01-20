using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exercise1 : BaseExercise
{
    [SerializeField] private BaseExerciseEventSO endExerciseEvent;

    protected override void Update()
    {
        base.Update();

        if (imagesPanel != null)
        {
            if (imagesPanel.IsLastSprite())
            {
                FulfillConditionForExerciseEvent(endExerciseEvent);
            }
        }
    }
}
