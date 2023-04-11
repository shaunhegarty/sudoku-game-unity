using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public BoardManager Board { get; private set; }

    public void RegisterBoard(BoardManager boardManager)
    {
        Board = boardManager;
    }

    /* Not sure if it's sustainable, but I've made a getter that sets D:
     * The benefit, in theory, is that now I don't need to put null checks 
     * everywhere or do anything special to ensure that it exists somewhere if I start from any scene other than the first
     * */
    private static GameManager instance; // use private lower case instance to be the true value
    public static GameManager Instance // then Instance is just property magic from then on
    {
        get { return GetInstance(); }
        private set { instance = value; }
    }

    private static GameManager GetInstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            GameObject newObject = new("GameManager");
            GameManager gameRunner = newObject.AddComponent<GameManager>();
            return gameRunner;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
