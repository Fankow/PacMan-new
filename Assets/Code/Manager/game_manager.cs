using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class game_manager : MonoBehaviour{
    public readonly int UP=0,DOWN=3,LEFT=1,RIGHT=2;//sum of opposite direction==3
    public int row,column;
    [HideInInspector]public int frameRate;
    public Sprite energizerSprite;

    
    private Vector2 energizerNodeSize;
    private int pelletAte,pelletNumber;
    [SerializeField]private int energizerNumber,energizerLeast;
    private int lives,scores;
    private int scoresPerPellet=10,scoresPerGhost=100;
    private int level=1,maxLevel=10;
    private Dictionary<int,int> banned;
    public bool gameActive;

    void Awake(){
        gameActive=false;
        frameRate=25;
        Application.targetFrameRate=frameRate;
    }

    void Start(){
        banned=new Dictionary<int,int>();
        energizerNodeSize=new Vector2(0.8f,0.8f);
        StartCoroutine(WaitForGenerator());
    }
    IEnumerator WaitForGenerator(){
        while(GameObject.Find("Node Generator")){
            yield return null;
        }//wait for finish generation of all nodes
        yield return new WaitForSeconds(0.2f);
        GameObject[] pellets=GameObject.FindGameObjectsWithTag("node_pellet");
        SpawnEnergizer(pellets);
        yield break;
    }
    
    private void SpawnEnergizer(GameObject[] pellets){
        //randomly pick energizerNumber GameObjects from pellets and
        //turn their tag to "node_energizer" and sprite to energizerSprite
        pelletNumber=pellets.Length;
        int i,idx;

        for(i=0;i<energizerNumber;i++){
            idx=Random.Range(0,pelletNumber);
            pelletNumber--;

            if(banned.ContainsKey(idx)){
                idx=banned[idx];
                //pellets[idx] already becomes node_energizer
            }

            pellets[idx].tag="node_energizer";
            pellets[idx].GetComponent<SpriteRenderer>().sprite=energizerSprite;
            pellets[idx].transform.localScale=energizerNodeSize;

            if(idx==pelletNumber){}
            else{
                banned.Add(idx,pelletNumber);
                //remap
            }
        }
        banned.Clear();
        gameActive=true;
    }


    IEnumerator LevelUp(){
        yield return new WaitForSeconds(0.2f);
        pelletAte=0;
        scoresPerPellet++;
        scoresPerGhost+=40;

        if(energizerNumber>energizerLeast){
            energizerNumber--;
        }

        GameObject[] pellets=GameObject.FindGameObjectsWithTag("node_pellet");
        GameObject.Find("Player").GetComponent<pacman_control>().PacmanRestart();
        for(int i=0;i<pellets.Length;i++){
            pellets[i].GetComponent<SpriteRenderer>().enabled=true;
            pellets[i].GetComponent<BoxCollider2D>().enabled=true;
        }
        SpawnEnergizer(pellets);
        yield break;
    }

    public void EatPellet(){
        pelletAte++;
        ChangeScores(scoresPerPellet);
        if(pelletAte==pelletNumber){
            gameActive=false;
            if(level==maxLevel){
                //pop message
            }
            else{
                StartCoroutine(LevelUp());
            }
        }
    }
    public void EatGhost(){
        ChangeLives(1);
        ChangeScores(scoresPerGhost);
    }
    public void EatEnergizer(){
    }


    private void ChangeLives(int val){
        lives+=val;
    }

    private void ChangeScores(int val){
        scores+=val;
    }

}
