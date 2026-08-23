using UnityEngine;
using UnityEngine.SceneManagement;

public class Launch_Game : MonoBehaviour
{
    public void LaunchRoom()
    {
        SceneManager.LoadScene("Room");
    }
}