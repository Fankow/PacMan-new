using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class game_manager : MonoBehaviour{
    public readonly int UP=0,DOWN=3,LEFT=1,RIGHT=2;
    public int row,column;
    public int frameRate;


    private int lives,scores;
    void Awake(){
        frameRate=25;
        Application.targetFrameRate=frameRate;
    }

}
