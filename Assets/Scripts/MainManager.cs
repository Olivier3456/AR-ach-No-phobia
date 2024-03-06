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
    [Space(20)]
    [SerializeField] private Transform leftPalmCenterMarker;
    [SerializeField] private Transform rightPalmCenterMarker;
    [Space(20)]
    [SerializeField] private AudioClip introClip_1;
    [SerializeField] private AudioClip introClip_2;
    [Space(20)]
    public UnityEvent<int> OnExerciseBegin = new UnityEvent<int>();

    public UnityEvent OnExerciseQuitted = new UnityEvent();

    public const string SECOND_LAUNCH_PLAYERPREFS_KEY = "SecondLaunch";


    // Chosen exercice can be not the same as current exercice if chosen exercice is not yet begun.
    private int chosenExerciseID = 0;

    private BaseExercise currentExercise;

    private AudioSource audioSource;

    private bool isInitDone;


    public int ChosenExerciseID { get { return chosenExerciseID; } }
    public void ChoseNextExercise(int exerciceID) { chosenExerciseID = exerciceID; }
    public SpawnObjectOnSceneAnchor SpawnObjectOnSceneAnchor { get { return spawnObjectOnSceneAnchor; } }
    public BaseExercise CurrentExercise { get { return currentExercise; } }
    public int TotalNumberOfExercises { get { return exercisesPrefabs.Length; } }
    public Transform LeftPalmCenterMarker { get { return leftPalmCenterMarker; } }
    public Transform RightPalmCenterMarker { get { return rightPalmCenterMarker; } }


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


    private void Start()
    {
        //// =============== DEBUG ===============
        //PlayerPrefs.DeleteAll();
        //// =====================================


        audioSource = GetComponent<AudioSource>();

        if (!PlayerPrefs.HasKey(SECOND_LAUNCH_PLAYERPREFS_KEY))
        {
            audioSource.clip = introClip_1;
            audioSource.Play();
            PlayerPrefs.SetInt(SECOND_LAUNCH_PLAYERPREFS_KEY, 1);
        }

        StartCoroutine(RequestSpaceSetup());    // The coroutine just waits the end of intro speach if this is the first launch.
    }

    private IEnumerator RequestSpaceSetup()
    {
        WaitForSeconds waitForOneSecond = new WaitForSeconds(1);

        while (audioSource.isPlaying)
        {
            yield return waitForOneSecond;
        }

        var classifications = new[]
            {
        OVRSceneManager.Classification.Table,
        OVRSceneManager.Classification.Floor,
        OVRSceneManager.Classification.Ceiling,
        OVRSceneManager.Classification.WallFace,
        OVRSceneManager.Classification.WallFace,
        OVRSceneManager.Classification.WallFace,
        OVRSceneManager.Classification.WallFace
            };

        var sceneManager = FindObjectOfType<OVRSceneManager>();
        sceneManager.RequestSceneCapture(classifications);

        isInitDone = true;

        audioSource.clip = introClip_2;
        audioSource.Play();
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
            audioSource.Stop();

            handMenuBehaviour.HideAllMenus(true);
            handMenuBehaviour.DeactivateOkButton();

            currentExercise = Instantiate(exercisesPrefabs[chosenExerciseID - 1]);

            OnExerciseBegin.Invoke(chosenExerciseID);

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
        OnExerciseQuitted.Invoke();
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
