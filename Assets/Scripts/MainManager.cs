using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    
    
    private int currentExerciceID = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("[MainManager] An instance of Main Manager already exist!");
        }
    }

    public int GetCurrentExerciceID() { return currentExerciceID; }
    public void SetCurrentExerciceID(int currentExerciceID) { this.currentExerciceID = currentExerciceID; }


    
}
