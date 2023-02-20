using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacman_control : MonoBehaviour{
    [HideInInspector]public GameObject curNode;
    public GameObject respawnNode;
    [HideInInspector]public int direction,previousDir;
    public int pelletAte;
    
    private game_manager manager;
    private Animator anime;
    private SpriteRenderer render;
    [SerializeField]private float speedNormal,speedFast;
    private float speed;
    private int speedFastTime,countDown=0,refillTime;
    private bool canSpeedFast=true,isRefill=false,updated=false;

    void Start(){
        render=transform.GetChild(0).transform.gameObject.GetComponent<SpriteRenderer>();
        anime=transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        manager=GameObject.Find("Main Camera").GetComponent<game_manager>();
        curNode=respawnNode;
        direction=-1;
        previousDir=-1;
        pelletAte=0;
        speed=speedNormal;
        refillTime=4*manager.frameRate;
        speedFastTime=8*manager.frameRate;

        transform.position=curNode.transform.position;
    }


    private void SetAnimation(bool x,bool y,bool up,int nextDir){
        render.flipX=x;
        render.flipY=y;
        anime.SetBool("isUp",up);
        direction=nextDir;
    }
    void Update(){
        updated=true;
        if(countDown>0){
            countDown--;
            if(isRefill){
                if(countDown==0){
                    canSpeedFast=true;
                    isRefill=false;
                    Debug.Log("can speed up again");
                }
            }
            else{
                if(countDown==0){
                    isRefill=true;
                    countDown=refillTime;
                    speed=speedNormal;
                    Debug.Log("effect end");
                }
            }
        }

        if(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow)){
            SetAnimation(false,false,true,manager.UP);
        }
        else if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)){
            SetAnimation(true,false,false,manager.LEFT);
        }
        else if(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)){
            SetAnimation(false,true,true,manager.DOWN);
        }
        else if(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)){
            SetAnimation(false,false,false,manager.RIGHT);
        }
        else{
            direction=previousDir;
        }//pacman keep moving in has previous direction

        if(Input.GetKeyDown(KeyCode.Space)&&canSpeedFast){
            speed=speedFast;
            canSpeedFast=false;
            countDown=speedFastTime;
        }

        if(direction==-1){}
        else{
            GameObject nextNode;
            node_control controller=curNode.GetComponent<node_control>();
            transform.position=Vector2.MoveTowards(transform.position,curNode.transform.position,speed*Time.deltaTime);
            if(Math.Abs(transform.position.x-curNode.transform.position.x)<=float.Epsilon&&Math.Abs(transform.position.y-curNode.transform.position.y)<=float.Epsilon){
                //reach cur_node, find next node
                if((nextNode=controller.nodeNearby[direction])!=null){
                    curNode=nextNode;
                    previousDir=direction;
                }
                else if(previousDir>=0&&(nextNode=controller.nodeNearby[previousDir])!=null){
                    curNode=nextNode;
                    //continue run at pre_dir;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(updated==false){
            return;
        }
        if(other.gameObject.CompareTag("node_pellet")){
            other.gameObject.GetComponent<SpriteRenderer>().enabled=false;
            other.gameObject.GetComponent<BoxCollider2D>().enabled=false;
            //dont need its collider anymore
            pelletAte++;
        }
    }
}
