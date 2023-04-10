using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Mono.Data.Sqlite;

using TMPro;

public class Player_manager : database_manager,IPlayer_manager{
    [HideInInspector]public string player_name;
    public GameObject create_player,player_login,delete_player,change_password;
    public TextMeshProUGUI Text;

    public main_manager data;
    

    public void CreatePlayerButton(){
        create_player.SetActive(true);
        player_login.SetActive(false);
    }

    public void ReturnLoginButton(){
        create_player.SetActive(false);
        player_login.SetActive(true);
        delete_player.SetActive(false);
        change_password.SetActive(false);
    }

    public void DeletePlayerButton(){
        player_login.SetActive(false);
        delete_player.SetActive(true);
    }

    public void ChangePasswordButton(){
        player_login.SetActive(false);
        change_password.SetActive(true);
    }

    
    protected override void Action(){
        command=db_connect.CreateCommand();
        command.CommandText="create table Player(name text primary key not null,pw text not null)";
        command.ExecuteNonQuery();
    }
    
    public void Reset(){
        Text.gameObject.SetActive(false);
        create_player.SetActive(false);
        player_login.SetActive(true);
        delete_player.SetActive(false);
        change_password.SetActive(false);
        player_name="";
        OpenDB();
    }
    protected override void Awake(){
        base.Awake();
        Reset();
        command=db_connect.CreateCommand();
        command.CommandText="select sql from sqlite_master where name='Player'";
        if(command.ExecuteReader().HasRows==false){
            Action();
        }
    }

    private void ShowText(string a){
        Text.gameObject.SetActive(true);
        Text.text=a;
        StartCoroutine(DisableText());
    }
    IEnumerator DisableText(){
        yield return new WaitForSeconds(1.5f);
        Text.gameObject.SetActive(false);
        yield break;
    }

    private bool CheckPlayerExists(TMP_InputField input_name,TMP_InputField password){
        if(input_name.text==""){
            ShowText("Please Enter Your Name");
            return false;
        }
        else{
            SqliteDataReader read;
            command=db_connect.CreateCommand();
            command.CommandText=string.Format("select Count(*) from Player where name='{0}'",input_name.text);
            read=command.ExecuteReader();
            if(read[0].ToString()=="0"){
                ShowText("No Such Player");
                password.text="";
                input_name.text="";
                read.Close();
                return false;
            }
            else{
                read.Close();
                command=db_connect.CreateCommand();
                command.CommandText=string.Format("select pw from Player where name='{0}'",input_name.text);
                read=command.ExecuteReader();
                if(read["pw"].ToString()!=password.text){
                    ShowText("Incorrect Password");
                    read.Close();
                    return false;
                }
                else{
                    read.Close();
                    return true;
                }
            }
        }
    }
    

    public void PlayerLoginButton(){
        TMP_InputField input_name=transform.Find("Login").transform.Find("name").GetComponent<TMP_InputField>();
        TMP_InputField password=transform.Find("Login").transform.Find("password").GetComponent<TMP_InputField>();

        if(CheckPlayerExists(input_name,password)){
            player_name=input_name.text;
            password.text="";
            input_name.text="";
            db_connect.Close();
            data.LoginSuccess(player_name);
        }
    }


    public void CreatePlayer(){
        TMP_InputField input_name=transform.Find("Login").transform.Find("name").GetComponent<TMP_InputField>();
        if(input_name.text==""){
            ShowText("Please Enter Your Name");
        }
        else{
            SqliteDataReader read;
            TMP_InputField password=transform.Find("Login").Find("password").GetComponent<TMP_InputField>();
            TMP_InputField retype=transform.Find("Login").transform.Find("Create").transform.Find("check").GetComponent<TMP_InputField>();

            command=db_connect.CreateCommand();
            command.CommandText=string.Format("select Count(*) from Player where name='{0}'",input_name.text);
            read=command.ExecuteReader();
            if(read[0].ToString()=="1"){
                ShowText("Player Already Exists");
                input_name.text="";
                retype.text="";
                password.text="";
                read.Close();
            }
            else if(password.text==""){
                ShowText("Please Enter Password");
                read.Close();
            }
            else if(password.text!=retype.text){
                ShowText("Retype Password Not Match");
                read.Close();
            }
            else{
                read.Close();
                command=db_connect.CreateCommand();
                command.CommandText=string.Format("insert into Player values({0},{1})",input_name.text,password.text);
                command.ExecuteNonQuery();
                ShowText("Create New Player Success\nPlease Return To Login Page And Login");
                input_name.text="";
                retype.text="";
                password.text="";
            }
        }
    }

    #pragma warning disable CS0168
    public void DeletePlayer(){
        TMP_InputField input_name=transform.Find("Login").transform.Find("name").GetComponent<TMP_InputField>();
        TMP_InputField password=transform.Find("Login").transform.Find("password").GetComponent<TMP_InputField>();
        if(CheckPlayerExists(input_name,password)){
            command=db_connect.CreateCommand();
            try{
                command.CommandText=string.Format("delete from Player where name='{0}'",input_name.text);
                command.ExecuteNonQuery();
                command=db_connect.CreateCommand();
                command.CommandText=string.Format("delete from Record where name='{0}'",input_name.text);
                command.ExecuteNonQuery();
            }catch(Exception e){}
            password.text="";
            input_name.text="";
            ShowText("Delete Successfully");
        }
    }
    #pragma warning restore CS0168
    

    public void ChangePassword(){
        TMP_InputField input_name=transform.Find("Login").transform.Find("name").GetComponent<TMP_InputField>();
        TMP_InputField password=transform.Find("Login").transform.Find("password").GetComponent<TMP_InputField>();

        if(CheckPlayerExists(input_name,password)){
            TMP_InputField new_pw=transform.Find("Login").transform.Find("Change").
            transform.Find("password").GetComponent<TMP_InputField>();
            TMP_InputField retype=transform.Find("Login").transform.Find("Change").
            transform.Find("check").GetComponent<TMP_InputField>();

            if(new_pw.text==""){
                ShowText("Please Enter New Password");
            }
            else if(new_pw.text!=retype.text){
                ShowText("Retype Password Not Match");
            }
            else{
                command=db_connect.CreateCommand();
                command.CommandText=string.Format("update Player set pw='{0}' where name='{1}'",new_pw.text,input_name.text);
                command.ExecuteNonQuery();
                ShowText("Update Password Success");
                input_name.text="";
                password.text="";
                new_pw.text="";
                retype.text="";
            }
        }
    }

    
    
}