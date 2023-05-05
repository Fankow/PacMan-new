using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

using Mono.Data.Sqlite;

using TMPro;

internal struct Record{
    internal int Lives,Scores;
    internal Record(int a,int b){
        Lives=a;Scores=b;
    }
}

//after login to the game
public  class main_manager : database_manager,Imain_manager{
    [HideInInspector]public static main_manager instance;//singleton to prevent DDOL instantiate many copies
    public Canvas maps,title,help,login;
    public Player_manager player;
    //public Image block;


    private const int sceneOffset=1,mapNumber=6;
    //change this offset when
    //new scene inserted before MapXX -> +1
    //scene remove before MapXX -> -1
    private StringBuilder displayText=new StringBuilder(40);
    private string playerName;
    private static Record[] records;

    public void Quit(){
        #if UNITY_EDITOR
        EditorApplication.isPlaying=false;
        #else
        Application.Quit();
        #endif
    }

    //button
    public void SelectMap(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(true);
        login.gameObject.SetActive(false);
    }

    //button
    public void ReturnTitle(){
        title.gameObject.SetActive(true);
        maps.gameObject.SetActive(false);
        help.gameObject.SetActive(false);
    }

    //button
    public void UnLoadAllCanvas(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(false);
        help.gameObject.SetActive(false);
        login.gameObject.SetActive(false);
    }

    //button
    public void LogOff(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(false);
        login.gameObject.SetActive(true);
        player.Reset();
    }

    //button
    public void HelpPage(){
        title.gameObject.SetActive(false);
        maps.gameObject.SetActive(false);
        help.gameObject.SetActive(true);
    }

    protected override void Action(){
        int i;
        for(i=0;i<mapNumber;i++){
            records[i].Lives=0;
            records[i].Scores=0;
        }
        command=db_connect.CreateCommand();
        command.CommandText="create table Record(name text not null,id integer not null,scores integer not null,lives integer not null)";
        command.ExecuteNonQuery();
    }    

    //button, load map according to the name of button
    public void LoadMap(){
        string n=EventSystem.current.currentSelectedGameObject.name;
        int sum=n[0]-'0';
        sum=sum*10+(n[1]-'0')-1+sceneOffset;
        UnLoadAllCanvas();
        SceneManager.LoadScene(sum);
    }

    //text shown in the cyan rect of every map
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

    //helper function to change db data
    private void UpdateDB(int id,int scores,int lives){
        OpenDB();
        command=db_connect.CreateCommand();
        command.CommandText=string.Format("update Record set scores={0},lives={1} where id={2} and name='{3}'",scores,lives,id,playerName);
        command.ExecuteNonQuery();
        db_connect.Close();
    }


    public void SaveScoreAndLive(int mapIndex,int scores,int lives){
        if(scores>records[mapIndex-sceneOffset].Scores){
            records[mapIndex-sceneOffset].Scores=scores;
            records[mapIndex-sceneOffset].Lives=lives;
            mapIndex=mapIndex-sceneOffset+1;
            UpdateDB(mapIndex,scores,lives);
            SetDisplayScore((mapIndex<10?GameObject.Find("Map:0"+mapIndex.ToString()):GameObject.Find("Map:"+mapIndex.ToString())),scores,lives);
        }
    }

    //called by the clear highest score button in select map page
    public void ClearData(){
        GameObject tem=EventSystem.current.currentSelectedGameObject;
        string n=tem.name;
        int sum=(n[0]-'0');
        sum=sum*10+(n[1]-'0');
        records[sum-1].Lives=0;
        records[sum-1].Scores=0;
        UpdateDB(sum,0,0);
        tem=tem.transform.parent.gameObject;
        SetDisplayScore(tem,0,0);
    }

    //called by the clear highest score button in playfield
    public void ClearData(int index){
        index-=sceneOffset;
        index++;
        records[index-1].Lives=0;
        records[index-1].Scores=0;
        UpdateDB(index,0,0);
        GameObject tem=GameObject.Find($"Map:0{index}");
        SetDisplayScore(tem,0,0);
    }

