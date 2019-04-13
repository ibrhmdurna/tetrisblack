using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Game : MonoBehaviour {

    public static int gridWidth = 10;
    public static int gridHeight = 19;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingAtLevelZero;
    public static int startingLevel;

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public GameObject hud_menu;
    public GameObject paused_menu;

    public Text hud_score;
    public Text hud_level;

    public AudioClip lineClearedSound;

    public static float fallSpeed = 1.0f;
    public static int currentScore = 0;

    public GameObject UI;

    private GameObject nextTetromino;
    private GameObject previewTetormino;
    private GameObject savedTetromino;
    private GameObject ghostTetromino;

    private int numberOfRowsThisTurn = 0;

    private Vector2 previewTetrominoPosition = new Vector2(-4f, 15f);
    private Vector2 savedTetrominoPosition = new Vector2(-4f, 9f);

    private AudioSource audioSource;

    private Shake shake;

    private bool gameStarted = false;
    public static bool isPaused = false;

    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3;

    public int maxSwaps = 2;
    private int currentSwaps = 0;

    // Use this for initialization
    void Start () {

        currentScore = 0;

        currentLevel = startingLevel;

        hud_level.text = currentLevel.ToString();

        SpawnNextTetromino();

        audioSource = GetComponent<AudioSource>();

        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3");

        shake = GameObject.FindGameObjectWithTag("ScreenShake").GetComponent<Shake>();
	}
	
	// Update is called once per frame
	void Update () {
        
        UpdateUI();

        UpdateScore();

        UpdateLevel();

        UpdateSpeed();

        CheckUserInput();
    }

    private void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (Time.timeScale == 1)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SaveTetromino(tempNextTetromino.transform);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        UI.GetComponent<UIController>().enabled = true;
        isPaused = true;
        audioSource.Pause();
        paused_menu.SetActive(true);
        hud_menu.SetActive(false);
        Camera.main.GetComponent<Blur>().enabled = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        UI.GetComponent<UIController>().enabled = false;
        isPaused = false;
        audioSource.Play();
        paused_menu.SetActive(false);
        hud_menu.SetActive(true);
        Camera.main.GetComponent<Blur>().enabled = false;
    }

    private void UpdateLevel()
    {
        if ((startingAtLevelZero == true) || (startingAtLevelZero == false && numLinesCleared / 10 > startingLevel))
        {
            currentLevel = numLinesCleared / 10;
        }

    }

    private void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    private void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
    }

    private void UpdateScore()
    {
        switch (numberOfRowsThisTurn)
        {
            case 1:
                ClearedOneLine();
                break;
            case 2:
                ClearedTwoLine();
                break;
            case 3:
                ClearedThreeLine();
                break;
            case 4:
                ClearedFourLine();
                break;
        }

        numberOfRowsThisTurn = 0;
    }

    public void ClearedOneLine()
    {
        currentScore += scoreOneLine + (currentLevel * 20);
        numLinesCleared++;
    }

    public void ClearedTwoLine()
    {
        currentScore += scoreTwoLine + (currentLevel * 25);
        numLinesCleared += 2;
    }

    public void ClearedThreeLine()
    {
        currentScore += scoreThreeLine + (currentLevel * 30);
        numLinesCleared += 3;
    }

    public void ClearedFourLine()
    {
        currentScore += scoreFourLine + (currentLevel * 40);
        numLinesCleared += 4;
    }

    public void UpdateHighScore()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", startingHighScore);
            PlayerPrefs.SetInt("highscore", currentScore);
        }
        else if (currentScore > startingHighScore2)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }
        else if (currentScore > startingHighScore3)
        {
            PlayerPrefs.SetInt("highscore3", currentScore);
        }

        PlayerPrefs.SetInt("lastscore", currentScore);
    }

    public void DeleteRow()
    {
        for(int y = 0; y < gridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DestroyRow(y);

                MoveRowDown(y + 1);

                PlayLineClearedAudio();

                shake.CamShake();

                y--;
            }
        }
    }

    private void MoveRowDown(int y)
    {
        for(int i = y; i < gridHeight; i++)
        {
            MoveMinoDown(i);
        }
    }

    private void MoveMinoDown(int y)
    {
        for(int x = 0; x < gridWidth; x++)
        {   
            if(grid[x,y] != null)
            {
                grid[x, y].position += new Vector3(0, -1, 0);

                grid[x, y - 1] = grid[x, y];

                grid[x, y] = null;
            }
        }
    }

    private void DestroyRow(int y)
    {
        for(int x = 0; x < gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }
    }

    private bool IsFullRowAt(int y)
    {
        for(int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null)
                return false;
        }

        numberOfRowsThisTurn++;

        return true;
    }

    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
        
    }

    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20f), Quaternion.identity);
            previewTetormino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            string prefabName = previewTetormino.GetComponent<Tetromino>().prefabName;
            if (prefabName.Equals("Tetromino_Square"))
            {
                previewTetormino.transform.localPosition = new Vector2(-4.5f, 15f);
            }
            else if (prefabName.Equals("Tetromino_Long"))
            {
                previewTetormino.transform.localPosition = new Vector2(-3.5f, 15.5f);
            }
            previewTetormino.GetComponent<Tetromino>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";

            SpawnGhostTetromino();
        }
        else
        {
            previewTetormino.transform.localPosition = new Vector2(5f, 20f);
            nextTetromino = previewTetormino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;


            previewTetormino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            string prefabName = previewTetormino.GetComponent<Tetromino>().prefabName;
            if (prefabName.Equals("Tetromino_Square"))
            {
                previewTetormino.transform.localPosition = new Vector2(-4.5f, 15f);
            }
            else if (prefabName.Equals("Tetromino_Long"))
            {
                previewTetormino.transform.localPosition = new Vector2(-3.5f, 15.5f);
            }
            previewTetormino.GetComponent<Tetromino>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";

            SpawnGhostTetromino();
        }

        currentSwaps = 0;
    }

    public void SpawnGhostTetromino()
    {
        if (GameObject.FindGameObjectWithTag("currentGhostTetromino") != null)
            Destroy(GameObject.FindGameObjectWithTag("currentGhostTetromino"));

        ghostTetromino = (GameObject)Instantiate(nextTetromino, nextTetromino.transform.position, Quaternion.identity);

        Destroy(ghostTetromino.GetComponent<Tetromino>());

        ghostTetromino.AddComponent<GhostTetromino>();
    }

    private void SaveTetromino(Transform t)
    {
        currentSwaps++;

        if (currentSwaps > maxSwaps)
            return;

        if(savedTetromino != null)
        {
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavedTetromino");

            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);

            if (!CheckIsValidPosition(tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;
                return;
            }

            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";
            string prefabName = savedTetromino.GetComponent<Tetromino>().prefabName;
            if (prefabName.Equals("Tetromino_Square"))
            {
                savedTetromino.transform.localPosition = new Vector2(-4.5f, 9f);
            }
            else if (prefabName.Equals("Tetromino_Long"))
            {
                savedTetromino.transform.localPosition = new Vector2(-3.5f, 9.5f);
            }
            savedTetromino.transform.localRotation = Quaternion.identity;
            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);

            SpawnGhostTetromino();
        }
        else
        {
            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavedTetromino";
            string prefabName = savedTetromino.GetComponent<Tetromino>().prefabName;
            if (prefabName.Equals("Tetromino_Square"))
            {
                savedTetromino.transform.localPosition = new Vector2(-4.5f, 9f);
            }
            else if (prefabName.Equals("Tetromino_Long"))
            {
                savedTetromino.transform.localPosition = new Vector2(-3.5f, 9.5f);
            }
            savedTetromino.transform.localRotation = Quaternion.identity;
            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));

            SpawnNextTetromino();
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public bool CheckIsAboveGrid(Tetromino tetromino)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);

                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    private bool CheckIsValidPosition(GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (!CheckIsInsideGrid(pos))
                return false;

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
                return false;
        }

        return true;
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    private void PlayLineClearedAudio()
    {
        audioSource.PlayOneShot(lineClearedSound);
    }

    private string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);

        string randomTetrominoName = "Tetromino_T";

        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Tetromino_T";
                break;
            case 2:
                randomTetrominoName = "Tetromino_Long";
                break;
            case 3:
                randomTetrominoName = "Tetromino_Square";
                break;
            case 4:
                randomTetrominoName = "Tetromino_J";
                break;
            case 5:
                randomTetrominoName = "Tetromino_L";
                break;
            case 6:
                randomTetrominoName = "Tetromino_S";
                break;
            case 7:
                randomTetrominoName = "Tetromino_Z";
                break;
        }

        return randomTetrominoName;
    }

    public void GameOver()
    {
        UpdateHighScore();

        SceneManager.LoadScene("GameOver");
    }
}
