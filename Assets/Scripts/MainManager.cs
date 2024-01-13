using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    [SerializeField] private SpawnObjectOnSceneAnchor spawnObjectOnSceneAnchor;
    [Space(20)]
    [SerializeField] private HandMenuBehaviour handMenuBehaviour;
    [Space(20)]
    [SerializeField] private ImagesPanel imagesPanelPrefab;

    // 0 for no exercice.
    private int currentExerciceID = 0;

    // Chosen exercice can be not the same as current exercice if the exercice is not yet begun.
    private int currentChosenExerciceID = 0;

    private ImagesPanel imagesPanelInstantiated = null;

    private List<GameObject> gameObjectsOfCurrentExercice = new List<GameObject>();

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
        if (currentChosenExerciceID == 0)
        {
            Debug.Log("[MainManager] No exercice chosen yet. Can't begin exercice.");
            return;
        }
        else if (currentChosenExerciceID == 1)
        {
            imagesPanelInstantiated = spawnObjectOnSceneAnchor.SpawnObject(imagesPanelPrefab.gameObject, SpawnObjectOnSceneAnchor.AnchorTypes.TABLE).GetComponent<ImagesPanel>();

            gameObjectsOfCurrentExercice.Add(imagesPanelInstantiated.gameObject);
        }

        currentExerciceID = currentChosenExerciceID;
        handMenuBehaviour.HideAllMenus(true);
    }

    public void QuitExercice()
    {
        handMenuBehaviour.HideAllMenus(true);
        DestroyAllExerciceGameObjects();
        currentExerciceID = 0;
    }


    private void DestroyAllExerciceGameObjects()
    {
        for (int i = 0; i < gameObjectsOfCurrentExercice.Count; i++)
        {
            Destroy(gameObjectsOfCurrentExercice[i]);
        }

        gameObjectsOfCurrentExercice.Clear();
    }


    public int GetCurrentChosenExerciceID() { return currentChosenExerciceID; }
    public void SetCurrentChosenExerciceID(int exerciceID) { currentChosenExerciceID = exerciceID; }
    public int GetCurrentExerciceID() { return currentExerciceID; }
    public ImagesPanel GetImagesPanel() { return imagesPanelInstantiated; }

    public void DisplayNextSpriteOfImagePanel()
    {
        if (imagesPanelInstantiated != null)
        {
            imagesPanelInstantiated.DisplayNextSprite();
        }
    }

    public void DisplayPreviousSpriteOfImagePanel()
    {
        if (imagesPanelInstantiated != null)
        {
            imagesPanelInstantiated.DisplayPreviousSprite();
        }
    }
}
