using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetromino : MonoBehaviour {

	// Use this for initialization
	void Start () {
        tag = "currentGhostTetromino";

        foreach (Transform mino in transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);
        }
    }
	
	// Update is called once per frame
	void Update () {
        FollowActiveTetromino();

        MoveDown();
    }

    void FollowActiveTetromino()
    {
        Transform currentActiveTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino").transform;

        transform.position = currentActiveTetromino.position;
        transform.rotation = currentActiveTetromino.rotation;
    }

    void MoveDown()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        if (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
        }
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);

            if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
                return false;

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent.tag == "currentActiveTetromino")
                return true;

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
                return false;
        }

        return true;
    }
}