    //called by game_manager in playfield to get the highest score of that map
    public int GetHighestScores(int mapIndex){
        return records[mapIndex-sceneOffset].Scores;
    }

    protected override void Awake(){
        if(instance!=null){
            Destroy(gameObject);
        }
        else{
            base.Awake();
            records=new Record[mapNumber];
            instance=this;
            DontDestroyOnLoad(gameObject);
        }
    }

    //load data
    public void LoginSuccess(in string name){
        SelectMap();
        //block.gameObject.SetActive(true);
        help.gameObject.SetActive(false);
        playerName=name;
        GameObject mapX;
        int i,j;
        records=new Record[mapNumber];
        
        if(OpenDB()){
            command=db_connect.CreateCommand();
            command.CommandText="select sql from sqlite_master where name='Record'";
            if(command.ExecuteReader().HasRows==false){
                Action();
            }
            SqliteDataReader read;
            for(i=0;i<10&&i<mapNumber;i=j){
                j=i+1;
                mapX=GameObject.Find("Map:0"+j.ToString());
                if(mapX==null){
                    Debug.Log("Map:0"+j+" is missing");
                    goto Stop_;
                }
                command=db_connect.CreateCommand();
                command.CommandText="select scores,lives from Record where id=@a and name=@b";
                command.Parameters.Add(new SqliteParameter("@a",j));
                command.Parameters.Add(new SqliteParameter("@b",playerName));
                read=command.ExecuteReader();
                //Debug.Log(command.CommandText+" "+read.HasRows);
                if(read.HasRows==false){
                    records[i].Scores=0;
                    records[i].Lives=0;
                    command=db_connect.CreateCommand();
                    command.CommandText=string.Format("insert into Record VALUES (@param1,@param2,0,0)");
                    command.Parameters.AddWithValue("@param1", playerName);
                    command.Parameters.AddWithValue("@param2", j);
                    command.ExecuteNonQuery();
                }
                else{
                    records[i].Scores=int.Parse(read["scores"].ToString());
                    records[i].Lives=int.Parse(read["lives"].ToString());
                }
                SetDisplayScore(mapX,records[i].Scores,records[i].Lives);
                read.Close();
            }

            for(;i<99&&i<mapNumber;i++){
                j=i+1;
                mapX=GameObject.Find("Map:"+j.ToString());
                if(mapX==null){
                    Debug.Log("Map:"+j+" is missing");
                    goto Stop_;
                }
                command=db_connect.CreateCommand();
                command.CommandText="select scores,lives from Record where id=@a and name=@b";
                command.Parameters.Add(new SqliteParameter("@a",j));
                command.Parameters.Add(new SqliteParameter("@b",playerName));
                read=command.ExecuteReader();
                if(read==null){
                    records[i].Scores=0;
                    records[i].Lives=0;
                    command=db_connect.CreateCommand();
                    command.CommandText=string.Format("insert into Record VALUES (@param1,@param2,0,0)");
                    command.Parameters.AddWithValue("@param1", playerName);
                    command.Parameters.AddWithValue("@param2", j);
                    command.ExecuteNonQuery();
                }
                else{
                    records[i].Scores=int.Parse(read["scores"].ToString());
                    records[i].Lives=int.Parse(read["lives"].ToString());
                }
                SetDisplayScore(mapX,records[i].Scores,records[i].Lives);
                read.Close();
            }
        }
        else{
            for(i=0;i<10&&i<mapNumber;i=j){
                j=i+1;
                mapX=GameObject.Find("Map:0"+j.ToString());
                if(mapX==null){
                    Debug.Log("Map:0"+j+" is missing");
                    goto Stop_;
                }
                SetDisplayScore(mapX,0,0);
            }

            for(;i<99&&i<mapNumber;i++){
                j=i+1;
                mapX=GameObject.Find("Map:"+j.ToString());
                if(mapX==null){
                    Debug.Log("Map:"+j+" is missing");
                    goto Stop_;
                }
                SetDisplayScore(mapX,0,0);
            }
        }
        Stop_:

        //block.gameObject.SetActive(false);
        ReturnTitle();
        DontDestroyOnLoad(gameObject);
        db_connect.Close();
    }
}
