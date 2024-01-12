using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class HandMenuBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelsChoiceMenu;
    [SerializeField] private GameObject settingsMenu;
    [Space(20)]
    [SerializeField] private ActiveStateSelector menuHandPoseLeft;
    [SerializeField] private ActiveStateSelector menuHandPoseRight;
    [Space(10)]
    [SerializeField] private OVRSkeleton ovrSkeletonLeftHand;
    [SerializeField] private OVRSkeleton ovrSkeletonRightHand;
    [Space(20)]
    [SerializeField] private Toggle[] exercicesToggles;
    [Space(20)]
    [SerializeField] private Vector3 offset;

    private ActiveStateSelector currentHandDisplayingMenu = null;

    private OVRBone leftThumbTip;
    private OVRBone rightThumbTip;


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
        ChangeMenuPosition();
    }


    private void ChangeMenuPosition()
    {
        if (leftThumbTip != null && rightThumbTip != null)
        {
            if (currentHandDisplayingMenu == menuHandPoseLeft)
            {
                transform.position = leftThumbTip.Transform.position + offset;
            }
            else if (currentHandDisplayingMenu == menuHandPoseRight)
            {
                transform.position = rightThumbTip.Transform.position + offset;
            }
        }
    }



    public void DisplayMainMenu()
    {
        levelsChoiceMenu.SetActive(false);
        //settingsMenu.SetActive(false);      // TODO

        mainMenu.SetActive(true);
    }


    public void DisplayLevelsChoiceMenu()
    {
        mainMenu.SetActive(false);
        //settingsMenu.SetActive(false);      // TODO

        levelsChoiceMenu.SetActive(true);
    }

    public void HideMenu()
    {
        mainMenu.SetActive(false);
        levelsChoiceMenu.SetActive(false);
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
                MainManager.Instance.SetCurrentExerciceID(i + 1);
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

            //transform.position = leftThumbTip.Transform.position + offset;

            ChangeMenuPosition();
            DisplayMainMenu();
        }
        else if (ass == menuHandPoseRight)
        {
            Debug.Log("La pose du menu pour la main droite a été reconnue.");

            //transform.position = rightThumbTip.Transform.position + offset;

            ChangeMenuPosition();
            DisplayMainMenu();
        }
    }

    public void MenuHandPoseUnselected(ActiveStateSelector ass)
    {
        if (currentHandDisplayingMenu == ass)
        {
            HideMenu();
            currentHandDisplayingMenu = null;
        }
    }



    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
