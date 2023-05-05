using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random=UnityEngine.Random;


public class pacman_control:entity,Ipacman_control{
    public Sprite pelletSprite;
    public AnimationClip deadAnimation;
    public Image speedTimeBar;
    public AudioClip munch1;
    public AudioClip munch2;
    public AudioClip dead;
    public AudioClip teleport;
    private AudioSource audio_;
    private Animator anime;
    private SpriteRenderer render;
    private Vector2 pelletNodeSize;
    private WaitForSeconds deadAnimationTime;
    private GameObject[] portals;
    private int speedFastTime,refillTime;
    private bool canSpeedFast=true,isRefill=false,updated=false;
    private int previousDirection;


    private void Awake(){
        portals=GameObject.FindGameObjectsWithTag("node_tp");
        ghost.pacman=this;
    }

    protected override void Start(){
        render=transform.GetChild(0).transform.gameObject.GetComponent<SpriteRenderer>();
        anime=transform.GetChild(0).transform.gameObject.GetComponent<Animator>();
        base.Start();
        audio_ = transform.GetComponent<AudioSource>();
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
        //check if CD > 0
        //if true, consume the speed time bar then refill 
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
        // check the player input and set the pacman animation correspondingly
        if(Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow)){
            SetAnimation(false,false,true,game_manager.UP);
        }
        else if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow)){
            SetAnimation(true,false,false,game_manager.LEFT);
        }
        else if(Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow)){
            SetAnimation(false,true,true,game_manager.DOWN);
        }
        else if(Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow)){
            SetAnimation(false,false,false,game_manager.RIGHT);
        }

        //speed up
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
            }
            if(previousDirection>=0&&curNode.CompareTag("node_tp")){
                RandomTeleport();
                //after teleportation if no any valid move, previousDirection will always be -1 and no teleport again
            }
            ghost.target=curNode;
        }
    }


    private void OnTriggerEnter2D(Collider2D other){
        if(updated==false){
            return;
        }
        // check if the object is node
        else if(other.gameObject.CompareTag("node_pellet")){
            other.gameObject.GetComponent<SpriteRenderer>().enabled=false;
            other.gameObject.GetComponent<BoxCollider2D>().enabled=false;
            //dont need its collider anymore
            manager.EatPellet();
            audio_.PlayOneShot(munch1);
        }
        // check if the object is energizer
        else if(other.gameObject.CompareTag("node_energizer")){
            other.gameObject.GetComponent<SpriteRenderer>().enabled=false;
            other.gameObject.GetComponent<BoxCollider2D>().enabled=false;
            manager.EatEnergizer(other.gameObject);
            audio_.PlayOneShot(munch2);
        }
        // check if the object is ghost
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
        //Set pacman to dead animation and play the dead audio
        manager.gameActive=false;
        anime.SetTrigger("dead");
        yield return deadAnimationTime;
        if(manager.EatPacman()){
            Restart();
            audio_.PlayOneShot(dead);
            manager.gameActive=true;
        }
        else{
            gameObject.SetActive(false);
        }
    }
   

    public override void LevelUp(){
        Restart();
    }
    //reset all variables
    protected override void Restart(){
        base.Restart();
        countDown=0;
        canSpeedFast=true;isRefill=false;
        direction=-1;previousDirection=-1;
        speedTimeBar.fillAmount=1;
        ghost.target=curNode;
    }

    private void RandomTeleport(){
        if(portals.Length<2){return;}
        int i;
        for(i=0;i<portals.Length;i++){
            if(curNode==portals[i]){break;}
        }
        int next=Random.Range(0,portals.Length-1);
        if(next>=i){
            next++;
        }
        curNode=portals[next];
        transform.position=curNode.transform.position;
        previousDirection=-1;
        direction=-1;
    }
}
