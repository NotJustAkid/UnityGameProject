using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCdialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    public bool CanInteract()
    {
        return !isDialogueActive;
    }
    
    public void Interact()
    {
        if(dialogueData == null || (PauseController.IsGamePaused && isDialogueActive))
        {
            return;
        }
        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }
        void StartDialogue()
        {
            isDialogueActive = true;
            dialogueIndex = 0;

            dialogueUI.SetNPCInfo(dialogueData.npcName,dialogueData.npcPortrait);
            dialogueUI.ShowDialogueUI(true);
            PauseController.SetPause(true);

            DisplayCurrentLine();
        }
    void NextLine()
    {
        if (isTyping)
        {
            //skip typing naimation and show ui full line
            StopAllCoroutines(); 
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }

        //clear out existing choices
        dialogueUI.ClearChoices();
        //check endDialogueLines 
        if(dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
         {
            EndDialogue();    
            return;
         }
        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if(dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }
        //check if choice & display
        if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            //if another line, type next line
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
        {
            isTyping = true;
            dialogueUI.SetDialogueText("");
            

            foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
            { 
                dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
                yield return new WaitForSeconds(dialogueData.typingSpeed);
            }
            isTyping = false;

Debug.Log("FINISHED TYPING INDEX: " + dialogueIndex);

            if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
             Debug.Log("AUTO PROGRESS TRIGGERED AT: " + dialogueIndex);
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    void DisplayChoices(DialogueChoice choice)
    {
        Debug.Log("DISPLAY CHOICES CALLED!");
        for(int i = 0;i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            dialogueUI.CreateChoiceButton(choice.choices[i],() => ChooseOption(nextIndex) );
        }
    }
    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }
    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText ("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }
}

