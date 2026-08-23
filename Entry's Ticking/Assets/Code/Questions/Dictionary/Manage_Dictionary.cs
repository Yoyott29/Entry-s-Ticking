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
    public WordList originalWordsList; // Reference to the original words list
    public WordList translatedWordsList; // Reference to the translated words list

    public static List<wordEquivalence> fullDictionary; // Static list to hold the full dictionary of word equivalences
    public static List<wordEquivalence> availableDictionary; // Static list to hold the available dictionary of word equivalences

    static public bool questionAnsweredCorrectly = false; // Static boolean to track if the question was answered correctly


    void Awake()
    {
        availableDictionary = new List<wordEquivalence>();
        createDictionary();
    }

    public void createDictionary() // Randomly creates the full dictionary from the original and translated word lists
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
            newWord.pageNumber = 1; // Default page number, is modified later when the word is added to the available dictionary

            translatedWordsListCopy.RemoveAt(randomIndex); // Remove the used translated word to avoid duplicates

            fullDictionary.Add(newWord);
        }

        wordEquivalence timeKey;

        timeKey.originalWord = "Key";
        timeKey.translatedWord = "Time";
        timeKey.pageNumber = 1;

        fullDictionary.Add(timeKey);

        addWord("Key");
    }

    public static void addWord(string originalWord) // Adds a word to the available dictionary (if it exists in the full dictionary)
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
