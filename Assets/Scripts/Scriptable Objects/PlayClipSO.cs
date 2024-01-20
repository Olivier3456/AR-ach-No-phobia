using UnityEngine;

[CreateAssetMenu(menuName = "Exercise Events/Play Clip Exercise Event SO")]
public class PlayClipSO : BaseExerciseEventSO
{
    public bool waitForEndOfClip;
    public AudioClip clipToPlay;
}
