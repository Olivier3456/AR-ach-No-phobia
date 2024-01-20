using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseExercise : MonoBehaviour
{
    [SerializeField] protected AudioSource exerciseAudioSource;
    [Space(15)]
    [SerializeField] protected BaseExerciseEventSO[] exerciseEvents;

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



        else if (eventSO is PlayClipSO)
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



        else if (eventSO is SpawnImagesPanelSO)
        {
            SpawnImagesPanelSO spawnIPSO = eventSO as SpawnImagesPanelSO;
            allObjectsSpawned.Add(MainManager.Instance.GetSpawnObjectOnSceneAnchor().SpawnObjectOnAnchorOfType(spawnIPSO.imagesPanel.gameObject,
                                                                                                               spawnIPSO.anchorType,
                                                                                                               spawnIPSO.spawnSituation));
            imagesPanel = allObjectsSpawned[allObjectsSpawned.Count - 1].GetComponent<ImagesPanel>();
            SwitchToNextEvent();
        }



        else if (eventSO is SpawnSpiderSO)
        {
            SpawnSpiderSO spawnSpiderSO = eventSO as SpawnSpiderSO;




            SwitchToNextEvent();
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
