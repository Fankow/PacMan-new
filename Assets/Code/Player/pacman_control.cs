using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class pacman_control:entity,Ipacman_control{
    public Sprite pelletSprite;
    public AnimationClip deadAnimation;
    public Image speedTimeBar;
    
    private Animator anime;
    private SpriteRenderer render;
    private Vector2 pelletNodeSize;
    private WaitForSeconds deadAnimationTime;
    private int speedFastTime,refillTime;
    private bool canSpeedFast=true,isRefill=false,updated=false;
    private int previousDirection;

    private void Awake(){
        ghost.pacman=this;
    }

    protected override void Start(){
        render=transform.GetChild(0).transform.gameObject.GetComponent<SpriteRenderer>();
        anime=transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        base.Start();

        pelletNodeSize=new Vector2(0.5f,0.5f);
        deadAnimationTime=new WaitForSeconds(deadAnimation.length/0.65f);
        direction=-1;
        previousDirection=-1;
        refillTime=2*manager.frameRate;
        speedFastTime=7*manager.frameRate;
        speedTimeBar.fillAmount=1;
    }


    private void SetAnimation(bool x,bool y,bool up,int nextDir){
        render.flipX=x;
        render.flipY=y;
        anime.SetBool("isUp",up);
        direction=nextDir;
    }
    void Update(){
        if(manager.gameActive==false){
            return;
        }
        updated=true;
        if(countDown>0){
            countDown--;
            if(isRefill){
                speedTimeBar.fillAmount+=1f/refillTime;
                if(countDown==0){
                    canSpeedFast=true;
                    isRefill=false;
                }
            }
            else{
                speedTimeBar.fillAmount-=1f/speedFastTime;
                if(countDown==0){
                    isRefill=true;
                    countDown=refillTime;
                    speed=speedNormal;
                }
            }
        }

        if(Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.UpArrow)){
            SetAnimation(false,false,true,game_manager.UP);
        }
        else if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)){
            SetAnimation(true,false,false,game_manager.LEFT);
        }
        else if(Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.DownArrow)){
            SetAnimation(false,true,true,game_manager.DOWN);
        }
        else if(Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow)){
            SetAnimation(false,false,false,game_manager.RIGHT);
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
                if((nextNode=controller.NodeNearby[direction])!=null){
                    curNode=nextNode;
                    previousDirection=direction;
                }
                else if(previousDirection>=0&&(nextNode=controller.NodeNearby[previousDirection])!=null){
                    curNode=nextNode;
                    //continue run at previousDirection
                }
                ghost.target=curNode;
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
            manager.EatPellet();
        }
        else if(other.gameObject.CompareTag("node_energizer")){
            other.gameObject.GetComponent<SpriteRenderer>().enabled=false;
            other.gameObject.GetComponent<BoxCollider2D>().enabled=false;
            manager.EatEnergizer(other.gameObject);
        }
        else if(other.gameObject.CompareTag("ghost")){
            if(other.gameObject.GetComponent<ghost>().BeingEaten()){
                manager.EatGhost();
            }
            else{
                StartCoroutine(PlayDeadAnimation());
            }
        }
    }

    private IEnumerator PlayDeadAnimation(){
        manager.gameActive=false;
        anime.SetTrigger("dead");
        yield return deadAnimationTime;
        if(manager.EatPacman()){
            Restart();
            manager.gameActive=true;
        }
        else{
            gameObject.SetActive(false);
        }
    }

    public override void LevelUp(){
        Restart();
    }
    protected override void Restart(){
        base.Restart();
        countDown=0;
        canSpeedFast=true;isRefill=false;
        direction=-1;previousDirection=-1;
        speedTimeBar.fillAmount=1;
        ghost.target=curNode;
    }
}
