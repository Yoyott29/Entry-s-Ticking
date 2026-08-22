using UnityEngine;

public class Display_Pages : MonoBehaviour
{
    
    private GameObject parentGO;
    void Start()
    {
        parentGO = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayNextPage(string pageToDisplay)
    {
        int currentPageIndex = transform.GetSiblingIndex();

        if (pageToDisplay == "Left") 
        {
            parentGO.transform.GetChild(currentPageIndex - 2).gameObject.SetActive(true);
            parentGO.transform.GetChild(currentPageIndex - 4).gameObject.SetActive(false);
        }
        else if (pageToDisplay == "Right")
        {
            parentGO.transform.GetChild(currentPageIndex - 1).gameObject.SetActive(true);
            parentGO.transform.GetChild(currentPageIndex - 3).gameObject.SetActive(false);
        }
    }

    public void displayPreviousPage(string pageToDisplay)
    {
        int currentPageIndex = transform.GetSiblingIndex();

        if (pageToDisplay == "Left") 
        {
            parentGO.transform.GetChild(currentPageIndex - 2).gameObject.SetActive(true);
            parentGO.transform.GetChild(currentPageIndex + 1).gameObject.SetActive(false);
        }
        else if (pageToDisplay == "Right")
        {
            parentGO.transform.GetChild(currentPageIndex - 1).gameObject.SetActive(true);
            parentGO.transform.GetChild(currentPageIndex + 2).gameObject.SetActive(false);
        }
    }

}
