using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    [SerializeField] private SpawnObjectOnSceneAnchor spawnObjectOnSceneAnchor;
    [Space(20)]
    [SerializeField] private HandMenuBehaviour handMenuBehaviour;
    [Space(20)]
    [SerializeField] private NavMeshHandler navMeshHandler;
    [Space(20)]
    [SerializeField] private BaseExercise[] exercisesPrefabs;

    // Chosen exercice can be not the same as current exercice if the exercice is not yet begun.
    private int chosenExerciseID = 0;

    private BaseExercise currentExercise;


    public int GetChosenExerciseID() { return chosenExerciseID; }
    public void ChoseNextExercise(int exerciceID) { chosenExerciseID = exerciceID; }
    public SpawnObjectOnSceneAnchor GetSpawnObjectOnSceneAnchor() { return spawnObjectOnSceneAnchor; }
    public NavMeshHandler GetNavMeshHandler() { return navMeshHandler; }
    public BaseExercise GetCurrentExercise() { return currentExercise; }
    public int TotalNumberOfExercises { get { return exercisesPrefabs.Length; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("[MainManager] An instance of Main Manager already exist. Destroying new one.");
            Destroy(gameObject);
        }
    }


    public void BeginExercise()
    {
        if (chosenExerciseID == 0)
        {
            Debug.Log("[MainManager] No exercise chosen yet. Can't begin exercice.");
            return;
        }
        else
        {
            handMenuBehaviour.HideAllMenus(true);
            handMenuBehaviour.DeactivateOkButton();
            currentExercise = Instantiate(exercisesPrefabs[chosenExerciseID - 1]);
            currentExercise.SetExerciseId(chosenExerciseID);

            Debug.Log($"[MainManager] Begining exercise {currentExercise.Id}");
            Debug.Log($"[MainManager] Exercice Game Object name is: {currentExercise.name}.");
        }
    }


    public void QuitExercise()
    {
        Debug.Log($"[MainManager] Quitting exercise {currentExercise.Id}");

        handMenuBehaviour.HideAllMenus(true);
        Destroy(currentExercise.gameObject);
        currentExercise = null;
    }


    public void GoToNextExercise()
    {
        QuitExercise();

        if (chosenExerciseID < TotalNumberOfExercises)
        {
            chosenExerciseID++;
            BeginExercise();
        }
    }


    public void DisplayNextSprite()
    {
        if (currentExercise != null)
        {
            if (currentExercise.ImagesPanel != null)
            {
                currentExercise.ImagesPanel.DisplayNextSprite();
            }
        }
    }

    public void DisplayPreviousSprite()
    {
        if (currentExercise != null)
        {
            if (currentExercise.ImagesPanel != null)
            {
                currentExercise.ImagesPanel.DisplayPreviousSprite();
            }
        }
    }
}
