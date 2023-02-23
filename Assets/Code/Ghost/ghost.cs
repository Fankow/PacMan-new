using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ghost : entity{
    
    [SerializeField]protected int searchRange;//used in pathfinding
    [SerializeField]protected int respawnDirection;
    protected bool isEdible;
    protected int respawnTime;

    protected override void Start(){
        base.Start();
        direction=respawnDirection;
        speed=speedNormal;
        isEdible=false;
    }

    protected virtual void Update(){
        if(CanChangeNode()){
            node_control controller=curNode.GetComponent<node_control>();
            //RandomMove(); a function of random movement for ghosts
        }
    }
    //public virtual void RandomMove(){
    //    
    //}

    //some ghost may have different change when levelup, so virtual function
    public virtual void LevelUp(){
        speed+=0.2f;
        speedNormal+=0.2f;
        speedFast+=0.2f;
        searchRange++;
        Restart();
    }

    //all ghosts are the same: translate to respawnNode and wait for count down
    public bool BeingEaten(){
        if(isEdible){
            Restart();
            return true;
        }
        else{
            return false;
        }
    }

    protected override void Restart(){
        base.Restart();
        direction=respawnDirection;
    }

}
