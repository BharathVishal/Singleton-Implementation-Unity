using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Singleton_Controller : MonoBehaviour
{

    //---------------------------------------------------------------------------Globals Container Related-------------------------------------------------------------------------
    [Header("Globals Container Related")]
    [Space(10)]
    public int score;

    //Singleton Logic
    public static Singleton_Controller Instance;

    void Awake()
    {
        //Check if there is already an instance of Singleton_Controller
        if (Instance == null)
        {
            //if not, set it to this.
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If instance already exists
        else if (Instance != this && Instance != null)
        {
            //Destroy this, this applies our singleton pattern so there can only be one instance of Singleton_Controller.
            Destroy(gameObject);
        }
    }

    public void increase_Score()
    {
        score += 1;
    }

    // Use this for initialization
    void Start()
    {
        score = 0;
    }



}
