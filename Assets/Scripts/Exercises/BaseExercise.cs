using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseExercise.ExerciseEvent;

public class BaseExercise : MonoBehaviour
{
    [Serializable]
    public class ExerciseEvent
    {
        [Serializable]
        public class ObjectToSpawnData
        {
            public SpawnObjectOnSceneAnchor.AnchorTypes anchorType;
            public SpawnObjectOnSceneAnchor.SpawnSituation spawnSituation;
            public GameObject prefab;
        }

        [Serializable]
        public class SpiderToSpawnData
        {
            public SpawnObjectOnSceneAnchor.AnchorTypes anchorType;
            public SpawnObjectOnSceneAnchor.SpawnSituation spawnSituation;
            public SpiderPlaceholderClass spider;
            public float size = 1;
            public float speed = 1;
            public float angle = 0;
        }


        public float time = 0;
        [Tooltip("None if no sound to play")] public AudioClip clipToPlay;
        [Tooltip("Empty if no object to spawn")] public ObjectToSpawnData[] objectsToSpawn;
        [Tooltip("Empty if no spider to spawn")] public SpiderToSpawnData[] spidersToSpawn;
    }

    [SerializeField] private bool isTimed = true;
    [SerializeField] private float exerciceLength = 30f;
    [Space(15)]
    [SerializeField] private ExerciseEvent[] exerciseEvents;

    private float exerciseTimer = 0;
    private int currentEventIndex = 0;
    private bool isInProgress = true;
    private List<GameObject> allObjectsSpawned = new List<GameObject>();
    private ImagesPanel imagesPanel = null;
    private int id;
    private bool isLastEventReached = false;

    public float ExerciseTimer { get { return exerciseTimer; } }
    public float ExerciceLength { get { return exerciceLength; } }
    public bool IsInProgress { get { return isInProgress; } }
    public List<GameObject> AllObjectsSpawned { get { return allObjectsSpawned; } }
    public ImagesPanel ImagesPanel { get { return imagesPanel; } }
    public int Id { get { return id; } }


    void Update()
    {
        exerciseTimer += Time.deltaTime;

        if (!isLastEventReached && exerciseEvents[currentEventIndex].time <= exerciseTimer)
        {
            Debug.Log($"[BaseExercise] exercise event {currentEventIndex} reached at {exerciseTimer} seconds.");

            if (exerciseEvents[currentEventIndex].clipToPlay != null)
            {
                AudioSource.PlayClipAtPoint(exerciseEvents[currentEventIndex].clipToPlay, Camera.main.transform.position);
            }

            if (exerciseEvents[currentEventIndex].objectsToSpawn.Length > 0)
            {
                for (int i = 0; i < exerciseEvents[currentEventIndex].objectsToSpawn.Length; i++)
                {
                    ObjectToSpawnData objectToSpawnData = exerciseEvents[currentEventIndex].objectsToSpawn[i];
                    allObjectsSpawned.Add(MainManager.Instance.GetSpawnObjectOnSceneAnchor().SpawnObjectOnAnchorOfType(objectToSpawnData.prefab,
                                                                                                                       objectToSpawnData.anchorType,
                                                                                                                       objectToSpawnData.spawnSituation));

                    if (allObjectsSpawned[allObjectsSpawned.Count - 1].TryGetComponent(out ImagesPanel imagesPanel))
                    {
                        this.imagesPanel = imagesPanel;

                        Debug.Log("[BaseExercise] The object instantiated is an images panel.");
                    }
                }
            }

            if (exerciseEvents[currentEventIndex].spidersToSpawn.Length > 0)
            {
                // TODO: Spawn spider(s)
                //allObjectsSpawned.Add(.........


            }


            if (currentEventIndex < exerciseEvents.Length - 1)
            {
                currentEventIndex++;
            }
            else
            {
                isLastEventReached = true;
            }
        }


        if (isTimed && isInProgress && exerciseTimer >= exerciceLength)
        {
            // End of exercise!
            TriggerExerciseEnd();
        }

        if (imagesPanel != null)
        {
            Debug.Log("[BaseExercise] imagesPanel != null.");

            if (imagesPanel.IsLastSprite())
            {
                TriggerExerciseEnd();
            }
        }
    }

    public void TriggerExerciseEnd()
    {
        if (isInProgress) Debug.Log("[BaseExercise] Current exercise is finished.");

        isInProgress = false;
    }

    public void SetExerciseId(int id)
    {
        this.id = id;
        Debug.Log($"[BaseExercise] Exercise id: {id}.");
    }

    private void OnDestroy()
    {
        for (int i = 0; i < allObjectsSpawned.Count; i++)
        {
            Destroy(allObjectsSpawned[i]);
        }
    }
}
