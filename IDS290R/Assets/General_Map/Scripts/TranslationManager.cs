using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TranslationManager : MonoBehaviour
{
    [Header("Dialouge UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text displayNameText;
    [SerializeField] private string sceneToLoad;
    private bool loading = false;
    [SerializeField] private string culture;
    private const string CULTURE = "culture";

    [Header("Choice UI")]
    [SerializeField] private GameObject[] choices;
    private Text[] choicesText;

    private const string SPEAKER_TAG = "speaker";
    private const string SCENE = "scene";



    private Story currentStory;
    private static TranslationManager instance;
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
        if (currentStory.currentChoices.Count == 0 && Input.GetKeyDown(KeyCode.Space) && !loading)
            ContinueStory();
        if (sceneToLoad != "")
        {
            StartCoroutine(DelaySceneLoad());
        }
        if (culture == "true")
        {
            dialogueText.rectTransform.sizeDelta = new Vector2(225, 200);
        }
    }

    IEnumerator DelaySceneLoad()
    {
        loading = true;
        yield return new WaitForSeconds(0.5f); // Wait 1 seconds
        ExitDialogueMode();
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new Text[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<Text>();
            index++;
        }

    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTags = tag.Split(':');
            if (splitTags.Length != 2)
            {

            }
            string tagKey = splitTags[0].Trim();
            string tagValue = splitTags[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case SCENE:
                    sceneToLoad = tagValue;
                    break;
                case CULTURE:
                    culture = tagValue;
                    break;
                default:
                    break;
            }
        }

    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(false);

        displayNameText.text = "???";
        sceneToLoad = "";
        culture = "";

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialogueText.rectTransform.sizeDelta = new Vector2(225, 95);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
            HandleTags(currentStory.currentTags);
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

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }
    public static TranslationManager GetInstance()
    {
        return instance;
    }

    public void Click()
    {
        if (!dialogueIsPlaying)
            return;
        if (currentStory.currentChoices.Count == 0 && !loading)
            ContinueStory();
        if (sceneToLoad != "")
        {
            StartCoroutine(DelaySceneLoad());
        }
    }
}
