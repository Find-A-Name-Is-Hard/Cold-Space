using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using Ami.BroAudio;
using Unity.VisualScripting;
using System.Linq;

public class CopilotDialogueManager : MonoBehaviour
{
    public CoPilotDeck m_coPilotDeck;
    public Dictionary<validActions, bool> registrationDictionary = new Dictionary<validActions, bool>();
    public TextMeshProUGUI myTextMesh;
    private float timeTillClear = 0;
    private float timeKeeping = 0;
    private bool textCleared = false;
    public float TextShownHowLong = 10f;
    private float lowHealthValue = 100f; // This is hacky and should be based on difficulty data stuff.
    private float timeLastHit = 0;
    private float wellPlayedTime = 30f;
    public Ami.BroAudio.IAudioPlayer audioPlayer;
    public float timeTillTutorialPopup = 10f;
    public bool movementKeyPressed = false;
    public bool shootKeyPressed = false;
    public bool slowMoveKeyPressed = false;

    public KeyCode[] movementKeys = {KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow};
    public KeyCode[] shootKeys = {KeyCode.Z};
    //Not Sure if right shift is correct.
    public KeyCode[] slowMoveKeys = {KeyCode.LeftShift, KeyCode.RightShift};
    
    public List<HardClues[]> enemiesRemainingList = new List<HardClues[]>();

    public static CopilotDialogueManager Instance { get; private set; }


    public struct ActResponseLive
    {


        //public bool pullFromHat;
        public int priority;
        public int interruptAtOrBelowPriority;
        public bool repeat;
        public List<TextAsset> linesToRead;
        //public TextAsset[] linesToRead;
        public bool canBeCalledTwiceInARow;
        public float maxTimeRelevant;
        public float minTimeBetweenRepetition;
        public bool addBackRandomness;
        public List<Ami.BroAudio.SoundID> soundIds;
    }

    public struct ValidActWithActVariation
    {
        public validActions validAct;
        public DialogueActionVariations actVariation;
    }

    public ValidActWithActVariation lastCalled = new ValidActWithActVariation();

    public Dictionary<ValidActWithActVariation, ActResponseLive> ActionResponseDict = new Dictionary<ValidActWithActVariation, ActResponseLive>();

    public struct ActValidation
    {
        public float whenAdded;
        public float whenLastPlayed;
        public bool inQueue;
        public bool isRelevant;
    }

    public Dictionary<ValidActWithActVariation, ActValidation> ActValidationDict = new Dictionary<ValidActWithActVariation, ActValidation>();

    public struct PriorityWithValidActWithActVariation
    {
        public int priority;
        public ValidActWithActVariation actVariation;
    }

    public List<PriorityWithValidActWithActVariation> talkOrderQueue = new List<PriorityWithValidActWithActVariation>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if(PlayerPrefs.GetInt("CopilotDifficulty", 0) == 2)
        {
            myTextMesh.text = "";
            this.gameObject.SetActive(false);
            
        }


