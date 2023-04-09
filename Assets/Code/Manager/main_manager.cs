using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

internal struct Record{
    internal int Lives,Scores;
    internal Record(int a,int b){
        Lives=a;Scores=b;
    }
}

public class main_manager : MonoBehaviour{
    [HideInInspector]public static main_manager instance;
    public Canvas maps,title,help,login;

    private const int sceneOffset=1,mapNumber=1;
    private StringBuilder displayText=new StringBuilder(40);
    private static Record[] records;
    //change this offset when
    //new scene inserted before MapXX -> +1
    //scene remove before MapXX -> -1

    public void Quit(){
        Application.Quit();
    }

    public void SelectMap(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(true);
        login.gameObject.SetActive(false);
    }

    public void ReturnTitle(){
        title.gameObject.SetActive(true);
        maps.gameObject.SetActive(false);
        help.gameObject.SetActive(false);
    }

    public void UnLoadAllCanvas(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(false);
        help.gameObject.SetActive(false);
        login.gameObject.SetActive(false);
    }

    public void LogOff(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(false);
        login.gameObject.SetActive(true);
    }

    public void HelpPage(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(false);
        help.gameObject.SetActive(true);
    }

    public void LoadMap(){
        string n=EventSystem.current.currentSelectedGameObject.name;
        int sum=n[0]-'0';
        sum=sum*10+(n[1]-'0')-1+sceneOffset;
        UnLoadAllCanvas();
        SceneManager.LoadScene(sum);
    }


    private void SetDisplayScore(GameObject obj,int scores,int lives){
        TextMeshProUGUI x=obj.transform.Find("map data").GetComponent<TextMeshProUGUI>();

        displayText.Append(obj.name);
        displayText.Append("\nScores:");
        displayText.Append(scores.ToString());
        displayText.Append("\nLives:");
        displayText.Append(lives.ToString());
        x.text=displayText.ToString();
        displayText.Remove(0,displayText.Length);
    }


    public void SaveScoreAndLive(int mapIndex,int scores,int lives){
        if(scores>records[mapIndex-sceneOffset].Scores){
            records[mapIndex-sceneOffset].Scores=scores;
            records[mapIndex-sceneOffset].Lives=lives;
            mapIndex=mapIndex-sceneOffset+1;
            SetDisplayScore((mapIndex<10?GameObject.Find("Map:0"+mapIndex.ToString()):GameObject.Find("Map:"+mapIndex.ToString())),scores,lives);
        }
    }


    public void ClearData(){
        GameObject tem=EventSystem.current.currentSelectedGameObject;
        string n=tem.name;
        int sum=(n[0]-'0');
        sum=sum*10+(n[1]-'0');
        records[sum-1].Lives=0;
        records[sum-1].Scores=0;

        tem=tem.transform.parent.gameObject;
        SetDisplayScore(tem,0,0);
    }


    public int GetHighestScores(int mapIndex){
        return records[mapIndex-sceneOffset].Scores;
    }

    private void Awake(){
        if(instance!=null){
            Destroy(gameObject);
        }
        else{
            records=new Record[mapNumber];
            instance=this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
