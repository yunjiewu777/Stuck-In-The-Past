using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchImagePauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public float GameIsPaused;
    public GameObject pauseMenuUI;
    public bool pauseShown = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if (pauseShown)
            {
                Resume();
            }
            else
            {
                GameIsPaused = Time.timeScale;
                Pause();
            }
        }
    }

    public void Resume ()
    {
        pauseShown = !pauseShown;
        pauseMenuUI.SetActive(false);
        Time.timeScale = GameIsPaused;
    }

    void Pause ()
    {
        pauseShown = !pauseShown;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LoadMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Word_Match_MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Word_Match");
    }

    public void UpdateScore()
    {
        SaveManager.GetInstance().minigameM.score[SaveManager.GetInstance().level.difficultyLevel - 1] = 5;            
    }
}
