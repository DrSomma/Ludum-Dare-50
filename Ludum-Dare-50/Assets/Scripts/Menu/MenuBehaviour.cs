using Manager;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    public void ClickStartButton()
    {
        LevelManager.Instance.LoadFirstLevel();
    }   
    
    public void OnHomeClicked()
    {
        LevelManager.Instance.LoadMainMenu();
    } 

    public void ClickStatisticButton()
    {
        // Todo Statistic
    }
    
    public void ClickQuitButton()
    {
        Application.Quit();
    }
}
