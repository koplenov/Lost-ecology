using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{
    public GameObject IntroInfo;
    public GameObject GideGroup;
    public void ButtonLoadGameScene()
    {
        SceneManager.LoadScene("Scenes/Game");
    }
    public void ButtonShowInfo()
    {
        GideGroup.SetActive(false);
        IntroInfo.SetActive(true);
    }
}
