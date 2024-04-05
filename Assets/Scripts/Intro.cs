using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private const int MAIN_SCENE_ID = 1;

    void Start()
    {
        StartCoroutine(Intro_Coroutine());
    }

    private IEnumerator Intro_Coroutine()
    {
        canvasGroup.alpha = 0f;

        yield return new WaitForSeconds(0.5f);

        float timer = 0f;

        //Canvas appears
        float apparitionLength = 2f;
        while (timer < apparitionLength)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha += Time.deltaTime / apparitionLength;
            MoveCanvas();
        }



        timer = 0f;
        float middleLength = 2f;
        while (timer < middleLength)
        {
            yield return null;
            timer += Time.deltaTime;            
            MoveCanvas();
        }



        // Canvas disappears.
        timer = 0f;
        float disapperanceLength = 1f;
        while (timer < disapperanceLength)
        {
            yield return null;
            timer += Time.deltaTime;
            canvasGroup.alpha -= Time.deltaTime / disapperanceLength;
            MoveCanvas();
        }

        canvasGroup.alpha = 0f;

        yield return null;

        SceneManager.LoadScene(MAIN_SCENE_ID);
    }


    private void MoveCanvas()
    {
        canvasGroup.transform.position -= Vector3.forward * Time.deltaTime * 0.1f;
    }
}
