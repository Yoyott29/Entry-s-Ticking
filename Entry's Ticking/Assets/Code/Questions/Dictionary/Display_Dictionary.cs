using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public struct dictionaryPage
{
    public int pageNumber;
    public List<wordEquivalence> wordsOnPage;
}

public class Display_Dictionary : MonoBehaviour
{
    public GameObject wordEquivalenceLinePrefab;
    public GameObject dictionaryPagePrefab;
    public List<dictionaryPage> dictionaryPages;

    private GameObject leftPageNumberGO;
    private GameObject rightPageNumberGO;

    private Button previousPageButton;
    private Button nextPageButton;

    private Animator pageAnimator;
    private Animator pageShadowsAnimator;

    private GameObject pageGO;

    private int currentPageNumber;

    void Start()
    {
        leftPageNumberGO = GameObject.Find("Left Page Number");
        rightPageNumberGO = GameObject.Find("Right Page Number");

        previousPageButton = GameObject.Find("Previous Page").GetComponent<Button>();
        nextPageButton = GameObject.Find("Next Page").GetComponent<Button>();

        pageGO = GameObject.Find("Page");
        pageAnimator = pageGO.GetComponent<Animator>();
        pageShadowsAnimator = GameObject.Find("Page Shadows").GetComponent<Animator>();

        currentPageNumber = 1;
        CreatePages();
        InstantiatePages();
        pageGO.transform.SetSiblingIndex(currentPageNumber + 1 + 5); // Adjust the sibling index to ensure the new pages are on top
    }

    void CreatePages()
    {
        dictionaryPages = new List<dictionaryPage>();

        int pageNumber = 0;
        int wordsPerPage = 16;
        int wordCount = 0;

        // foreach (var word in Manage_Dictionary.availableDictionary)
        foreach (var word in Manage_Dictionary.fullDictionary)
        {
            if (wordCount % wordsPerPage == 0)
            {
                dictionaryPage newPage = new dictionaryPage();
                pageNumber++;
                newPage.pageNumber = pageNumber;
                newPage.wordsOnPage = new List<wordEquivalence>();
                dictionaryPages.Add(newPage);
            }
            dictionaryPages[dictionaryPages.Count - 1].wordsOnPage.Add(word);
            wordCount++;
        }
        if (pageNumber % 2 == 1) // If the last page has an odd number, create an empty page to make it even
        {
            dictionaryPage newPage = new dictionaryPage();
            pageNumber++;
            newPage.pageNumber = pageNumber;
            newPage.wordsOnPage = new List<wordEquivalence>();
            dictionaryPages.Add(newPage);
        }
    }

    void InstantiatePages()
    {
        foreach (var page in dictionaryPages)
        {
            GameObject pageGO = Instantiate(dictionaryPagePrefab, transform);
            pageGO.name = "Page " + page.pageNumber;

            if (page.pageNumber % 2 == 1)
                pageGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(50, -30); // Position for odd pages
            else
                pageGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, -30); // Position for even pages

            foreach (var word in page.wordsOnPage)
            {
                GameObject line = Instantiate(wordEquivalenceLinePrefab, pageGO.transform);
                line.transform.GetComponent<TextMeshProUGUI>().text = word.translatedWord + " - " + word.originalWord;
                line.name = word.translatedWord;
            }
            if (page.pageNumber > 2)
                pageGO.SetActive(false); // Hide all pages except the first two
        }
    }

    public void NextPage()
    {
        if (currentPageNumber + 2 <= dictionaryPages.Count)
        {
            pageAnimator.SetTrigger("NextPage");
            pageShadowsAnimator.SetTrigger("NextPage");

            // SetPageActive(currentPageNumber, false);
            SetPageActive(currentPageNumber + 1, false);

            currentPageNumber += 2;

            // SetPageActive(currentPageNumber, true);
            SetPageActive(currentPageNumber + 1, true);

            pageGO.transform.SetSiblingIndex(currentPageNumber + 1 + 5); // Adjust the sibling index to ensure the new pages are on top

            leftPageNumberGO.GetComponent<TextMeshProUGUI>().text = currentPageNumber.ToString();
            rightPageNumberGO.GetComponent<TextMeshProUGUI>().text = (currentPageNumber + 1).ToString();

            previousPageButton.interactable = true;
            if (currentPageNumber + 2 > dictionaryPages.Count)
                nextPageButton.interactable = false;
            else
                nextPageButton.interactable = true;
        }
    }

    public void PreviousPage()
    {
        if (currentPageNumber > 1)
        {
            pageAnimator.SetTrigger("PreviousPage");
            pageShadowsAnimator.SetTrigger("PreviousPage");

            // SetPageActive(currentPageNumber, false);
            // SetPageActive(currentPageNumber + 1, false);

            currentPageNumber -= 2;

            // SetPageActive(currentPageNumber, true);
            // SetPageActive(currentPageNumber + 1, true);

            pageGO.transform.SetSiblingIndex(currentPageNumber + 1 + 5); // Adjust the sibling index to ensure the new pages are on top

            leftPageNumberGO.GetComponent<TextMeshProUGUI>().text = currentPageNumber.ToString();
            rightPageNumberGO.GetComponent<TextMeshProUGUI>().text = (currentPageNumber + 1).ToString();

            nextPageButton.interactable = true;
            if (currentPageNumber == 1)
                previousPageButton.interactable = false;
            else
                previousPageButton.interactable = true;
        }
    }

    private void SetPageActive(int pageNumber, bool active)
    {
        Transform page = transform.Find("Page " + pageNumber);

        if (page != null)
            page.gameObject.SetActive(active);
    }



    //Next Page
    //1  -> change Right Page (Next Page)
    //2 -> change Left Page (Previous Page)
    //3
    //4 -> Change Left Page (Next Page) / Change Right Page (Previous Page)
    //5
}
