using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
//using UnityEngine.SceneManagement;
using System.Linq;

public class CurrentProjectileDisplay : MonoBehaviour
{
    public Dictionary<validActions, bool> registrationDictionary = new Dictionary<validActions, bool>();
    public GameObject NormalAttackObject;
    public GameObject FireObject;
    public GameObject LightObject;
    public GameObject MagneticObject;
    public GameObject ExplosiveObject;
    //public GameObject lastObjectActivated;


    private void OnEnable()
    {
        NormalAttackObject.SetActive(true);
        FireObject.SetActive(false);
        ExplosiveObject.SetActive(false);
        LightObject.SetActive(false);
        MagneticObject.SetActive(false);
        //lastObjectActivated = NormalAttackObject;
        StartCoroutine(TryRegisterTestEnd());
        StartCoroutine(TryRegisterTestStart());
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
        NormalAttackObject.SetActive(true);
        FireObject.SetActive(false);
        ExplosiveObject.SetActive(false);
        LightObject.SetActive(false);
        MagneticObject.SetActive(false);
        Debug.Log("Test Finished In Current projectile display");
    }


    private IEnumerator TryRegisterTestStart()
    {

        while (registrationDictionary.ContainsKey(validActions.OnTestAtkStart) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnTestAtkStart += TestStartEffect;
                registrationDictionary[validActions.OnTestAtkStart] = true;
            }
            yield return null;
        }
        yield break;
    }

    private void TestStartEffect(int clueName)
    {
        //hardcoding this is sloppy
        switch (clueName)
        {
            case 2:
                FireObject.SetActive(true);
                break;
            case 4:
                ExplosiveObject.SetActive(true);
                break;
            case 1:
                LightObject.SetActive(true);
                break;
            case 3:
                MagneticObject.SetActive(true);
                break;
        }
        NormalAttackObject.SetActive(false);
        Debug.Log("Test Started in current projectile display");
    }
}
