using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneUI : MonoBehaviour
{
    public GameObject controlPanel;
    
    public void StartGame()
    {
        SceneManager.LoadScene("Final");
    }

    public void ShowControls()
    {
        controlPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlPanel.SetActive(false);
    }
}
