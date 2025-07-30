using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public string mainScene;

    public void StartGame()
    {
        SceneManager.LoadScene(mainScene);
    }

}
