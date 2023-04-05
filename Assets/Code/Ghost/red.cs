using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red : ghost{
    protected override void Start(){
        base.Start();
    }

    
    public override void LevelUp(){
        speed+=0.3f;
        speedNormal+=0.3f;
        speedFast+=0.3f;
        searchRange++;
        base.LevelUp();
    }

    protected override void Update(){
        if(CanUpdate()){
            base.Update();
        }
    }
}
