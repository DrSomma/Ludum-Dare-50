using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    public void ClickStartButton()
    {
        LevelManager.Instance.LoadNextLevel();
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
