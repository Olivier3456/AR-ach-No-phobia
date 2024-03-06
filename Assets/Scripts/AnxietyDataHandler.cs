using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AnxietyData
{
    public ExerciseData[] exercises;
}

[Serializable]
public class ExerciseData
{
    public string time;
    public int exerciseId;
    public int anxietyNote;
}

public class AnxietyDataHandler : MonoBehaviour
{
    private static AnxietyDataHandler instance;

    private static AnxietyData anxietyData;

    private static string path;

    private static int anxietyLevel = 0;
    public static void SetAnxietyLevel(int level) { anxietyLevel = level; }

    public static AnxietyData AnxietyData { get { return anxietyData; } }

    public static UnityEvent OnAnxietyDataUpdated = new UnityEvent();


    private void Awake()
    {
        path = Application.persistentDataPath + "/AnxietyData.json";

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        LoadAnxietyDataFromFile();
    }


    private void LoadAnxietyDataFromFile()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No file to load yet");
            anxietyData = new AnxietyData();
            OnAnxietyDataUpdated.Invoke();
            return;
        }

        string jsonData = File.ReadAllText(path);

        try
        {
            anxietyData = JsonUtility.FromJson<AnxietyData>(jsonData);
            if (anxietyData == null)
            {
                anxietyData = new AnxietyData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading JSON data: " + e.Message);
            anxietyData = new AnxietyData();
        }

        Debug.Log("Anxiety data loaded from file");
        OnAnxietyDataUpdated.Invoke();
    }


    private void SaveAnxietyDataToFile()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        string jsonData = JsonUtility.ToJson(anxietyData, true);
        File.WriteAllText(path, jsonData);

        Debug.Log("Exercise data saved in file " + path);
    }

    public void AddAnxietyData()
    {
        if (anxietyData == null)
        {
            Debug.LogError("Anxiety data not loaded yet! This case should not happen. Can't save data.");
            return;
        }

        ExerciseData newExerciseData = new ExerciseData();
        newExerciseData.time = DateTime.Now.ToString();
        newExerciseData.exerciseId = MainManager.Instance.CurrentExercise.Id;
        newExerciseData.anxietyNote = anxietyLevel;

        ExerciseData[] newArray;
        if (anxietyData.exercises == null)
        {
            Debug.Log("Initializing new exercise data array");
            newArray = new ExerciseData[1];
        }
        else
        {
            newArray = new ExerciseData[anxietyData.exercises.Length + 1];
            Array.Copy(anxietyData.exercises, newArray, anxietyData.exercises.Length);
        }

        newArray[newArray.Length - 1] = newExerciseData;
        anxietyData.exercises = newArray;

        Debug.Log($"Anxiety data for current exercise added to array. Total anxiety data number: {anxietyData.exercises.Length}.");

        OnAnxietyDataUpdated.Invoke();

        //for (int i = 0; i < anxietyData.exercises.Length; i++)
        //{
        //    Debug.Log($"Anxiety Data {i}: time = {anxietyData.exercises[i].time}, exercise ID = {anxietyData.exercises[i].exerciseId}, anxiety note = {anxietyData.exercises[i].anxietyNote}.");
        //}
    }


    public void ReinitializeData()
    {
        anxietyData = new AnxietyData();

        Debug.Log("User's anxiety data reinitialized");
        OnAnxietyDataUpdated.Invoke();

        if (PlayerPrefs.HasKey(MainManager.SECOND_LAUNCH_PLAYERPREFS_KEY))
        {
            PlayerPrefs.DeleteKey(MainManager.SECOND_LAUNCH_PLAYERPREFS_KEY);   // Intro sound will be played by MainManager next launch.
        }
    }


    private void OnApplicationQuit()
    {
        SaveAnxietyDataToFile();
    }
}
