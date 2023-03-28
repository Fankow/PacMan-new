using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class game_manager : MonoBehaviour{
    public readonly int UP=0,DOWN=3,LEFT=1,RIGHT=2;//sum of opposite direction==3

    public Sprite pelletSprite,energizerSprite;

    public int row,column;
    [HideInInspector]public int frameRate;
    [HideInInspector]public bool gameActive;

    
    private GameObject[] pellets;
    private Vector2 energizerNodeSize,pelletNodeSize;

    private int pelletAte,pelletNumber;
    [SerializeField]private int energizerSpawnNumber,energizerLeast;
    private int lives=3,scores;
    private int scoresPerPellet=10,scoresPerGhost=100;
    private int level=1,maxLevel=10;
    
    private Dictionary<int,int> banned;
    private List<int> energizersRemain;
    

    void Awake(){
        gameActive=false;
        frameRate=25;
        Application.targetFrameRate=frameRate;
    }

    void Start(){
        energizersRemain=new List<int>();
        banned=new Dictionary<int,int>();
        energizerNodeSize=new Vector2(0.8f,0.8f);
        pelletNodeSize=new Vector2(0.5f,0.5f);
        StartCoroutine(WaitForGenerator());
    }
    IEnumerator WaitForGenerator(){
        while(GameObject.Find("Node Generator")){
            yield return null;
        }//wait for finish generation of all nodes
        yield return new WaitForSeconds(0.2f);

        pellets=GameObject.FindGameObjectsWithTag("node_pellet");
        pelletNumber=pellets.Length;
        SpawnEnergizer(pelletNumber);
        
        yield break;
    }
    
    private void SpawnEnergizer(int choices){
        //randomly pick energizerSpawnNumber GameObjects from pellets and
        //turn their tag to "node_energizer" and sprite to energizerSprite
        int i,idx,x;
        int choice;
        //Debug.Log($"choices={choices}");
        for(i=0;i<energizerSpawnNumber;i++){
            idx=Random.Range(0,choices);
            //Debug.Log($"get {idx}");
            choices--;

            if(banned.TryGetValue(idx,out x)){
                if(idx==choices){
                    banned.Remove(idx);
                    //Debug.Log($"mapped, get {x}");
                }
                else{
                    int y;
                    choice=choices;
                    while(banned.TryGetValue(choice,out y)){choice=y;}
                    //see if choices was mapped
                    banned[idx]=choice;
                    //Debug.Log($"mapped, get {x} mapped {choice}");
                }
                idx=x;
                //pellets[idx] already becomes node_energizer
            }
            else{
                if(idx==choices){}
                else{
                    choice=choices;
                    while(banned.TryGetValue(choice,out x)){choice=x;}
                    //see if choices was mapped
                    banned.Add(idx,choice);
                    //Debug.Log($"map {idx} with {choice}");
                }
            }

            pellets[idx].tag="node_energizer";
            pellets[idx].GetComponent<SpriteRenderer>().sprite=energizerSprite;
            pellets[idx].transform.localScale=energizerNodeSize;
        }

        pelletNumber-=energizerSpawnNumber;
        banned.Clear();
        energizersRemain.Clear();
        gameActive=true;
    }


    IEnumerator LevelUp(){
        yield return new WaitForSeconds(0.2f);
        pelletAte=0;
        scoresPerPellet++;
        scoresPerGhost+=40;

        if(energizerSpawnNumber>energizerLeast){
            energizerSpawnNumber--;
        }

        int choices=pellets.Length;
        int i;

        for(i=0;i<pellets.Length;i++){
            pellets[i].GetComponent<SpriteRenderer>().enabled=true;
            pellets[i].GetComponent<BoxCollider2D>().enabled=true;
            if(pellets[i].CompareTag("node_energizer")){
                banned.Add(i,-1);
                energizersRemain.Add(i);
            }
        }
        pelletNumber=pellets.Length-energizersRemain.Count;

        for(i=0;i<energizersRemain.Count;i++){
            if(energizersRemain[i]<pelletNumber){
                do{
                    choices--;
                }while(banned.ContainsKey(choices));
                banned[energizersRemain[i]]=choices;
                //remap the last energizersRemain.Count indices to previous banned indices
            }
        }

        SpawnEnergizer(pelletNumber);
        //pelletNumber on map=total pellets-energizer left in previous level-number of energizer that will spawn
        GameObject.Find("Player").GetComponent<pacman_control>().PacmanRestart();
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
    public bool EatPacman(){
        ChangeLives(-1);
        ChangeScores(-150);
        if(lives<=0){
            return false;
        }
        else{
            return true;
        }
    }
    public void EatGhost(){
        ChangeLives(1);
        ChangeScores(scoresPerGhost);
    }
    public void EatEnergizer(GameObject node){
        node.transform.localScale=pelletNodeSize;
        node.GetComponent<SpriteRenderer>().sprite=pelletSprite;
        node.tag="node_pellet";
    }


    private void ChangeLives(int val){
        lives+=val;
    }

    private void ChangeScores(int val){
        scores+=val;
    }

}
