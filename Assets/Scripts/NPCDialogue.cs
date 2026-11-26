using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue",menuName = "NPC Dialogue")]
public class NPCdialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    public DialogueChoice[] choices;
} 
[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;//dialogue lines where choices appear
    public string[] choices; //player responce options
    public int[]nextDialogueIndexes;
}
