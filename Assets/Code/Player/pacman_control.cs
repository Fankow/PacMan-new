using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacman_control : entity{
    public Sprite pelletSprite;

    
    private Animator anime;
    private SpriteRenderer render;
    private Vector2 pelletNodeSize;
    private int speedFastTime,refillTime;
    private bool canSpeedFast=true,isRefill=false,updated=false;
    private int previousDirection;

    protected override void Start(){
        render=transform.GetChild(0).transform.gameObject.GetComponent<SpriteRenderer>();
        anime=transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        base.Start();

        pelletNodeSize=new Vector2(0.5f,0.5f);
        direction=-1;
        previousDirection=-1;
        refillTime=4*manager.frameRate;
        speedFastTime=8*manager.frameRate;
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
            direction=previousDirection;
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
            if(CanChangeNode()){
                //reach cur_node, find next node
                if((nextNode=controller.nodeNearby[direction])!=null){
                    curNode=nextNode;
                    previousDirection=direction;
                }
                else if(previousDirection>=0&&(nextNode=controller.nodeNearby[previousDirection])!=null){
                    curNode=nextNode;
                    //continue run at previousDirection
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(updated==false){
            return;
        }
        else if(other.gameObject.CompareTag("node_pellet")){
            other.gameObject.GetComponent<SpriteRenderer>().enabled=false;
            other.gameObject.GetComponent<BoxCollider2D>().enabled=false;
            //dont need its collider anymore
            if(manager.EatPellet()){
                Restart();
                //eating all pellet and level up, back to its respawn node
            }
        }
        else if(other.gameObject.CompareTag("node_energizer")){
            other.gameObject.GetComponent<SpriteRenderer>().sprite=pelletSprite;
            other.gameObject.GetComponent<SpriteRenderer>().enabled=false;
            other.gameObject.GetComponent<BoxCollider2D>().enabled=false;
            other.gameObject.transform.localScale=pelletNodeSize;
            other.gameObject.tag="node_pellet";
            manager.EatEnergizer();
        }
    }

    protected override void Restart(){
        base.Restart();
        canSpeedFast=true;isRefill=false;
        direction=-1;previousDirection=-1;
    }
}
