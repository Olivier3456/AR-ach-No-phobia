using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
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

                Debug.Log("Left Thumb tip found");
            }
        }

        foreach (var bone in ovrSkeletonRightHand.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip)
            {
                rightThumbTip = bone;

                Debug.Log("Right Thumb tip found");
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
            Debug.Log($"[HandMenuBehaviour] Displaying main menu for NO exercise.");

            noExerciceMainMenu.SetActive(true);
        }
        else
        {
            Debug.Log($"[HandMenuBehaviour] Displaying main menu for exercise.");

            exerciceMainMenu.SetActive(true);

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
        levelsChoiceMenu.SetActive(true);
    }


    public void HideAllMenus(bool isMenuDisappearing)
    {
        noExerciceMainMenu.SetActive(false);
        settingsMenu.SetActive(false);      // TODO
        levelsChoiceMenu.SetActive(false);
        exerciceMainMenu.SetActive(false);
        quitExerciceConfirmationMenu.SetActive(false);

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
                Debug.Log($"[HandMenuBehaviour] Exercise chosen: exercice {i + 1}.");
                return;
            }
        }

        Debug.Log("[HandMenuBehaviour] No exercice toggle is on! Can't chose exercise.");
    }


    public void MenuHandPoseSelected(ActiveStateSelector ass)
    {
        if (currentHandDisplayingMenu != null)
        {
            return;
        }

        currentHandDisplayingMenu = ass;

        Debug.Log("Hand pose for menu detected.");

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


    public void DeactivateOkButton()
    {
        quitButton.SetActive(false);
    }


    public void ChoseAnxietyLevel(int anxietyLevel)
    {
        nextExerciseButton.SetActive(true);
        quitButton.SetActive(true);

        AnxietyDataHandler.SetAnxietyLevel(anxietyLevel);
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
