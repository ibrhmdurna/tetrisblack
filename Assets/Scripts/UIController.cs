using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

    public Text levelText;
    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;

    public Text lastScore;

    public GameObject focusButton;
    public GameObject button2;
    public GameObject button3;
    public Image arrow;
    public Button resumeButton;

    public AudioClip hightlightSound;
    public AudioClip pressedSound;

    public int buttonCount = 2;

    private AudioSource audioSource;

    // Use this for initialization
    void Start () {

        audioSource = GetComponent<AudioSource>();

        FocusButton();

        if (levelText != null)
            levelText.text = "0";

        if (highScoreText != null)
            highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();

        if (highScoreText2 != null)
            highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();

        if (highScoreText3 != null)
            highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();

        if (lastScore != null)
            lastScore.text = PlayerPrefs.GetInt("lastscore").ToString();

        //SetCursorState();
    }
	
	// Update is called once per frame
	void Update () {
        CheckUserInput();
	}

    public void FocusButton()
    {
        if (focusButton != null)
            EventSystem.current.SetSelectedGameObject(focusButton);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Level");
    }

    public void PlayGame()
    {
        if (Game.startingLevel == 0)
        {
            Game.startingAtLevelZero = true;
        }
        else
        {
            Game.startingAtLevelZero = false;
        }
        SceneManager.LoadScene("Level");
    }

    public void ChangedValue(float value)
    {
        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void GameMenu()
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || (Input.GetKeyDown(KeyCode.Return)))
        {
            PlayPressedAudio();
            if(buttonCount == 3)
            {
                FindObjectOfType<Game>().ResumeGame();
            }
        }
    }

    void MoveDown()
    {
        MoveDownArrow();
        if (arrow.transform.localPosition.y.Equals(-92f))
        {
            EventSystem.current.SetSelectedGameObject(focusButton);
        }
        else if (arrow.transform.localPosition.y.Equals(-142f))
        {
            EventSystem.current.SetSelectedGameObject(button2);
        }
        else
        {
            if (arrow.transform.localPosition.y.Equals(-192f))
            {
                EventSystem.current.SetSelectedGameObject(button3);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(focusButton);
            }
        }
    }

    void MoveUp()
    {
        MoveUpArrow();
        if (arrow.transform.localPosition.y.Equals(-92f))
        {
            EventSystem.current.SetSelectedGameObject(focusButton);
        }
        else if (arrow.transform.localPosition.y.Equals(-142f))
        {
            EventSystem.current.SetSelectedGameObject(button2);
        }
        else
        {
            if (arrow.transform.localPosition.y.Equals(-192f))
            {
                EventSystem.current.SetSelectedGameObject(button3);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(focusButton);
            }
        }
    }

    void MoveDownArrow()
    {
        PlayHightlightAudio();
        if (arrow.transform.localPosition.y.Equals(-92f))
        {
            arrow.transform.localPosition += new Vector3(0f, -50f);
        }
        else if (arrow.transform.localPosition.y.Equals(-142f))
        {
            if (buttonCount == 2)
                arrow.transform.localPosition += new Vector3(0f, 50f);
            if (buttonCount == 3)
                arrow.transform.localPosition += new Vector3(0f, -50f);
        }
        else
        {
            arrow.transform.localPosition += new Vector3(0f, 100f);
        }
    }

    void MoveUpArrow()
    {
        PlayHightlightAudio();
        if (arrow.transform.localPosition.y.Equals(-92f))
        {
            if (buttonCount == 2)
                arrow.transform.localPosition += new Vector3(0f, -50f);
            if (buttonCount == 3)
                arrow.transform.localPosition += new Vector3(0f, -100f);
        }
        else if (arrow.transform.localPosition.y.Equals(-142f))
        {
            arrow.transform.localPosition += new Vector3(0f, 50f);
        }
        else
        {
            arrow.transform.localPosition += new Vector3(0f, 50f);
        }
    }

    void PlayHightlightAudio()
    {
        audioSource.PlayOneShot(hightlightSound);
    }

    void PlayPressedAudio()
    {
        audioSource.PlayOneShot(pressedSound);
    }

    void SetCursorState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // Hide cursor when locking
        Cursor.visible = false;
    }
}
