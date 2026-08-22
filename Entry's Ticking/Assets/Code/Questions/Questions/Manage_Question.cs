using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class Manage_Question : MonoBehaviour
{
    public WordList questionsList;
    public WordList answersList;
    public GameObject answerPrefab;

    private GameObject questionGO;
    private GameObject answersGO;

    private List<GameObject> notes;
    private List<GameObject> answerButtons;

    private string correctAnswer;
    private GameObject selectedNotes;
    private int answerButtonNumber = 3;



    void Start()
    {
        questionGO = GameObject.Find("Question");
        answersGO = GameObject.Find("Answers");
        notes = new List<GameObject>();
        answerButtons = new List<GameObject>();

        GameObject questionNotes = GameObject.Find("Question Notes");

        notes.Add(questionNotes);
        PickRandomQuestion();
    }

    void PickRandomQuestion()
    {
        if (answersList.words.Count < answerButtonNumber)
        {
            Debug.LogError("Not enough answers in answersList to fill all answer buttons.");
            return;
        }

        int randomIndex = Random.Range(0, questionsList.words.Count);
        string selectedQuestion = questionsList.words[randomIndex];
        correctAnswer = answersList.words[randomIndex];

        questionGO.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = EncryptSentence(selectedQuestion, true);

        CreateAnswerButtons(correctAnswer);
    }

    void CreateAnswerButtons(string correctAnswer)
    {
        int randomCorrectButtonIndex = Random.Range(0, answerButtonNumber);
        List<int> usedIndices = new List<int>();

        for (int i = 0; i < answerButtonNumber; i++)
        {
            GameObject newAnswer = Instantiate(answerPrefab, answersGO.transform);
            GameObject newAnswerNotes = newAnswer.transform.Find("Notes").gameObject;

            notes.Add(newAnswerNotes);
            answerButtons.Add(newAnswer);
            newAnswer.name = "Answer " + i;
            newAnswerNotes.name = "Answer " + i + " Notes";

            if (i == randomCorrectButtonIndex)
                newAnswer.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = EncryptSentence(correctAnswer, false);
            else
            {
                int randomWrongAnswerIndex = Random.Range(0, answersList.words.Count);

                while (usedIndices.Contains(randomWrongAnswerIndex) || answersList.words[randomWrongAnswerIndex] == correctAnswer)
                    randomWrongAnswerIndex = Random.Range(0, answersList.words.Count);
                usedIndices.Add(randomWrongAnswerIndex);
                newAnswer.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = EncryptSentence(answersList.words[randomWrongAnswerIndex], false);
            }
        }
    }

    void Update()
    {
        CheckNotesClick();
        CheckNotesInput();

        CheckAnswerClick();
    }

    void CheckNotesClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < notes.Count; i++)
            {
                GameObject note = notes[i];
                RectTransform rectTransform = note.GetComponent<RectTransform>();
                Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);

                if (rectTransform.rect.Contains(localMousePosition))
                {
                    SelectNotes(note);
                    break;
                }
            }
        }
    }

    public void SelectNotes(GameObject note)
    {
        foreach (GameObject possibleNote in notes)
        {
            if (possibleNote != note)
                possibleNote.GetComponent<Image>().color = new Color32(192, 184, 172, 255);
        }
        note.GetComponent<Image>().color = Color.grey;
        selectedNotes = note;
    }

    public void CheckNotesInput()
    {
        foreach (char c in Input.inputString)
        {
            if (selectedNotes != null)
            {
                TMPro.TextMeshProUGUI textComponent = selectedNotes.transform.Find("Notes Text").GetComponent<TMPro.TextMeshProUGUI>();
                if (textComponent.text.Length >= 74 && c != '\b') // Limit to 74 characters, allow backspace
                    continue;

                if (c == '\b') // Backspace
                {
                    if (textComponent.text == "Notes...")
                        textComponent.text = "";
                    if (textComponent.text.Length > 0)
                        textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 1);
                    if (textComponent.text.Length == 0)
                        textComponent.text = "Notes...";
                }
                else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == ' ') // Letters and space
                {
                    if (textComponent.text == "Notes...")
                        textComponent.text = "";
                    textComponent.text += c;
                }
             }
         }
    }

    public void CheckAnswerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < answerButtons.Count; i++)
            {
                GameObject answerButton = answerButtons[i];
                RectTransform rectTransform = answerButton.GetComponent<RectTransform>();
                Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
                if (rectTransform.rect.Contains(localMousePosition))
                {
                    string selectedAnswer = DecryptSentence(answerButton.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text);
                    
                    selectedAnswer = selectedAnswer.TrimEnd('.'); // Remove punctuation for comparison

                    if (selectedAnswer.ToLower() == correctAnswer.ToLower())
                        Debug.Log("Correct Answer!");
                    else
                        Debug.Log("Wrong Answer!");
                    break;
                }
            }
        }
    }


    public string EncryptSentence(string sentence, bool question)
    {
        string[] words = sentence.Split(' ');
        string newSentence = "";

        for (int i = 0; i < words.Length; i++)
        {
            string translatedWord = Manage_Dictionary.fullDictionary.Find(x => x.originalWord.ToLower() == words[i].ToLower()).translatedWord;
            newSentence += translatedWord;

            if (i < words.Length - 1)
                newSentence += " ";
        }

        if (question)
            newSentence += " ?";
        else
            newSentence += ".";
        return newSentence;
    }

    public string DecryptSentence(string sentence)
    {
        sentence = sentence.TrimEnd('.');
        string[] words = sentence.Split(' ');
        string newSentence = "";

        for (int i = 0; i < words.Length; i++)
        {
            string originalWord = Manage_Dictionary.fullDictionary.Find(x => x.translatedWord.ToLower() == words[i].ToLower()).originalWord;
            newSentence += originalWord;

            if (i < words.Length - 1)
                newSentence += " ";
        }
    
        return newSentence;
    }
}
