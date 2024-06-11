using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HandMenuBehaviour : MonoBehaviour
{
    [Header("Parent menus GameObjects")]
    [SerializeField] private GameObject noExerciceMainMenu;
    [SerializeField] private GameObject exerciceMainMenu;
    [SerializeField] private GameObject quitExerciceConfirmationMenu;
    [SerializeField] private GameObject levelsChoiceMenu;
    [SerializeField] private GameObject progressionMenu;
    [SerializeField] private GameObject reinitializeProgressionConfirmationMenu;
    [SerializeField] private GameObject settingsMenu;
    [Space(20)]
    [Header("Buttons previous/next for images panel")]
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [Space(20)]
    [SerializeField] private ActiveStateSelector menuHandPoseLeft;
    [SerializeField] private ActiveStateSelector menuHandPoseRight;
    [Space(10)]
    [SerializeField] private OVRSkeleton ovrSkeletonLeftHand;
    [SerializeField] private OVRSkeleton ovrSkeletonRightHand;
    [Space(20)]
    [Header("Exercises toggles of the levels choice menu.")]
    [SerializeField] private Toggle[] exercicesToggles;
    [Space(20)]
    [SerializeField] private TextMeshPro exerciseLabelText;
    [Space(20)]
    [SerializeField] private bool showHandsModelWhenMenuIsVisible;
    [SerializeField] private SkinnedMeshRenderer leftHandRenderer;
    [SerializeField] private SkinnedMeshRenderer rightHandRenderer;
    [SerializeField] private Material visibleHandMaterial;
    [SerializeField] private Material invisibleHandMaterial;
    [Space(20)]
    [Header("Quit Exercise Menu")]
    [SerializeField] private GameObject anxietyButtonsParent;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject nextExerciseButton;
    [SerializeField] private GameObject noButton;
    [SerializeField] private GameObject exerciseNotFinishedLabel;
    [SerializeField] private GameObject exerciseFinishedLabel;
    [Space(20)]
    [SerializeField] private GameObject distancesCanvasGameObject;
    [SerializeField] private TextMeshPro displayDistancesButtonText;
    [Space(20)]
    [SerializeField] private TextMeshPro displaySubtitlesButtonText;
    [Space(20)]
    [SerializeField] private Transform[] anxietyButtonsTransforms;
    [SerializeField] private SpriteRenderer selectionCircle;
    [SerializeField] private Gradient anxietyColorGradient;

    private OVRBone leftThumbTip = null;
    private OVRBone rightThumbTip = null;

    private bool isMenuVisible = false;


    private IEnumerator Start()
    {
        yield return null;

        foreach (var bone in ovrSkeletonLeftHand.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip)
            {
                leftThumbTip = bone;

                //Debug.Log("Left Thumb tip found");
            }
        }

        foreach (var bone in ovrSkeletonRightHand.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip)
            {
                rightThumbTip = bone;

                //Debug.Log("Right Thumb tip found");
            }
        }

        MainManager.Instance.OnExerciseFinished.AddListener(DisplayActualMainMenu);
    }


    private void Update()
    {
        if (isMenuVisible)
        {
            // Menu disappears if the camera is too far from the user.

            float distanceFromCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
            float maxDistance = 1f;
            if (distanceFromCamera > maxDistance)
            {
                HideAllMenus(true);
                return;
            }


            // Menu disappears if the user is not looking at it.

            Vector3 directionFromCamera = transform.position - Camera.main.transform.position;
            Vector3 lookDirection = Camera.main.transform.forward;

            float angle = Vector3.Angle(directionFromCamera, lookDirection);
            float maxAngle = 60;
            //Debug.Log("Angle: " + angle);
            if (angle > maxAngle)
            {
                HideAllMenus(true);
                return;
            }
        }
    }


    public void MenuHandPoseSelected(ActiveStateSelector ass)
    {
        if (showHandsModelWhenMenuIsVisible)
        {
            leftHandRenderer.material = visibleHandMaterial;
            rightHandRenderer.material = visibleHandMaterial;
        }

        SetMenuPositionAndRotation(ass);
        DisplayActualMainMenu();
    }
    

    private void SetMenuPositionAndRotation(ActiveStateSelector handDisplayingMenu)
    {
        Vector3 direction = Vector3.zero;   // Direction from camera to hand.
        if (handDisplayingMenu == menuHandPoseLeft)
        {
            direction = leftThumbTip.Transform.position - Camera.main.transform.position;
        }
        else if (handDisplayingMenu == menuHandPoseRight)
        {
            direction = rightThumbTip.Transform.position - Camera.main.transform.position;
        }
        direction.y = 0;
        direction = direction.normalized;


        Vector3 orthogonalDirection = new Vector3(-direction.z, 0, direction.x);    // Lateral offset.
        orthogonalDirection = orthogonalDirection.normalized;


        if (handDisplayingMenu == menuHandPoseLeft)  // Places the menu at the position of the hand, with a lateral offset.
        {
            transform.position = leftThumbTip.Transform.position - (orthogonalDirection * 0.2f);
        }
        else if (handDisplayingMenu == menuHandPoseRight)
        {
            transform.position = rightThumbTip.Transform.position + (orthogonalDirection * 0.2f);
        }


        Vector3 newDirection = Vector3.zero;    // The direction from camera to menu's new position.
        if (handDisplayingMenu == menuHandPoseLeft)
        {
            newDirection = transform.position - Camera.main.transform.position;
        }
        else if (handDisplayingMenu == menuHandPoseRight)
        {
            newDirection = transform.position - Camera.main.transform.position;
        }
        newDirection.y = 0;
        newDirection = newDirection.normalized;


        Quaternion rotation = Quaternion.LookRotation(newDirection);    // Sets the rotation of the menu.
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
    

    public void DisplayActualMainMenu()
    {
        HideAllMenus(false);

        isMenuVisible = true;

        if (MainManager.Instance.CurrentExercise == null)
        {
            //Debug.Log("[HandMenuBehaviour] Displaying main menu for NO exercise.");

            noExerciceMainMenu.SetActive(true);
        }
        else
        {
            //Debug.Log($"[HandMenuBehaviour] Displaying main menu for exercise {MainManager.Instance.CurrentExercise.Id}.");

            if (MainManager.Instance.CurrentExercise.IsInProgress)
            {
                exerciseLabelText.text = $"Exercice {MainManager.Instance.CurrentExercise.Id}";
                exerciceMainMenu.SetActive(true);
            }
            else
            {
                DisplayQuitConfirmationMenu();
            }

            DisplayOrHidePreviousAndNextButtons();
        }
    }


    public void DisplayQuitConfirmationMenu()
    {
        HideAllMenus(false);
        quitExerciceConfirmationMenu.SetActive(true);

        bool exerciseInProgress = MainManager.Instance.CurrentExercise.IsInProgress;

        exerciseNotFinishedLabel.SetActive(exerciseInProgress);
        exerciseFinishedLabel.SetActive(!exerciseInProgress);
        anxietyButtonsParent.SetActive(!exerciseInProgress);
        quitButton.SetActive(exerciseInProgress);
        nextExerciseButton.SetActive(false);

        noButton.SetActive(exerciseInProgress);

        selectionCircle.gameObject.SetActive(false);
    }


    public void DisplayOrHidePreviousAndNextButtons()
    {
        if (MainManager.Instance.CurrentExercise == null)
        {
            return;
        }

        ImagesPanel imagesPanel = MainManager.Instance.CurrentExercise.ImagesPanel;

        if (imagesPanel == null)
        {
            previousButton.SetActive(false);
            nextButton.SetActive(false);
        }
        else
        {
            if (imagesPanel.IsFirstSprite())
            {
                previousButton.SetActive(false);
            }
            else
            {
                previousButton.SetActive(true);
            }

            if (imagesPanel.IsLastSprite())
            {
                nextButton.SetActive(false);
            }
            else
            {
                nextButton.SetActive(true);
            }
        }
    }


    public void DisplayLevelsChoiceMenu()
    {
        HideAllMenus(false);

        foreach (Toggle toggle in exercicesToggles)
        {
            if (toggle.gameObject.name.Contains(MainManager.Instance.ChosenExerciseID.ToString()))
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }

        levelsChoiceMenu.SetActive(true);
    }


    public void DisplayProgressionMenu()
    {
        HideAllMenus(false);
        progressionMenu.SetActive(true);
    }


    public void DisplayReinitializeProgressionConfirmationMenu()
    {
        HideAllMenus(false);
        reinitializeProgressionConfirmationMenu.SetActive(true);
    }


    public void DisplaySettingsMenu()
    {
        HideAllMenus(false);
        settingsMenu.SetActive(true);
    }



    public void HideAllMenus(bool isMenuDisappearing)
    {
        noExerciceMainMenu.SetActive(false);
        progressionMenu.SetActive(false);
        reinitializeProgressionConfirmationMenu.SetActive(false);
        levelsChoiceMenu.SetActive(false);
        exerciceMainMenu.SetActive(false);
        quitExerciceConfirmationMenu.SetActive(false);
        settingsMenu.SetActive(false);

        if (isMenuDisappearing)
        {
            //currentHandDisplayingMenu = null;
            leftHandRenderer.material = invisibleHandMaterial;
            rightHandRenderer.material = invisibleHandMaterial;

            isMenuVisible = false;
        }
    }


    public void ChoseExercise(bool isOn)
    {
        if (!isOn)
        {
            return;  // To avoid this function to be executed each time by the toggle switched on AND the toggle switched off.
        }

        for (int i = 0; i < exercicesToggles.Length; i++)
        {
            if (exercicesToggles[i].isOn)
            {
                MainManager.Instance.ChoseNextExercise(i + 1);
                //Debug.Log($"[HandMenuBehaviour] Exercise chosen: exercice {i + 1}.");
                return;
            }
        }

        //Debug.Log("[HandMenuBehaviour] No exercice toggle is on! Can't chose exercise.");
    }





    public void DeactivateOkButton()
    {
        quitButton.SetActive(false);
    }


    public void ChoseAnxietyLevel(int anxietyLevel)
    {
        bool isLastExercise = MainManager.Instance.CurrentExercise.Id == MainManager.Instance.TotalExercisesNumber;
        nextExerciseButton.SetActive(!isLastExercise);
        quitButton.SetActive(true);
        noButton.SetActive(false);

        AnxietyDataHandler.SetAnxietyLevel(anxietyLevel);

        selectionCircle.transform.position = anxietyButtonsTransforms[anxietyLevel].position;
        selectionCircle.color = anxietyColorGradient.Evaluate(anxietyLevel * 0.1f);
        selectionCircle.gameObject.SetActive(true);
    }


    public void ToggleDisplayOrHideDistances()
    {
        distancesCanvasGameObject.SetActive(!distancesCanvasGameObject.activeSelf);
        displayDistancesButtonText.text = distancesCanvasGameObject.activeSelf ? "Masquer la distance de l'araignée la plus proche" : "Afficher la distance de l'araignée la plus proche";
    }


    public void ToggleDisplayOrHideSubtitles()
    {
        SubtitlesManager.MustDisplaySubtitles(!SubtitlesManager.DisplaySubtitles);
        displaySubtitlesButtonText.text = SubtitlesManager.DisplaySubtitles ? "Masquer les sous-titres" : "Afficher les sous-titres";
    }


    public void ApplicationQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
