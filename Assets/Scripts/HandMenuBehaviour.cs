using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HandMenuBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject noExerciceMainMenu;
    [SerializeField] private GameObject exerciceMainMenu;
    [SerializeField] private GameObject quitConfirmationMenu;
    [SerializeField] private GameObject levelsChoiceMenu;
    [SerializeField] private GameObject settingsMenu;
    [Space(10)]
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;
    [Space(20)]
    [SerializeField] private ActiveStateSelector menuHandPoseLeft;
    [SerializeField] private ActiveStateSelector menuHandPoseRight;
    [Space(10)]
    [SerializeField] private OVRSkeleton ovrSkeletonLeftHand;
    [SerializeField] private OVRSkeleton ovrSkeletonRightHand;
    [Space(20)]
    [SerializeField] private Toggle[] exercicesToggles;
    [Space(20)]
    [SerializeField] private Vector3 offsetForRightHand;
    private Vector3 offsetForLeftHand;

    private ActiveStateSelector currentHandDisplayingMenu = null;

    private OVRBone leftThumbTip;
    private OVRBone rightThumbTip;


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
        UpdateMenuPositionAndRotation();
    }


    private void UpdateMenuPositionAndRotation()
    {
        if (currentHandDisplayingMenu != null && leftThumbTip != null && rightThumbTip != null)
        {
            Vector3 direction = transform.position - Camera.main.transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 adjustedOffset;

            if (currentHandDisplayingMenu == menuHandPoseLeft)
            {
                adjustedOffset = Quaternion.Euler(0, rotation.eulerAngles.y, 0) * offsetForLeftHand;
                transform.position = leftThumbTip.Transform.position + adjustedOffset;
            }
            else if (currentHandDisplayingMenu == menuHandPoseRight)
            {
                adjustedOffset = Quaternion.Euler(0, rotation.eulerAngles.y, 0) * offsetForRightHand;
                transform.position = rightThumbTip.Transform.position + adjustedOffset;
            }

            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }
    }


    public void DisplayQuitConfirmationMenu()
    {
        HideAllMenus(false);
        quitConfirmationMenu.SetActive(true);
    }



    public void DisplayActualMainMenu()
    {
        HideAllMenus(false);

        if (MainManager.Instance.GetCurrentExerciceID() == 0)
        {
            noExerciceMainMenu.SetActive(true);
        }
        else
        {
            exerciceMainMenu.SetActive(true);
        }

        DisplayOrHidePreviousAndNextButtons();
    }


    public void DisplayOrHidePreviousAndNextButtons()
    {
        if (MainManager.Instance.GetCurrentExerciceID() == 1 && MainManager.Instance.GetImagesPanel() != null && !MainManager.Instance.GetImagesPanel().IsFirstSprite())
        {
            previousButton.SetActive(true);
        }
        else
        {
            previousButton.SetActive(false);
        }

        if (MainManager.Instance.GetCurrentExerciceID() == 1 && MainManager.Instance.GetImagesPanel() != null && !MainManager.Instance.GetImagesPanel().IsLastSprite())
        {
            nextButton.SetActive(true);
        }
        else
        {
            nextButton.SetActive(false);
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
        quitConfirmationMenu.SetActive(false);

        if (isMenuDisappearing)
        {
            currentHandDisplayingMenu = null;
        }
    }



    public void SetCurrentExercice(bool isOn)
    {
        if (!isOn)  // Avoid this function to be executed each time by the toggle switched on AND the toggle switched off.
        {
            return;
        }

        for (int i = 0; i < exercicesToggles.Length; i++)
        {
            if (exercicesToggles[i].isOn)
            {
                MainManager.Instance.SetCurrentChosenExerciceID(i + 1);
                Debug.Log($"[HandMenuBehaviour] Exercice chosen: exercice {i + 1}.");
                return;
            }
        }

        Debug.LogError("[HandMenuBehaviour] No exercice toggle is on!");
    }


    public void MenuHandPoseSelected(ActiveStateSelector ass)
    {
        if (currentHandDisplayingMenu != null)
        {
            return;
        }

        currentHandDisplayingMenu = ass;

        if (ass == menuHandPoseLeft)
        {
            Debug.Log("La pose du menu pour la main gauche a été reconnue.");

            UpdateMenuPositionAndRotation();
            DisplayActualMainMenu();
        }
        else if (ass == menuHandPoseRight)
        {
            Debug.Log("La pose du menu pour la main droite a été reconnue.");

            UpdateMenuPositionAndRotation();
            DisplayActualMainMenu();
        }
    }

    public void MenuHandPoseUnselected(ActiveStateSelector ass)
    {
        if (currentHandDisplayingMenu == ass)
        {
            HideAllMenus(true);
        }
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
