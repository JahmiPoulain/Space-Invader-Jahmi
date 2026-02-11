using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isPaused;
    private InputSystem_Actions controls;

    [SerializeField] int score = 0;
    public int lives = 3;
    public int highScore;

    [SerializeField]
    TMP_Text _scoreUI;
    [SerializeField]
    TMP_Text _highScoreUI;
    [SerializeField]
    TMP_Text _lifeUI;

    public GameObject UILife1;
    public GameObject UILife2;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        controls = new InputSystem_Actions();
        controls.UI.Pause.performed += ctx => Pause();
    }

    private void Start()
    {
        GetHighScore();       
    }

    private void OnEnable()
    {
        controls.UI.Pause.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Pause.Disable();
    }

    void Pause()
    {
        isPaused = !isPaused;
    }
    public void AddScore(int points)
    {
        score += points;

        if (score.ToString().Length <= 2)
        {
            _scoreUI.text = "00"+ score.ToString();
            return;
        }
        if (score.ToString().Length == 3)
        {
            _scoreUI.text = "0" + score.ToString();
            return;
        }
        _scoreUI.text = score.ToString();                
    }
    
    public void ResetScore() => score = 0;

    public void LoseLife()
    {
        if (PlayerScript.instance.respawnBool)
        {
            return;
        }
        lives--;
        PlayerScript.instance.HitByMissile();
        // on enleve les vies en bas a gauche de l'ecran
        if (lives == 2 && UILife2.activeSelf)
        {
            UILife2.SetActive(false);
        }
        else if (lives == 1 && UILife1.activeSelf)
        {
            UILife1.SetActive(false);
        }
        if (lives == 0)
        {
            GameOver();
        }
        _lifeUI.text = lives.ToString();
    }

    public void GameOver()
    {
        SaveScore();
    }

    public void CompletedLevel()
    {
        SaveScore();
        //Debug.Break();
    }

    void SaveScore()
    {
        if (score < highScore) return;

        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
    }

    void GetHighScore()
    {
        highScore = PlayerPrefs.GetInt("Score", score);

        if (highScore.ToString().Length <= 2)
        {
            _highScoreUI.text = "00" + highScore.ToString();
            return;
        }
        if (highScore.ToString().Length == 3)
        {
            _highScoreUI.text = "0" + highScore.ToString();
            return;
        }
        _highScoreUI.text = highScore.ToString();
    }
}
