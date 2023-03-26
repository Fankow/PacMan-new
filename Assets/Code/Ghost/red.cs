using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red : ghost{
    
    
    protected override void Start(){
        base.Start();
    }

    protected override void Update(){
        if(CanChangeNode()){
            target=pacman.CurNode;
            int nextDirection=BFS(Reach);
            if(nextDirection>=0){
                direction=nextDirection;
                curNode=curNode.GetComponent<node_control>().NodeNearby[direction];
                eyesRenderer.sprite=eyes[direction];
            }
        }
    }
}
