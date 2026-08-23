using UnityEngine;

public class Show_Explanations : MonoBehaviour
{
    GameObject Menu;
    GameObject Explanations;

    void Start()
    {
        Menu = GameObject.Find("Menu");
        Explanations = GameObject.Find("Explanations");

        Explanations.SetActive(false);
    }

    public void ShowExplanations()
    {
        Menu.SetActive(false);
        Explanations.SetActive(true);
    }
}
