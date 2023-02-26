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
    void Awake(){
        frameRate=25;
        Application.targetFrameRate=frameRate;
    }

    void Start(){
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
        pelletNumber=pellets.Length;
        //randomly pick energizerNumber GameObjects from pellets and
        //turn their tag to "node_energizer" and sprite to energizerSprite

        int[,] bound=new int[(energizerNumber+2),2];
        int segment=1;
        int i,idx;
        bound[0,0]=int.MinValue;bound[0,1]=0;
        bound[1,0]=pelletNumber;bound[1,1]=int.MaxValue;

        for(i=0;i<energizerNumber;i++){
            idx=Random.Range(0,segment);

            idx=Random.Range(bound[idx,1],bound[idx+1,0]);
            Debug.Log("idx="+idx.ToString());
            pellets[idx].tag="node_energizer";
            pellets[idx].GetComponent<SpriteRenderer>().sprite=energizerSprite;
            pellets[idx].transform.localScale=energizerNodeSize;

            int j,low,high;
            for(j=segment;j>=0;j--){
                if(idx<bound[j,0]){
                    bound[j+1,0]=bound[j,0];
                    bound[j+1,1]=bound[j,1];
                }
                else{
                    j++;
                    bound[j,0]=idx;
                    bound[j,1]=idx+1;
                    break;
                }
            }
            segment++;

            j=segment;
            low=0;high=1;
            while(high<=j){
                if(bound[low,1]>=bound[high,0]){
                    bound[low,1]=bound[high,1];
                    segment--;
                }
                else{
                    low++;
                    bound[low,0]=bound[high,0];
                    bound[low,1]=bound[high,1];
                }
                high++;
            }

            Debug.Log("segment="+segment);
            for(j=0;j<=segment;j++){
                Debug.Log(bound[j,0]+" "+bound[j,1]);
            }
        }
        pelletNumber-=energizerNumber;
    }


    private void LevelUp(){
        pelletAte=0;
        scoresPerPellet++;
        scoresPerGhost+=40;

        if(energizerNumber>energizerLeast){
            energizerNumber--;
        }

        GameObject[] pellets=GameObject.FindGameObjectsWithTag("node_pellet");
        for(int i=0;i<pellets.Length;i++){
            pellets[i].GetComponent<SpriteRenderer>().enabled=true;
            pellets[i].GetComponent<BoxCollider2D>().enabled=true;
        }
        SpawnEnergizer(pellets);
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
    }


    private void ChangeLives(int val){
        lives+=val;
    }

    private void ChangeScores(int val){
        scores+=val;
    }

}
