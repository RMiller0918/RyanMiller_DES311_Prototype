using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
