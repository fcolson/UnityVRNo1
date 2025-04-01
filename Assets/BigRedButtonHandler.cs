using UnityEngine;
using UnityEngine.SceneManagement;

public class BigRedButtonHandler : MonoBehaviour
{
    // This method will be called when the button is pressed
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
