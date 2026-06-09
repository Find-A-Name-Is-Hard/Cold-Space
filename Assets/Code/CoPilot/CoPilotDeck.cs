using UnityEngine;

[CreateAssetMenu(fileName = "CoPilotDeck", menuName = "Scriptable Objects/CoPilotDeck")]
public class CoPilotDeck : ScriptableObject
{
    public string copilotName;
    public Sprite copilotSprite;
    public ActionResponse[] actionResponses;

}
