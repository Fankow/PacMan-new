using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System;

using UnityEngine;

using Mono.Data.Sqlite;

public abstract class database_manager : MonoBehaviour{
    protected string dblocation;
    protected SqliteConnection db_connect;
    protected SqliteCommand command;

    protected virtual void Awake(){
        command=new SqliteCommand();
        dblocation="URI=file:"+Application.dataPath+@"/DB/data.db";
        db_connect=new SqliteConnection();
    }

    protected bool OpenDB(){
        if(Directory.Exists(Application.dataPath+@"/DB")==false){
            Directory.CreateDirectory(Application.dataPath+@"/DB");
        }
        if(File.Exists(Application.dataPath+@"/DB/data.db")==false){
            db_connect.ConnectionString=string.Format("{0};Mode=ReadWriteCreate;",dblocation);
            db_connect.Open();
            Action();
            return false;
        }
        else{
            db_connect.ConnectionString=string.Format("{0};Mode=ReadWrite;",dblocation);;
            db_connect.Open();
            return true;
        }
    }

    protected abstract void Action();
}
