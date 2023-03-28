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
        if(manager.gameActive&&CanChangeNode()){
            target=pacman.CurNode;
            int nextDirection=AStar();
            if(nextDirection>=0){
                direction=nextDirection;
                curNode=curNode.GetComponent<node_control>().NodeNearby[direction];
                eyesRenderer.sprite=eyes[direction];
            }
        }
    }


}
