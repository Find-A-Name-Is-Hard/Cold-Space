using UnityEngine;
using System.Collections;
using Ami.BroAudio;

[CreateAssetMenu(fileName = "ActionResponse", menuName = "Scriptable Objects/ActionResponse")]
public class ActionResponse : ScriptableObject
{
    

    public bool shuffleOnCreate;
    public bool addBackRandomness;
    public bool repeat;
    public bool canBeCalledTwiceInARow = false;
    public int priority = 0;
    public int interruptAtOrBelowPriority = -1;
    public float maxTimeRelevant;
    public float minTimeBetweenRepetition;
    public validActions actionName;
    public DialogueActionVariations actionVariation;
    public TextAsset[] linesToRead;
    public Ami.BroAudio.SoundID[] soundIds;
}
