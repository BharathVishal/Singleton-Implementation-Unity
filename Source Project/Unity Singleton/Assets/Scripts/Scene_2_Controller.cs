/**
 *
 * Copyright 2018-2023 Bharath Vishal G.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 **/

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
