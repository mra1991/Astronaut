using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //declaration of singleton instance
    public static GameManager instance = null;

    //variables for score
    [SerializeField] TMP_Text tmpScore; //text mesh pro for score
    const string preText = "SCORE: ";
    const int ptsPerGem = 10; //points per gem 
    private int score = 0;
    private int bestScore;

    //variavles for lives and levels
    [SerializeField] private int lives = 3; //number of lives
    [SerializeField] TMP_Text tmpLives; //text mesh pro for lives
    private Locomotion astronaut; //the locomotion script is attached on the character
    [Tooltip("Attach your level prefabs here.")][SerializeField] private GameObject[] levels; //array of level objects
    private int currLevelNum = 0; //current level number
    [SerializeField] TMP_Text tmpLevel; //text mesh pro for level
    private GameObject currLevelObj; //current level object

    //variables for pause menu and game over
    private bool pause = false;
    private bool gameOver = false;
    [Tooltip("Attach UI button for Retry(Reloads the scene)")]
    [SerializeField] private GameObject btnRetry;
    [Tooltip("Attach UI button for Exit(Only works in the build and not the editor)")] 
    [SerializeField] private GameObject btnExit;
    [SerializeField] TMP_Text tmpStat; //text mesh pro for status (pause, game over, mission successful)
    private string statText = "Pause";
    [SerializeField] TMP_Text tmpHighScore; //text mesh pro for high score
    private string highScoreText = "High Score : ";




    private void Awake()
    {
        //singleton design
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f; //make sure time is not freezed (pause)
        bestScore = PlayerPrefs.GetInt("BestScore", 0); //retrieve best score from playerprefs
        astronaut = GameObject.FindObjectOfType<Locomotion>(); //retrieve the character object
        LoadLevel(); //instantiate first level
        HideMenu();
        DisplayScore();
        DisplayLives();
        DisplayLevel();
        DisplayHighScore();
        DisplayStat();
    }

    //refreshes the text for score
    void DisplayScore()
    {
        tmpScore.text = preText + score.ToString("D4");
    }

    //refreshes the text for level
    void DisplayLives()
    {
        tmpLives.text = "Lives: " + lives;
    }

    //refreshes the text for level
    void DisplayLevel()
    {
        tmpLevel.text = "Level: " + (currLevelNum + 1);
    }

    //refreshes the text for high score
    void DisplayHighScore()
    {
        tmpHighScore.text = highScoreText + bestScore.ToString("D4");
    }

    //refreshes the text for status
    void DisplayStat()
    {
        tmpStat.text = statText;
    }

    void ShowMenu()
    {
        btnRetry.SetActive(true);
        btnExit.SetActive(true);
        tmpHighScore.enabled = true;
        tmpStat.enabled = true;
    }

    void HideMenu()
    {
        btnRetry.SetActive(false);
        btnExit.SetActive(false);
        tmpHighScore.enabled = false;
        tmpStat.enabled = false;
    }

    public void Quit()
    {
        Application.Quit(); //only works in the build
    }

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }



    //function to be called by collision event when the character hits a gem
    public void AddPoints()
    {
        score += ptsPerGem; //add score
        DisplayScore(); //refresh the score
    }


    //to be called when the character lands on an object tagged Death
    public void Death()
    {
        lives--; //lose a life
        DisplayLives(); //refresh the text for lives
        if (lives > 0)
        {
            astronaut.Init();
        }
        else //GAME OVER
        {
            statText = "Game Over!";
            DisplayStat();
            GameOver();
        }
    }

    void GameOver()
    {
        SaveStats(); //save high score (and other stats if any)
        gameOver = true;
        ShowMenu();
        Destroy(astronaut.gameObject);
    }

    public void PauseOrPlay()
    {
        if (!gameOver)
        {
            if (!pause)
            {
                pause = true;
                Time.timeScale = 0f;
                ShowMenu();
            }
            else
            {
                pause = false;
                Time.timeScale = 1f;
                HideMenu();
            }
        }
        else if (!pause)
        {
            pause = true;
            ShowMenu();
        }
    }

    //save stats (high score for now)
    void SaveStats()
    {
        if (score > bestScore) //if new best score is achieved
        {
            highScoreText = "New " + highScoreText;
            bestScore = score;
            DisplayHighScore();
            PlayerPrefs.SetInt("BestScore", score); //record best score in playerprefs
            PlayerPrefs.Save();
        }
    }

    public void LevelUp()
    {
        if (currLevelNum < levels.Length - 1) //if there are more levels
        {
            currLevelNum++; //go to next level
            astronaut.Init(); //re-initialize the position of the player in the world
            LoadLevel(); //instantiate new level
            DisplayLevel(); //refresh the text for level
        }
        else //all levels finished successfully
        {
            statText = "Mission Successful!";
            DisplayStat();
            GameOver();
        }

    }

    private void LoadLevel()
    {
        //making sure current level object is null
        if (currLevelObj)
        {
            Destroy(currLevelObj);
        }
        //instantiate new level
        currLevelObj = Instantiate(levels[currLevelNum]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
