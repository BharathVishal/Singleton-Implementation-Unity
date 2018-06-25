using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Scene_1_Controller : MonoBehaviour {
    public Text score_Text;


public void update_Score()
{
    Singleton_Controller.Instance.increase_Score();
	        score_Text.text = "Score : " + Singleton_Controller.Instance.score;

}


    public void goto_Scene_2()
    {
        SceneManager.LoadScene("Second_Scene");
    }


 void Start() {
    	        score_Text.text = "Score : " + Singleton_Controller.Instance.score;
}
}
