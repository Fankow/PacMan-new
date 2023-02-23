using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class game_manager : MonoBehaviour{
    public readonly int UP=0,DOWN=3,LEFT=1,RIGHT=2;//sum of opposite direction==3
    public int row,column;
    public int frameRate;


    private int pelletAte,pelletNumber;
    private int EnergizerAte,EnergizerNumber;
    private int lives,scores;
    private int scoresPerPellet=10,scoresPerGhost=100;
    private int level=1,maxLevel=10;
    void Awake(){
        frameRate=25;
        Application.targetFrameRate=frameRate;
    }

    private void LevelUp(){
        scoresPerPellet++;
        scoresPerGhost+=40;
    }

    public bool EatPellet(){
        pelletAte++;
        ChangeScores(scoresPerPellet);
        if(pelletAte==pelletNumber){
            if(level==maxLevel){
                //game end
                return false;
            }
            else{
                LevelUp();
                return true;
            }
        }
        else{
            return false;
        }
    }
    public void EatGhost(){
        ChangeLives(1);
        ChangeScores(scoresPerGhost);
    }


    private void ChangeLives(int val){
        lives+=val;
    }

    private void ChangeScores(int val){
        scores+=val;
    }

}
