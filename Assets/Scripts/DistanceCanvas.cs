using TMPro;
using UnityEngine;

public class DistanceCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText1;
    [SerializeField] private TextMeshProUGUI distanceText2;
    [SerializeField] private TextMeshProUGUI distanceText3;


    void Update()
    {
        if (DistanceUpdater.DistanceUpdatersInstantiated > 0)
        {
            distanceText1.gameObject.SetActive(true);
            distanceText2.gameObject.SetActive(true);
            distanceText3.gameObject.SetActive(true);

            distanceText1.text = $"distance: {DistanceUpdater.MinDistanceFromCam.ToString("0.00")}";
            distanceText2.text = $"average dist: {DistanceUpdater.AverageDistanceFromCam.ToString("0.00")}";
            distanceText3.text = $"min dist: {DistanceUpdater.MinDistanceFromCamEver.ToString("0.00")}";
        }
        else
        {
            distanceText1.gameObject.SetActive(false);
            distanceText2.gameObject.SetActive(false);
            distanceText3.gameObject.SetActive(false);
        }
    }
}
