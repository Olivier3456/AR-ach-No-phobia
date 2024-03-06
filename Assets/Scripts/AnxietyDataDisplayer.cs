using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnxietyDataDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldTemplate;
    [SerializeField] private Transform inputFieldsParentTransform;

    private List<TMP_InputField> instantiatedInputFields = new List<TMP_InputField>();


    private void Start()
    {
        UpdateProgressionDisplay();

        AnxietyDataHandler.OnAnxietyDataUpdated.AddListener(UpdateProgressionDisplay);
    }

    private void OnDestroy()
    {
        AnxietyDataHandler.OnAnxietyDataUpdated.RemoveListener(UpdateProgressionDisplay);
    }


    private void UpdateProgressionDisplay()
    {
        Debug.Log("Update progression display");

        foreach (var item in instantiatedInputFields)
        {
            Destroy(item.gameObject);
        }

        ExerciseData[] exercises = AnxietyDataHandler.AnxietyData.exercises;

        if (exercises.Length > 0)
        {
            foreach (var item in exercises)
            {
                TMP_InputField inputField = Instantiate(inputFieldTemplate.gameObject.GetComponent<TMP_InputField>(), inputFieldsParentTransform);
                instantiatedInputFields.Add(inputField);

                string inputFieldText = $"Date : {item.time} | Exercice {item.exerciseId} | Note d'anxiété : {item.anxietyNote}.";
                inputField.text = inputFieldText;
                inputField.gameObject.SetActive(true);
            }
        }
        else
        {
            TMP_InputField inputField = Instantiate(inputFieldTemplate.gameObject.GetComponent<TMP_InputField>(), inputFieldsParentTransform);
            instantiatedInputFields.Add(inputField);
            string inputFieldText = $"Vous n'avez pas encore fini d'exercice.";
            inputField.text = inputFieldText;
            inputField.gameObject.SetActive(true);
        }
    }
}
