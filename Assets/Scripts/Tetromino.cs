using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour {

    float fall = 0;
    private float fallSpeed;
    public string prefabName;

    public bool allowRotation = true;
    public bool limitRotation = false;

    public AudioClip landSound;
    public AudioClip moveSound;
    public AudioClip rotateSound;

    public int individualScore = 100;

    private float continuousVerticalSpeed = 0.05f;
    private float continuousHorizontalSpeed = 0.01f;
    private float buttonDownWaitMax = 0.2f;

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitMaxTimerHorizontal = 0;
    private float buttonDownWaitMaxTimerVertical = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    private float individualScoreTime;
    //private bool moved = false;

    private AudioSource audioSource;

    private Shake shake;

    // Use this for initialization
    void Start () {

        audioSource = GetComponent<AudioSource>();

        if(shake != null)
            shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<Shake>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!Game.isPaused)
        {
            CheckUserInput();

            UpdateIndividualScore();

            UpdateFallSpeed();
        }
    }

    void UpdateFallSpeed()
    {
        fallSpeed = Game.fallSpeed;
    }

    void UpdateIndividualScore()
    {
        if (individualScore > 1)
        {
            individualScoreTime += Time.deltaTime;
        }
        else
        {
            individualScoreTime = 0;

            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false;
            horizontalTimer = 0;
            buttonDownWaitMaxTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitMaxTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SlamDown();
        }
    }

    void SlamDown()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Game>().UpdateGrid(this);

            FindObjectOfType<Game>().DeleteRow();

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game>().GameOver();
            }

            PlayLandAudio();
            FindObjectOfType<Game>().SpawnNextTetromino();

            Game.currentScore += individualScore;

            enabled = false;
            tag = "Untagged";
        }
    }

    void MoveLeft()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitMaxTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitMaxTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }
        horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    void MoveRight()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitMaxTimerHorizontal < buttonDownWaitMax)
            {
                buttonDownWaitMaxTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continuousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
        {
            movedImmediateHorizontal = true;
        }
        horizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
            PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    void MoveDown()
    {
        if (movedImmediateVertical)
        {
            if (buttonDownWaitMaxTimerVertical < buttonDownWaitMax)
            {
                buttonDownWaitMaxTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < continuousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateVertical)
        {
            movedImmediateVertical = true;
        }

        verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);

            if (Input.GetKey(KeyCode.DownArrow))
            {
                PlayMoveAudio();
            }
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);

            PlayLandAudio();

            FindObjectOfType<Game>().DeleteRow();

            if (FindObjectOfType<Game>().CheckIsAboveGrid(this))
            {
                FindObjectOfType<Game>().GameOver();
            }

            FindObjectOfType<Game>().SpawnNextTetromino();

            Game.currentScore += individualScore;

            enabled = false;
            tag = "Untagged";
        }

        fall = Time.time;
    }

    void Rotate()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }

            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
                PlayRotateAudio();
            }
            else
            {
                if (limitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    transform.Rotate(0, 0, -90);
                }
            }
        }
    }

    void PlayMoveAudio()
    {
        audioSource.PlayOneShot(moveSound);
    }

    void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }

    void PlayRotateAudio()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            
            if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
            {
                return false;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }

        return true;
    }
}
