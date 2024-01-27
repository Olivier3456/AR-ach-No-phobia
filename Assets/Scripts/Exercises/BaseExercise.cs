using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExercise : MonoBehaviour
{
    [SerializeField] protected AudioSource exerciseAudioSource;
    [Space(15)]
    [SerializeField] protected BaseExerciseEventSO[] exerciseEvents;
    [Space(15)]

    protected int currentEventIndex = 0;
    protected float currentEventTimer = 0f;
    protected bool isInProgress = true;
    protected List<GameObject> allObjectsSpawned = new List<GameObject>();
    protected ImagesPanel imagesPanel = null;
    protected int id;
    protected bool isPlayingCurrentPlayClipEvent = false;
    protected List<BaseExerciseEventSO> exercisesEventsWithBeginingConditionFullfiled = new List<BaseExerciseEventSO>();

    public bool IsInProgress { get { return isInProgress; } }
    public List<GameObject> AllObjectsSpawned { get { return allObjectsSpawned; } }
    public ImagesPanel ImagesPanel { get { return imagesPanel; } }
    public int Id { get { return id; } }
    public BaseExerciseEventSO CurrentEvent { get { return exerciseEvents[currentEventIndex]; } }
    public int CurrentEventIndex { get { return currentEventIndex; } }


    public void SetExerciseId(int id)
    {
        this.id = id;
        Debug.Log($"[BaseExercise] Exercise id: {id}.");
    }


    protected virtual void Awake()
    {
        if (exerciseAudioSource == null)
        {
            exerciseAudioSource = GetComponent<AudioSource>();
        }
    }



    protected virtual void Update()
    {
        if (!isInProgress)
        {
            return;
        }

        BaseExerciseEventSO eventSO = exerciseEvents[currentEventIndex];

        if (eventSO.beginWithCondition)
        {
            if (!exercisesEventsWithBeginingConditionFullfiled.Contains(eventSO))
            {
                return;
            }
        }



        if (eventSO is WaitSO)
        {
            HandleWaitEvent(eventSO);
        }
        else if (eventSO is PlayClipSO)
        {
            HandlePlayClipEvent(eventSO);
        }
        else if (eventSO is SpawnImagesPanelSO)
        {
            HandleSpawnImagesPanelEvent(eventSO);
        }
        else if (eventSO is SpawnSpiderSO)
        {
            HandleSpawnSpiderEvent(eventSO);
        }
    }



    private void HandleSpawnSpiderEvent(BaseExerciseEventSO eventSO)
    {
        SpawnSpiderSO spawnSpiderSO = eventSO as SpawnSpiderSO;
        allObjectsSpawned.Add(MainManager.Instance.SpawnObjectOnSceneAnchor.SpawnObjectOnAnchorOfType(spawnSpiderSO.spider.gameObject,
                                                                                                      spawnSpiderSO.anchorType,
                                                                                                      spawnSpiderSO.spawnSituation,
                                                                                                      out OVRSceneAnchor sceneAnchor));
        Spider spider = allObjectsSpawned[allObjectsSpawned.Count - 1].GetComponent<Spider>();
        spider.InitNavigation(sceneAnchor);

        SwitchToNextEvent();
    }

    private void HandleSpawnImagesPanelEvent(BaseExerciseEventSO eventSO)
    {
        SpawnImagesPanelSO spawnIPSO = eventSO as SpawnImagesPanelSO;
        allObjectsSpawned.Add(MainManager.Instance.SpawnObjectOnSceneAnchor.SpawnObjectOnAnchorOfType(spawnIPSO.imagesPanel.gameObject,
                                                                                                      spawnIPSO.anchorType,
                                                                                                      spawnIPSO.spawnSituation,
                                                                                                      out OVRSceneAnchor sceneAnchor));
        imagesPanel = allObjectsSpawned[allObjectsSpawned.Count - 1].GetComponent<ImagesPanel>();

        SwitchToNextEvent();
    }

    private void HandlePlayClipEvent(BaseExerciseEventSO eventSO)
    {
        PlayClipSO playClipSO = eventSO as PlayClipSO;

        if (!isPlayingCurrentPlayClipEvent)
        {
            exerciseAudioSource.clip = playClipSO.clipToPlay;
            exerciseAudioSource.Play();

            if (playClipSO.waitForEndOfClip)
            {
                isPlayingCurrentPlayClipEvent = true;
            }
            else
            {
                SwitchToNextEvent();
            }
        }
        else
        {
            if (!exerciseAudioSource.isPlaying)
            {
                isPlayingCurrentPlayClipEvent = false;
                exerciseAudioSource.clip = null;
                SwitchToNextEvent();
            }
        }
    }

    private void HandleWaitEvent(BaseExerciseEventSO eventSO)
    {
        WaitSO waitSO = eventSO as WaitSO;

        if (waitSO.waitTime <= currentEventTimer)
        {
            currentEventTimer = 0f;
            SwitchToNextEvent();
        }
        else
        {
            currentEventTimer += Time.deltaTime;
        }
    }

    protected virtual void SwitchToNextEvent()
    {
        if (currentEventIndex < exerciseEvents.Length - 1)
        {
            currentEventIndex++;
        }
        else
        {
            if (isInProgress)
            {
                isInProgress = false;
                Debug.Log("[BaseExercise] Current exercise is finished.");
            }
        }
    }

    public void FulfillConditionForExerciseEvent(BaseExerciseEventSO exerciseEventSO)
    {
        if (!exercisesEventsWithBeginingConditionFullfiled.Contains(exerciseEventSO))
        {
            exercisesEventsWithBeginingConditionFullfiled.Add(exerciseEventSO);
        }
    }



    protected virtual void OnDestroy()
    {
        for (int i = 0; i < allObjectsSpawned.Count; i++)
        {
            Destroy(allObjectsSpawned[i]);
        }
    }
}
