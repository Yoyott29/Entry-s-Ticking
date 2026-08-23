using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class Manage_Question : MonoBehaviour
{
    public WordList questionsList;
    public WordList answersList;
    public GameObject answerPrefab;

    private GameObject questionGO;
    private GameObject answersGO;

    private GameObject deathFadeRectangle;
    private GameObject deathMenuGO;

    private List<GameObject> notes;
    private List<GameObject> answerButtons;

    private string correctAnswer;
    private GameObject selectedNotes;
    private int answerButtonNumber = 3;
    public static bool waitingForAnswer = false;



    void Start()
    {
        questionGO = GameObject.Find("Question");
        answersGO = GameObject.Find("Answers");
        notes = new List<GameObject>();
        answerButtons = new List<GameObject>();
        deathFadeRectangle = GameObject.Find("Death Fade Rectangle");
        deathMenuGO = GameObject.Find("Death Menu");
        deathMenuGO.SetActive(false);

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

        Debug.Log("Correct Answer: " + correctAnswer);
        Debug.Log("Encrypted Correct Answer: " + EncryptSentence(correctAnswer, false));
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
        if (Input.GetMouseButtonDown(0) && !waitingForAnswer)
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

                    StartCoroutine(AnswerResultCoroutine(selectedAnswer.ToLower() == correctAnswer.ToLower(), answerButton));
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

    IEnumerator AnswerResultCoroutine(bool isCorrect, GameObject answerButton)
    {
        waitingForAnswer = true;

        Manage_Sounds.PlaySFX("Question Timer", gameObject, false);

        yield return new WaitForSeconds(2f);

        if (isCorrect)
            Manage_Sounds.PlaySFX("Correct Answer", gameObject, false);
        else
            Manage_Sounds.PlaySFX("Wrong Answer", gameObject, false);

        for (int i = 0; i < answerButtons.Count; i++)
        {
            GameObject button = answerButtons[i];
            string buttonAnswer = DecryptSentence(button.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text).TrimEnd('.');

            if (buttonAnswer.ToLower() == correctAnswer.ToLower())
                button.GetComponent<Image>().color = new Color32(96, 152, 65, 255); // Green for correct answer
            else if (button == answerButton)
                button.GetComponent<Image>().color = new Color32(150, 75, 65, 255); // Red for incorrect answer
        }

        yield return new WaitForSeconds(2f);

        if (isCorrect)
            Manage_Dictionary.questionAnsweredCorrectly = true;
        else
            yield return StartCoroutine(LaunchDeath());

        waitingForAnswer = false;
    }

    IEnumerator LaunchDeath()
    {
        int alpha = 0;

        Manage_Sounds.PlaySFX("Death", gameObject, false);

        while (alpha < 255)
        {
            alpha += 5;
            deathFadeRectangle.GetComponent<Image>().color = new Color32(0, 0, 0, (byte)alpha);
            yield return new WaitForSeconds(0.01f);
        }
        deathMenuGO.SetActive(true);
        
        yield return new WaitForSeconds(1f);

    }

    public void ReturnToMenu()
    {
        GameManager.ResetAndReturnToMenu();
    }


}
