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
    [SerializeField] private AudioClip introClip_3;
    [SerializeField] private AudioClip introClip_4;
    [Space(20)]
    [SerializeField] private SubtitlesManager subtitlesManager;
    [Space(20)]
    public UnityEvent<int> OnExerciseBegin = new UnityEvent<int>();

    public UnityEvent OnExerciseQuitted = new UnityEvent();
    public UnityEvent OnExerciseFinished = new UnityEvent();

    //public const string SECOND_LAUNCH_PLAYERPREFS_KEY = "SecondLaunch";


    // Chosen exercice can be not the same as current exercice if chosen exercice is not yet begun.
    private int chosenExerciseID = 1;

    private BaseExercise currentExercise;

    private AudioSource audioSource;

    private bool isFirstLaunch;

    public int ChosenExerciseID { get { return chosenExerciseID; } }
    public void ChoseNextExercise(int exerciceID) { chosenExerciseID = exerciceID; }
    public SpawnObjectOnSceneAnchor SpawnObjectOnSceneAnchor { get { return spawnObjectOnSceneAnchor; } }
    public BaseExercise CurrentExercise { get { return currentExercise; } }
    public int TotalNumberOfExercises { get { return exercisesPrefabs.Length; } }
    public Transform LeftPalmCenterMarker { get { return leftPalmCenterMarker; } }
    public Transform RightPalmCenterMarker { get { return rightPalmCenterMarker; } }
    public int TotalExercisesNumber { get { return exercisesPrefabs.Length; } }
    public AudioSource AudioSource { get { return audioSource; } }
    public SubtitlesManager SubtitlesManager { get { return subtitlesManager; } }



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

        SceneAnchorHelper.OnSceneAnchorsLoaded.AddListener(OnSceneAnchorsLoaded);
    }


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (AnxietyDataHandler.AnxietyData.exercises == null || AnxietyDataHandler.AnxietyData.exercises.Length < 1)
        {
            audioSource.clip = introClip_1; // Welcome in AR-ach-NO-Phobia.
            audioSource.Play();
            subtitlesManager.ShowSubtitles();
            isFirstLaunch = true;
        }
        else
        {
            audioSource.clip = introClip_3; // Welcome back in AR-ach-NO-Phobia.
            audioSource.Play();
            subtitlesManager.ShowSubtitles();
        }
    }


    private void OnSceneAnchorsLoaded(bool areAllNeededAnchorsFound)
    {
        bool isSpaceSetupRequired = !areAllNeededAnchorsFound;
        StartCoroutine(RequestSpaceSetupIfNeeded(isSpaceSetupRequired));    // We execute this coroutine even if space setup is not needed, to play the 2e intro audio clip for the first launch.
    }

    private IEnumerator RequestSpaceSetupIfNeeded(bool isRequired)
    {
        WaitForSeconds waitForOneSecond = new WaitForSeconds(1);

        if (!isFirstLaunch && isRequired)
        {
            audioSource.clip = introClip_4; // The room is not set correctly anymore. User need to add the missing elements.
            audioSource.Play();
            subtitlesManager.ShowSubtitles();
        }

        while (audioSource.isPlaying)
        {
            yield return waitForOneSecond;
        }

        if (isRequired)
        {
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
        }

        if (isFirstLaunch && currentExercise == null)
        {
            audioSource.clip = introClip_2; // User's room is now set correctly + instructions for accessing the menu.
            audioSource.Play();
            subtitlesManager.ShowSubtitles();
        }
    }
        

    public void BeginExercise()
    {
        audioSource.Stop();

        handMenuBehaviour.HideAllMenus(true);
        handMenuBehaviour.DeactivateOkButton();

        currentExercise = Instantiate(exercisesPrefabs[chosenExerciseID - 1]);

        OnExerciseBegin.Invoke(chosenExerciseID);

        //Debug.Log($"[MainManager] Begining exercise {currentExercise.Id}");
        //Debug.Log($"[MainManager] Exercise Game Object name is: {currentExercise.name}.");        
    }


    public void QuitExercise()
    {
        //Debug.Log($"[MainManager] Quitting exercise {currentExercise.Id}");

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
