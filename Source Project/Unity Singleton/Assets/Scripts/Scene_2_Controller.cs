using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Scene_2_Controller : MonoBehaviour {


    public Text score_Text;


    public void increase_Score()
    {
		Singleton_Controller.Instance.score+=1;
        score_Text.text = "Score : " + Singleton_Controller.Instance.score;

    }


    public void goto_Scene_1()
    {
        print("invoked");
       SceneManager.LoadScene("First_Scene");
    }


       void Start()
    {
        score_Text.text = "Score : " +Singleton_Controller.Instance.score;
    }

}
