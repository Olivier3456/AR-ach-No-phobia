using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================== DEPRECATED ====================

public class AnxietyButtons : MonoBehaviour
{
    [SerializeField] private bool arrangeButtonsInCircle = true;
    [SerializeField] private float radius = 0.05f;
    [Space(20)]
    [SerializeField] private Transform[] anxietyButtons;
    [SerializeField] private GameObject OkButton;

    void Start()
    {
        OkButton.SetActive(false);

        if (arrangeButtonsInCircle)
        {
            ArrangeButtonsInCircle();
        }
    }


    public void AnxietyLevelChosen(int anxietyLevel)
    {
        OkButton.SetActive(true);
    }


    private void ArrangeButtonsInCircle()
    {
        float angleIncrement = 360f / anxietyButtons.Length;

        for (int i = 0; i < anxietyButtons.Length; i++)
        {
            float angle = 90 - (i * angleIncrement);
            float radians = Mathf.Deg2Rad * angle;

            float y = Mathf.Sin(radians) * radius;
            float z = Mathf.Cos(radians) * radius;

            Vector3 localPosition = new Vector3(z, y, 0f);

            anxietyButtons[i].transform.localPosition = localPosition;
        }
    }
}
