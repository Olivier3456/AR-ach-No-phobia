using System;
using UnityEngine;

[Serializable]
public class ClipSubtitlesData
{
    public AudioClip clip;
    public Subtitle[] subtitles;
}
[Serializable]
public class Subtitle
{
    public string text;
    public float beginAt;
}

[CreateAssetMenu(menuName = "Audio/Audio Clip Subtitles SO")]
public class AudioClipSubtitlesSO : ScriptableObject
{
    public ClipSubtitlesData clipSubtitles;
}
