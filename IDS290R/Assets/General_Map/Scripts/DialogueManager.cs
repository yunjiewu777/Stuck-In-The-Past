using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{

    [Header("Dialouge UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choice UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;


    private Story currentStory;

    private static DialogueManager instance;
    public bool dialogueIsPlaying { get; private set; }


    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        instance = this;
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
            return;
        if (currentStory.currentChoices.Count == 0 && (Input.GetKeyDown(KeyCode.Space)|| Input.GetMouseButtonDown(0)))
            ContinueStory();
    }

    public static DialogueManager GetInstance()
    {
        return instance;        
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";    
    }


    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
        }
        else
            ExitDialogueMode();
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
            Debug.LogError("More choices were given than the UI can support. Number of choices given:" + currentChoices.Count);

        int index = 0;

        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
            choices[i].gameObject.SetActive(false);

        //StartCoroutine(SelectFirstChoice());
    }


    /*
    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }
    */

    public void MakeChoice(int choiceIndex) 
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

}
