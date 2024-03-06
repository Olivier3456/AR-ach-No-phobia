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

        for (int i = 0; i < instantiatedInputFields.Count; i++)
        {
            Destroy(instantiatedInputFields[i].gameObject);
        }
        instantiatedInputFields.Clear();
        Debug.Log("All instantiated input fields deleted");

        ExerciseData[] exercises = AnxietyDataHandler.AnxietyData.exercises;

        if (exercises != null && exercises.Length > 0)
        {
            foreach (var item in exercises)
            {
                TMP_InputField inputField = Instantiate(inputFieldTemplate.gameObject.GetComponent<TMP_InputField>(), inputFieldsParentTransform);
                instantiatedInputFields.Add(inputField);

                string inputFieldText = $"{item.time} | Exercice {item.exerciseId} | Anxiété : {item.anxietyNote}";
                inputField.text = inputFieldText;
                inputField.gameObject.SetActive(true);
            }
        }
        else
        {
            TMP_InputField inputField = Instantiate(inputFieldTemplate.gameObject.GetComponent<TMP_InputField>(), inputFieldsParentTransform);
            instantiatedInputFields.Add(inputField);
            string inputFieldText = "Vous n'avez encore fini aucun exercice.";
            inputField.text = inputFieldText;
            inputField.gameObject.SetActive(true);
        }
    }
}
