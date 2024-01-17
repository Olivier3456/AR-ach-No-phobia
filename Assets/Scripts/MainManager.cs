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
    [SerializeField] private BaseExercise[] exercisesPrefabs;

    // Chosen exercice can be not the same as current exercice if the exercice is not yet begun.
    private int chosenExerciceID = 0;

    private BaseExercise currentExercise;

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


    public void BeginExercice()
    {
        if (chosenExerciceID == 0)
        {
            Debug.Log("[MainManager] No exercise chosen yet. Can't begin exercice.");
            return;
        }
        else
        {
            handMenuBehaviour.HideAllMenus(true);
            handMenuBehaviour.DeactivateOkButton();
            currentExercise = Instantiate(exercisesPrefabs[chosenExerciceID - 1]);
            currentExercise.SetExerciseId(chosenExerciceID);

            Debug.Log($"[MainManager] Begining exercise {currentExercise.Id}");
            Debug.Log($"[MainManager] Exercice Game Object name is: {currentExercise.name}.");
        }
    }

    public void QuitExercice()
    {
        Debug.Log($"[MainManager] Quitting exercise {currentExercise.Id}");

        handMenuBehaviour.HideAllMenus(true);
        Destroy(currentExercise.gameObject);
        currentExercise = null;
    }


    public int GetChosenExerciceID() { return chosenExerciceID; }
    public void ChoseNextExercise(int exerciceID) { chosenExerciceID = exerciceID; }
    public SpawnObjectOnSceneAnchor GetSpawnObjectOnSceneAnchor() { return spawnObjectOnSceneAnchor; }
    public BaseExercise GetCurrentExercise() { return currentExercise; }


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
