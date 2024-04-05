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
    [Header("Offset for all menus. The offset for left hand will be symetrical to the offset for right hand.")]
    [SerializeField] private Vector3 offsetForRightHand;
    private Vector3 offsetForLeftHand;
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
    [SerializeField] private GameObject exerciseNotFinishedLabel;
    [SerializeField] private GameObject exerciseFinishedLabel;
    [Space(20)]
    [SerializeField] private GameObject distancesCanvasGameObject;
    [SerializeField] private TextMeshPro displayDistancesButtonText;
    [Space(20)]
    [SerializeField] private TextMeshPro displaySubtitlesButtonText;

    private ActiveStateSelector currentHandDisplayingMenu = null;
    private OVRBone leftThumbTip = null;
    private OVRBone rightThumbTip = null;


    private void Awake()
    {
        offsetForLeftHand = new Vector3(-offsetForRightHand.x, offsetForRightHand.y, offsetForRightHand.z);
    }


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
    }


    private void Update()
    {
        if (currentHandDisplayingMenu != null && leftThumbTip != null && rightThumbTip != null)
        {
            UpdateMenuPositionAndRotation(true);
        }
    }


    public void MenuHandPoseSelected(ActiveStateSelector ass)
    {
        if (currentHandDisplayingMenu != null)
        {
            return;
        }

        currentHandDisplayingMenu = ass;

        //Debug.Log("Hand pose for menu detected.");

        if (showHandsModelWhenMenuIsVisible)
        {
            leftHandRenderer.material = visibleHandMaterial;
            rightHandRenderer.material = visibleHandMaterial;
        }

        UpdateMenuPositionAndRotation(false);
        DisplayActualMainMenu();
    }


    public void MenuHandPoseUnselected(ActiveStateSelector ass)
    {
        if (currentHandDisplayingMenu == ass)
        {
            HideAllMenus(true);
        }
    }


    private void UpdateMenuPositionAndRotation(bool lerpPosition)
    {
        Vector3 direction = transform.position - Camera.main.transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 adjustedOffset;
        Vector3 targetPosition = Vector3.zero;

        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

        if (currentHandDisplayingMenu == menuHandPoseLeft)
        {
            adjustedOffset = Quaternion.Euler(0, rotation.eulerAngles.y, 0) * offsetForLeftHand;
            targetPosition = leftThumbTip.Transform.position + adjustedOffset;
        }
        else if (currentHandDisplayingMenu == menuHandPoseRight)
        {
            adjustedOffset = Quaternion.Euler(0, rotation.eulerAngles.y, 0) * offsetForRightHand;
            targetPosition = rightThumbTip.Transform.position + adjustedOffset;
        }

        if (lerpPosition)
        {
            float lerp = 0.1f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerp);
        }
        else
        {
            transform.position = targetPosition;
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
    }


    public void DisplayActualMainMenu()
    {
        HideAllMenus(false);

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
            currentHandDisplayingMenu = null;
            leftHandRenderer.material = invisibleHandMaterial;
            rightHandRenderer.material = invisibleHandMaterial;
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

        AnxietyDataHandler.SetAnxietyLevel(anxietyLevel);
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
