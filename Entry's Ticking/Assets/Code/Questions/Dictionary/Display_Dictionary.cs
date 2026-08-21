using TMPro;
using UnityEngine;

public class Display_Dictionary : MonoBehaviour
{
    public GameObject wordEquivalenceLinePrefab;
    void Start()
    {
        // foreach (var word in Manage_Dictionary.availableDictionary)
        foreach (var word in Manage_Dictionary.fullDictionary)
        {
            string wordEquivalenceLine = word.translatedWord + " = " + word.originalWord;

            GameObject newLine = Instantiate(wordEquivalenceLinePrefab, transform);

            newLine.name = word.originalWord;

            newLine.GetComponent<TextMeshProUGUI>().text = wordEquivalenceLine;
        }
    }
}
