using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
//using UnityEngine.SceneManagement;
using System.Linq;

public class ResultsManager : MonoBehaviour
{

    public Dictionary<validActions, bool> registrationDictionary = new Dictionary<validActions, bool>();
    public GameObject FireObject;
    public GameObject LightObject;
    public GameObject MagneticObject;
    public GameObject ExplosiveObject;


    private void OnEnable()
    {
        FireObject.SetActive(false);
        ExplosiveObject.SetActive(false);
        LightObject.SetActive(false);
        MagneticObject.SetActive(false);
        StartCoroutine(TryRegisterTestEnd());
        //SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    /*
    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        
    }*/

    private IEnumerator TryRegisterTestEnd()
    {
        while (registrationDictionary.ContainsKey(validActions.OnTestFinish) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnTestFinish += TestFinishEffect;
                registrationDictionary[validActions.OnTestFinish] = true;
            }
            yield return null;
        }
        yield break;
    }

    private void TestFinishEffect(bool isTestCorrect, HardClues clueName)
    {
        if (isTestCorrect)
        {
            switch (clueName)
            {
                case HardClues.Flammable:
                    FireObject.SetActive(true);
                    break;
                case HardClues.Explosive:
                    ExplosiveObject.SetActive(true);
                    break;
                case HardClues.LightSensitive:
                    LightObject.SetActive(true);
                    break;
                case HardClues.Magnetic:
                    MagneticObject.SetActive(true);
                    break;
            }
        }
        else
        {

        }

        Debug.Log("Test Finished In Results Manager");
    }



}
