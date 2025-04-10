using UnityEngine;
using UnityEngine.SceneManagement;

public class BigRedButtonHandler : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