        foreach (ActionResponse ActRes in m_coPilotDeck.actionResponses)
        {
            ActResponseLive tempActRes = new ActResponseLive();

            tempActRes.linesToRead = new List<TextAsset>();
            tempActRes.soundIds = new List<SoundID>();
            foreach (TextAsset txt in ActRes.linesToRead)
            {
                tempActRes.linesToRead.Add(txt);
            }
            foreach (SoundID sound in ActRes.soundIds)
            {
                tempActRes.soundIds.Add(sound);
            }

            if (ActRes.shuffleOnCreate)
            {
                // fisher�yates shuffle     
                for (int i = 0; i < tempActRes.linesToRead.Count; i++)
                {
                    // Pick random Element
                    int j = UnityEngine.Random.Range(i, tempActRes.linesToRead.Count);

                    // Swap Elements
                    TextAsset tmp = tempActRes.linesToRead[i];
                    tempActRes.linesToRead[i] = tempActRes.linesToRead[j];
                    tempActRes.linesToRead[j] = tmp;

                    SoundID tmp2 = tempActRes.soundIds[i];
                    tempActRes.soundIds[i] = tempActRes.soundIds[j];
                    tempActRes.soundIds[j] = tmp2;
                }
            }
            tempActRes.addBackRandomness = ActRes.addBackRandomness;
            tempActRes.repeat = ActRes.repeat;
            tempActRes.priority = ActRes.priority;
            tempActRes.interruptAtOrBelowPriority = ActRes.interruptAtOrBelowPriority;
            tempActRes.canBeCalledTwiceInARow = ActRes.canBeCalledTwiceInARow;
            tempActRes.maxTimeRelevant = ActRes.maxTimeRelevant;
            tempActRes.minTimeBetweenRepetition = ActRes.minTimeBetweenRepetition;

            ValidActWithActVariation tempVaWv = new ValidActWithActVariation();
            tempVaWv.actVariation = ActRes.actionVariation;
            tempVaWv.validAct = ActRes.actionName;
            ActionResponseDict[tempVaWv] = tempActRes;
        }

        
    }

    private bool m_isRegisterTestEnd;

    private void OnEnable()
    {
        //StartCoroutine(TryRegisterPlayerSpawn());

        PlayerSpawnEffect();
        StartCoroutine(TryRegisterTestEnd());
        StartCoroutine(TryRegisterCureEnd());
        StartCoroutine(TryRegisterHealthChange());
        StartCoroutine(TryRegisterEnergyChange());
        StartCoroutine(TimeSinceStartPassed());
    }


    public void populateEnemRemainingList(EnemyAiManager enemAIMan)
    {
        foreach (EnemyAttrtibutes enemAtt in enemAIMan.allPossibleEnemies)
        {
            List<HardClues> tempHardList = new List<HardClues>();
            foreach(EnemyAttrtibutes.HardClueEntry hce in enemAtt.clues)
            {
                if (hce.isActive)
                {
                    tempHardList.Add(hce.hardClue);
                }
            }
            tempHardList.Sort();
            enemiesRemainingList.Add(tempHardList.ToArray());
        }
        Debug.Log("Populate Enemy Remaining List: " + enemiesRemainingList.Count());
    }

    private IEnumerator TimeSinceStartPassed()
    {
        yield return new WaitForSeconds(timeTillTutorialPopup);
        //Check no movement or shots
        if ((!movementKeyPressed)&&(!shootKeyPressed))
        {
            AddDialogue(validActions.OnTimeSinceGameStart, DialogueActionVariations.notShootingOrMoving);
            StartCoroutine(SlowMoveTimeCheck());
        }
        
        //check if no movement
        else if (!movementKeyPressed)
        {
            AddDialogue(validActions.OnTimeSinceGameStart, DialogueActionVariations.notMoving);
            StartCoroutine(SlowMoveTimeCheck());
        }
        //check if no shooting
        else if (!shootKeyPressed)
        {
            AddDialogue(validActions.OnTimeSinceGameStart, DialogueActionVariations.notShooting);
            StartCoroutine(SlowMoveTimeCheck());
        }
        //check if no shift
        else if (!slowMoveKeyPressed)
        {
            AddDialogue(validActions.OnTimeSinceGameStart, DialogueActionVariations.notSlowMoving);
        }
        
    }

    private IEnumerator SlowMoveTimeCheck()
    {
        yield return new WaitForSeconds(timeTillTutorialPopup);
        //Check no movement or shots
        if (!slowMoveKeyPressed)
        {
            AddDialogue(validActions.OnTimeSinceGameStart, DialogueActionVariations.notSlowMoving);
        }
        
    }



    private IEnumerator TryRegisterTestEnd()
    {
        while (registrationDictionary.ContainsKey(validActions.OnTestFinish)==false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
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
            AddDialogue(validActions.OnTestFinish, DialogueActionVariations.positiveResult);
            List<HardClues[]> removeThese = new List<HardClues[]>();
            for (int i = 0; i < enemiesRemainingList.Count; i++)
            {
                if (!enemiesRemainingList[i].Contains(clueName))
                {
                    removeThese.Add(enemiesRemainingList[i]);
                    Debug.Log("removing this: "+ enemiesRemainingList[i]);
                }
            }

            foreach (HardClues[] hcList in removeThese)
            {
                enemiesRemainingList.Remove(hcList);
            }

            
            
            
        }
        else
        {
            AddDialogue(validActions.OnTestFinish, DialogueActionVariations.negativeResult);
            
            List<HardClues[]> removeThese = new List<HardClues[]>();
            for (int i = 0; i < enemiesRemainingList.Count; i++)
            {
                if (enemiesRemainingList[i].Contains(clueName))
                {
                    removeThese.Add(enemiesRemainingList[i]);
                    Debug.Log("removing this: "+ enemiesRemainingList[i]);
                }
            }
            
            foreach (HardClues[] hcList in removeThese)
            {
                enemiesRemainingList.Remove(hcList);
            }
        }
        
        StartCoroutine(DoDeduction());

        Debug.Log("Enemies Remaining Count After Test Finished =" +enemiesRemainingList.Count());
        Debug.Log("Test Finished In Dialogue Manager");
    }

    private IEnumerator TryRegisterCureEnd()
    {
        while (registrationDictionary.ContainsKey(validActions.OnCureHappen) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnCureHappen += CureFinishEffect;
                registrationDictionary[validActions.OnCureHappen] = true;
            }
            yield return null;
        }
        yield break;
    }


    private void CureFinishEffect(bool isCureCorrect, HardClues[] clues)
    {
        if (isCureCorrect)
        {
            AddDialogue(validActions.OnCureHappen, DialogueActionVariations.positiveResult);
            //This is Hacky
            //LevelManager.m_Instance.retMain(false);
        }
        else
        {

            AddDialogue(validActions.OnCureHappen, DialogueActionVariations.negativeResult);
            Array.Sort(clues);

            int removeAt = -1;
            for (int i = 0; i < enemiesRemainingList.Count; i++)
            {
                if (enemiesRemainingList[i]==clues)
                {
                    removeAt = i;
                }
            }
            if (removeAt!=-1)
            {
                enemiesRemainingList.RemoveAt(removeAt);
            }
            Debug.Log("Enemies Remaining Count =" +enemiesRemainingList.Count());
            StartCoroutine(DoDeduction());
        }

        Debug.Log("Cure Finished In Dialogue Manager");
    }


    private IEnumerator DoDeduction()
    {
        if(PlayerPrefs.GetInt("CopilotDifficulty", 0) == 0)
        {
            yield return new WaitForSeconds(timeTillTutorialPopup);
            //Check no movement or shots
            if (enemiesRemainingList.Count()>=3)
            {
                AddDialogue(validActions.OnMakeDeduction, DialogueActionVariations.threeSuspects);
            }
            
            //check if no movement
            else if (enemiesRemainingList.Count()==2)
            {
                AddDialogue(validActions.OnMakeDeduction, DialogueActionVariations.twoSuspects);
            }
            //check if no shooting
            else if (enemiesRemainingList.Count()==1)
            {
                AddDialogue(validActions.OnMakeDeduction, DialogueActionVariations.oneSuspect);
            }
            
        }

        
        
        
    }


    /*
    private IEnumerator TryRegisterPlayerSpawn()
    {
        while (registrationDictionary.ContainsKey(validActions.OnPlayerSpawn) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnPlayerSpawn += PlayerSpawnEffect;
                registrationDictionary[validActions.OnPlayerSpawn] = true;
            }
            yield return null;
        }
        yield break;
    }*/

    private void PlayerSpawnEffect()
    {
        AddDialogue(validActions.OnPlayerSpawn, DialogueActionVariations.none);
        //Debug.Log("Test Finished In Dialogue Manager");
    }

    private IEnumerator TryRegisterHealthChange()
    {
        while (registrationDictionary.ContainsKey(validActions.OnPlayerHealthUpdate) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnPlayerHealthUpdate += PlayerHealthUpdate;
                registrationDictionary[validActions.OnPlayerHealthUpdate] = true;
            }
            yield return null;
        }
        yield break;
    }

    private void PlayerHealthUpdate(int newHealthValue, int max)
    {
        //Check if playing perfectly for a long enough period of time
        timeLastHit = timeKeeping;
        ValidActWithActVariation tempVaWv = new ValidActWithActVariation();
        tempVaWv.actVariation = DialogueActionVariations.playingWell;
        tempVaWv.validAct = validActions.OnPlayerHealthUpdate;
        ActValidation actValid2 = new ActValidation();
        actValid2.inQueue = false;
        actValid2.isRelevant = false;
        if (ActValidationDict.ContainsKey(tempVaWv))
        {
            actValid2.whenAdded = ActValidationDict[tempVaWv].whenAdded;
            actValid2.whenLastPlayed = ActValidationDict[tempVaWv].whenLastPlayed;
        }
        else
        {
            actValid2.whenAdded = float.MinValue;
            actValid2.whenLastPlayed = float.MinValue;
        }
        
        ActValidationDict[tempVaWv] = actValid2;
        //Check if below threshold to warn about low health.
        
        if (newHealthValue<lowHealthValue)
        {
            AddDialogue(validActions.OnPlayerHealthUpdate, DialogueActionVariations.lowHealth);
        }

        //Check if the player is dead.
        if (newHealthValue <= 0)
        {
            AddDialogue(validActions.OnPlayerHealthUpdate, DialogueActionVariations.isDead);
            //this is hacky
            //LevelManager.m_Instance.retMain(true);
        }
    }


    private IEnumerator TryRegisterEnergyChange()
    {
        while (registrationDictionary.ContainsKey(validActions.OnPlayerEnergyUpdate) == false) /* or (registrationDictionary[validActions.OnTestFinish] != true)*/
        {
            if (LevelManager.m_Instance != null)
            {
                LevelManager.m_Instance.OnPlayerEnergyUpdate += PlayerEnergyUpdate;
                registrationDictionary[validActions.OnPlayerEnergyUpdate] = true;
            }
            yield return null;
        }
        yield break;
    }

    private void PlayerEnergyUpdate(int newEnergyValue, int max)
    {
        //Check if @ max energy
            
        if (newEnergyValue >= max)//max) //just hardcoded to test
        {
            AddDialogue(validActions.OnPlayerEnergyUpdate, DialogueActionVariations.fullEnergy);
            //Debug.Log("Max Energy in Dialogue manager");
        }
        else
        {
            //Debug.Log("Not Max Energy in Dialogue manager");
            ValidActWithActVariation tempVaWv = new ValidActWithActVariation();
            tempVaWv.actVariation = DialogueActionVariations.fullEnergy;
            tempVaWv.validAct = validActions.OnPlayerEnergyUpdate;
            ActValidation actValid2 = new ActValidation();
            actValid2.inQueue = false;
            actValid2.isRelevant = false;
            if (ActValidationDict.ContainsKey(tempVaWv))
            {
                actValid2.whenAdded = ActValidationDict[tempVaWv].whenAdded;
                actValid2.whenLastPlayed = ActValidationDict[tempVaWv].whenLastPlayed;
            }
            else
            {
                actValid2.whenAdded = float.MinValue;
                actValid2.whenLastPlayed = float.MinValue;
            }
            ActValidationDict[tempVaWv] = actValid2;
        }
    }



    private void AddDialogue(validActions actName, DialogueActionVariations actVariation)
    {
        ValidActWithActVariation tempVaWv = new ValidActWithActVariation();
        tempVaWv.actVariation = actVariation;
        tempVaWv.validAct = actName;

        //check if worth adding
        if (((ActValidationDict.ContainsKey(tempVaWv) == false) || (ActValidationDict[tempVaWv].inQueue == false)) && (ActionResponseDict[tempVaWv].linesToRead.Count >= 1))
        {

            PriorityWithValidActWithActVariation tempPVaWv = new PriorityWithValidActWithActVariation();
            tempPVaWv.actVariation = tempVaWv;
            tempPVaWv.priority = ActionResponseDict[tempVaWv].priority;

            talkOrderQueue.Add(tempPVaWv);
        }
        else
        {
            return;
        }
        

        ActValidation actValid = new ActValidation();
        actValid.inQueue = true;
        actValid.isRelevant = true;
        actValid.whenAdded = timeKeeping;
        if (ActValidationDict.ContainsKey(tempVaWv))
        {
            actValid.whenLastPlayed = ActValidationDict[tempVaWv].whenLastPlayed;
        }
        else
        {
            actValid.whenLastPlayed = float.MinValue;
        }
        ActValidationDict[tempVaWv] = actValid;

        //Do some checks to see if we need to interrupt immediately or not.
        if ((textCleared)||(!(ActionResponseDict.ContainsKey(tempVaWv)&&ActionResponseDict.ContainsKey(lastCalled)))||(ActionResponseDict[tempVaWv].interruptAtOrBelowPriority>ActionResponseDict[lastCalled].priority))
        {
            SpeakDialogue();
        }

    }

    private void SpeakDialogue()
    {
        //should do some validation.
        ValidActWithActVariation tempVaWv = new ValidActWithActVariation();

        bool hasValidCanidate = false;
        while (!hasValidCanidate)
        {
            if (talkOrderQueue.Count == 0)
            {
                return;
            }
            tempVaWv = fakePopQueue();
            hasValidCanidate = true;
            //check if still in queue
            if (ActValidationDict[tempVaWv].inQueue==false)
            {
                hasValidCanidate = false;
            }
            //check if still relevant
            if (ActValidationDict[tempVaWv].isRelevant == false)
            {
                hasValidCanidate = false;
            }

            //check how much time has passed since it was last played
            //(timeKeeping>timeLastHit+wellPlayedTime)
            if (timeKeeping < ActValidationDict[tempVaWv].whenLastPlayed + ActionResponseDict[tempVaWv].minTimeBetweenRepetition)
            {
                hasValidCanidate = false;
                Debug.Log(tempVaWv.actVariation.ToString() + " validation failed due to not enough time passed since last played.");
            }

            //check how much time has passed since it was last added
            if (timeKeeping > ActValidationDict[tempVaWv].whenAdded + ActionResponseDict[tempVaWv].maxTimeRelevant)
            {
                hasValidCanidate = false;
                Debug.Log(tempVaWv.actVariation.ToString() + " validation failed due to lose relevance");
            }

            //check if it can be called twice in a row
            if (lastCalled.actVariation == tempVaWv.actVariation && lastCalled.validAct == tempVaWv.validAct && (!ActionResponseDict[tempVaWv].canBeCalledTwiceInARow))
            {
                hasValidCanidate = false;
            }

            //Clear from validation dict on failed validation
            if (!hasValidCanidate)
            {
                ActValidation actValid2 = new ActValidation();
                actValid2.inQueue = false;
                actValid2.isRelevant = false;
                actValid2.whenAdded = ActValidationDict[tempVaWv].whenAdded;
                actValid2.whenLastPlayed = ActValidationDict[tempVaWv].whenLastPlayed;
                ActValidationDict[tempVaWv] = actValid2;
            }
        }

        
        Debug.Log("From Dictionary of Action at= " + ActionResponseDict[tempVaWv].linesToRead[0].text);

        //should do a more complex pick here.
        
        TextAsset tempTextAsset = ActionResponseDict[tempVaWv].linesToRead[0];
        SoundID tempSoundId = ActionResponseDict[tempVaWv].soundIds[0];
        ActionResponseDict[tempVaWv].linesToRead.RemoveAt(0);
        if (ActionResponseDict[tempVaWv].repeat)
        {
            ActionResponseDict[tempVaWv].linesToRead.Add(tempTextAsset);
            ActionResponseDict[tempVaWv].soundIds.Add(tempSoundId);
            //Not implemented adding randomness to where it is put back in.
        }
        myTextMesh.text = tempTextAsset.text;
        audioPlayer = BroAudio.Play(tempSoundId);

        //Remove text after time.
        timeTillClear = TextShownHowLong;
        textCleared = false;

        ActValidation actValid = new ActValidation();
        actValid.inQueue = false;
        actValid.isRelevant = false;
        actValid.whenAdded = ActValidationDict[tempVaWv].whenAdded;
        actValid.whenLastPlayed = timeKeeping;
        ActValidationDict[tempVaWv] = actValid;

        lastCalled = tempVaWv;

    }


    private ValidActWithActVariation fakePopQueue()
    {
        
        int maxPriority = talkOrderQueue[0].priority;
        int chosenIndex = 0;
        //this is really slow but whatever.
        for (int i = 1; i < talkOrderQueue.Count; i++)
        {
            if (talkOrderQueue[i].priority>maxPriority)
            {
                chosenIndex = i;
                maxPriority = talkOrderQueue[i].priority;
            }
        }
        ValidActWithActVariation tempCawVa = talkOrderQueue[chosenIndex].actVariation;
        talkOrderQueue.RemoveAt(chosenIndex);
        return tempCawVa;
    }

    // Update is called once per frame
    void Update()
    {
        timeKeeping += Time.deltaTime;

        if (timeKeeping>timeLastHit+wellPlayedTime)
        {
            AddDialogue(validActions.OnPlayerHealthUpdate, DialogueActionVariations.playingWell);
        }

        if (!textCleared)
        {
            timeTillClear -= Time.deltaTime;
            if (timeTillClear<=0)
            {
                textCleared = true;
                myTextMesh.text = "";
            }
        }
        if (textCleared)
        {
            if (talkOrderQueue.Count!=0)
            {
                SpeakDialogue();
            }
        }

        foreach (KeyCode key in movementKeys)
        {
            movementKeyPressed = movementKeyPressed||Input.GetKey(key);
        }
        foreach (KeyCode key in shootKeys)
        {
            shootKeyPressed = shootKeyPressed||Input.GetKey(key);
        }
        foreach (KeyCode key in slowMoveKeys)
        {
            slowMoveKeyPressed = slowMoveKeyPressed||Input.GetKey(key);
        }
        
    }
}
