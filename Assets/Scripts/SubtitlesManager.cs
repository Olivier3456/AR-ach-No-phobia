using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitlesManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI subtitlesText;
    [Space(20)]
    [SerializeField] private AudioClipSubtitlesSO[] audioClipSubtitlesSOs;
    

    private Coroutine subCoroutine = null;


    private void Start()
    {
        subtitlesText.text = "";
    }


    public void ShowSubtitles()
    {
        Subtitle[] clipSubs = null;

        foreach (var item in audioClipSubtitlesSOs)
        {
            if (item.clipSubtitles.clip == audioSource.clip)
            {
                clipSubs = item.clipSubtitles.subtitles;
                //Debug.Log($"Found subtitles data for clip {item.clipSubtitles.clip.name}.");
                break;
            }
        }

        if (clipSubs != null)
        {
            if (subCoroutine == null)
            {
                subCoroutine = StartCoroutine(ShowSubtitles_Coroutine(clipSubs));
            }
        }
        else
        {
            Debug.Log("No subtitles data found for the clip actually playing.");
        }
    }


    private IEnumerator ShowSubtitles_Coroutine(Subtitle[] clipSubs)
    {
        //Debug.Log("Beginning subtitle coroutine.");

        float timer = 0f;

        if (clipSubs.Length == 0)
        {
            yield break;
        }

        int subIndex = 0;
        subtitlesText.text = clipSubs[subIndex].text;

        //subtitlesText.gameObject.SetActive(true);

        while (audioSource.isPlaying)
        {
            yield return null;
            timer += Time.deltaTime;

            if (subIndex < clipSubs.Length - 1)
            {
                //Debug.Log("Looking for next subtitle time...");

                if (clipSubs[subIndex + 1].beginAt < timer)
                {
                    //Debug.Log("Next subtitle time reached. Displaying this subtitle.");

                    subIndex++;
                    subtitlesText.text = clipSubs[subIndex].text;

                    //Debug.Log($"Next subtitle displayed. Text is: {subtitlesText.text}");
                }
            }
        }

        subtitlesText.text = "";
        //subtitlesText.gameObject.SetActive(false);
        subCoroutine = null;
    }
}
