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
