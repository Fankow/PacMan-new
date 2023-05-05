using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for ghost and pacman
public abstract class entity : MonoBehaviour{
    protected GameObject curNode;
    public GameObject CurNode{get{return curNode;}}
    [SerializeField]protected GameObject respawnNode;
    protected game_manager manager;
    protected float speed;
    [SerializeField]protected float speedNormal,speedFast;
    protected int direction;
    public int Direction{get{return direction;}}
    protected int countDown;

    protected virtual void Start(){
        manager=GameObject.Find("Main Camera").GetComponent<game_manager>();
        Restart();
    }

    protected bool CanChangeNode(){
        transform.position=Vector2.MoveTowards(transform.position,curNode.transform.position,speed*Time.deltaTime);
        return Math.Abs(transform.position.x-curNode.transform.position.x)<=float.Epsilon&&Math.Abs(transform.position.y-curNode.transform.position.y)<=float.Epsilon;
    }

    protected virtual void Restart(){
        curNode=respawnNode;
        transform.position=curNode.transform.position;
        speed=speedNormal;
    }

    public abstract void LevelUp();
}
