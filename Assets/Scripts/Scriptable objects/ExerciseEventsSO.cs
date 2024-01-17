using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExerciseEventSO : ScriptableObject
{
    public float time;
}

[CreateAssetMenu(menuName = "Exercise Events/Play Clip Exercise Event SO")]
public class PlayClipSO : BaseExerciseEventSO
{
    public bool waitForEndOfClip;
    public AudioClip clipToPlay;
}

public class SpawnObjectBaseSO : BaseExerciseEventSO
{
    public SpawnObjectOnSceneAnchor.AnchorTypes anchorType;
    public SpawnObjectOnSceneAnchor.SpawnSituation spawnSituation;
}

[CreateAssetMenu(menuName = "Exercise Events/Spawn Images Panel Exercise Event SO")]
public class SpawnImagesPanelSO : SpawnObjectBaseSO
{
    public ImagesPanel imagesPanel;
}

[CreateAssetMenu(menuName = "Exercise Events/Spawn Spider Exercise Event SO")]
public class SpawnSpiderSO : SpawnObjectBaseSO
{
    public SpiderPlaceholderClass spider;
    public float size = 1;
    public float speed = 1;
    public float angle = 0;
}
