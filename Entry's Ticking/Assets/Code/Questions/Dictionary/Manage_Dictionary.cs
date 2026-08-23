using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct wordEquivalence
{
    public string originalWord;
    public string translatedWord;
    public int pageNumber;
}

public class Manage_Dictionary : MonoBehaviour
{
    public WordList originalWordsList;
    public WordList translatedWordsList;

    public int startingWordsCount = 14; // How many random words are already known at game start (+ "Key" which is always known)

    public static List<wordEquivalence> fullDictionary;
    public static List<wordEquivalence> availableDictionary;

    static public bool questionAnsweredCorrectly = false;

    void Awake()
    {
        if (fullDictionary == null)
        {
            availableDictionary = new List<wordEquivalence>();
            createDictionary();
        }
    }

    public void createDictionary()
    {
        if (originalWordsList.words.Count != translatedWordsList.words.Count)
        {
            Debug.LogError("Original and translated word lists must have the same number of words.");
            return;
        }

        fullDictionary = new List<wordEquivalence>();

        List<string> originalWordsListCopy = new List<string>(originalWordsList.words);
        List<string> translatedWordsListCopy = new List<string>(translatedWordsList.words);

        for (int i = 0; i < originalWordsListCopy.Count; i++)
        {
            wordEquivalence newWord;

            int randomIndex = Random.Range(0, translatedWordsListCopy.Count);

            newWord.originalWord = originalWordsListCopy[i];
            newWord.translatedWord = translatedWordsListCopy[randomIndex];
            newWord.pageNumber = 1;

            translatedWordsListCopy.RemoveAt(randomIndex);

            fullDictionary.Add(newWord);
        }

        wordEquivalence timeKey;
        timeKey.originalWord = "Key";
        timeKey.translatedWord = "Time";
        timeKey.pageNumber = 1;

        fullDictionary.Add(timeKey);

        // "Key" is always already known
        addWord("Key");
        List<string> alreadyUnlockedWords = new List<string> { "Key" };

        List<wordEquivalence> pickableWords = new List<wordEquivalence>(fullDictionary);
        pickableWords.RemoveAll(w => w.originalWord == "Key");

        int wordsToGive = Mathf.Min(startingWordsCount, pickableWords.Count);

        for (int i = 0; i < wordsToGive; i++)
        {
            int randomIndex = Random.Range(0, pickableWords.Count);
            wordEquivalence chosen = pickableWords[randomIndex];

            addWord(chosen.originalWord);
            alreadyUnlockedWords.Add(chosen.originalWord);

            pickableWords.RemoveAt(randomIndex);
        }

        if (WordPoolManager.instance != null)
        {
            List<string> allWords = new List<string>();
            foreach (var word in fullDictionary)
                allWords.Add(word.originalWord);

            WordPoolManager.instance.InitializePool(allWords, alreadyUnlockedWords);
        }
    }

    public static void addWord(string originalWord)
    {
        foreach (wordEquivalence word in fullDictionary)
        {
            if (word.originalWord == originalWord)
            {
                availableDictionary.Add(word);
                return;
            }
        }
    }
}