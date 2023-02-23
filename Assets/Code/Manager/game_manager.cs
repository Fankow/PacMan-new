using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class game_manager : MonoBehaviour{
    public readonly int UP=0,DOWN=3,LEFT=1,RIGHT=2;//sum of opposite direction==3
    public int row,column;
    public int frameRate;
    public Sprite pelletSprite,energizerSprite;


    private int pelletAte,pelletNumber;
    [SerializeField]private int energizerNumber,energizerLeast;
    private int energizerAte;
    private int lives,scores;
    private int scoresPerPellet=10,scoresPerGhost=100;
    private int level=1,maxLevel=10;
    void Awake(){
        frameRate=25;
        Application.targetFrameRate=frameRate;
    }

    void Start(){
        StartCoroutine(WaitForGenerator());
    }
    IEnumerator WaitForGenerator(){
        while(GameObject.Find("Node Generator")){
            yield return null;
        }//wait for finish generation of all nodes
        SpawnEnergizer();
        yield break;
    }
    private void SpawnEnergizer(){
        GameObject[] pellets=GameObject.FindGameObjectsWithTag("node_pellet");
        //randomly pick energizerNumber GameObjects from pellets and
        //turn their tag to "node_energizer" and sprite to energizerSprite
    }


    private void LevelUp(){
        scoresPerPellet++;
        scoresPerGhost+=40;

        if(energizerNumber>energizerLeast){
            energizerNumber--;
        }
        SpawnEnergizer();
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
    public void EatEnergizer(){
        energizerAte++;
    }


    private void ChangeLives(int val){
        lives+=val;
    }

    private void ChangeScores(int val){
        scores+=val;
    }

}
