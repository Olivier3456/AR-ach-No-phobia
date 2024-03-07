using TMPro;
using UnityEngine;

public class DistanceCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentDistanceText;
    [SerializeField] private TextMeshProUGUI minimumDistanceText;
    [SerializeField] private TextMeshProUGUI averageDistanceText;


    void Update()
    {
        if (DistanceUpdater.DistanceUpdatersInstantiated > 0)
        {
            currentDistanceText.gameObject.SetActive(true);
            minimumDistanceText.gameObject.SetActive(true);
            averageDistanceText.gameObject.SetActive(true);

            currentDistanceText.text = $"actuelle : {DistanceUpdater.MinDistanceFromCam.ToString("0.00")} m";
            minimumDistanceText.text = $"minimale : {DistanceUpdater.MinDistanceFromCamEver.ToString("0.00")} m";
            averageDistanceText.text = $"moyenne : {DistanceUpdater.AverageDistanceFromCam.ToString("0.00")} m";
        }
        else
        {
            currentDistanceText.gameObject.SetActive(false);
            minimumDistanceText.gameObject.SetActive(false);
            averageDistanceText.gameObject.SetActive(false);
        }
    }
}
