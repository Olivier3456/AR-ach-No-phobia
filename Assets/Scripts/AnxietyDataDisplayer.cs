using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnxietyDataDisplayer : MonoBehaviour
{
    [SerializeField] private Transform textParentTransform;
    [SerializeField] private TextMeshProUGUI textTemplate;

    private List<TextMeshProUGUI> instantiatedTexts = new List<TextMeshProUGUI>();


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
        //Debug.Log("Update progression display");

        for (int i = 0; i < instantiatedTexts.Count; i++)
        {
            Destroy(instantiatedTexts[i].gameObject);
        }
        instantiatedTexts.Clear();
        //Debug.Log("All instantiated texts deleted");

        ExerciseData[] exercises = AnxietyDataHandler.AnxietyData.exercises;

        if (exercises != null && exercises.Length > 0)
        {
            foreach (var item in exercises)
            {
                TextMeshProUGUI TMP_text = Instantiate(textTemplate.gameObject.GetComponent<TextMeshProUGUI>(), textParentTransform);
                instantiatedTexts.Add(TMP_text);

                string text = $"{item.time} | Ex {item.exerciseId} | Anx {item.anxietyNote} | Dist min {item.minDistanceFromSpider.ToString("0.00")} | Dist moy {item.averageDistanceFromNearestSpider.ToString("0.00")}";
                TMP_text.text = text;
                TMP_text.gameObject.SetActive(true);
            }
        }
        else
        {
            TextMeshProUGUI TMP_text = Instantiate(textTemplate.gameObject.GetComponent<TextMeshProUGUI>(), textParentTransform);
            instantiatedTexts.Add(TMP_text);
            string text = "Vous n'avez encore fini aucun exercice.";
            TMP_text.text = text;
            TMP_text.gameObject.SetActive(true);
        }
    }
}
