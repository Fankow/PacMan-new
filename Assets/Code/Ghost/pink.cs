using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pink : ghost{
    private float speedUp;

    protected override void Start(){
        speedUp=speedNormal*1.9f;
        base.Start();
    }

    public override void LevelUp(){
        base.LevelUp();
        speedUp=speedNormal*1.9f;
    }

    protected override void Update(){
        if(CanUpdate()){
            if(isEdible==false){
                int nextDir;
                float pacmanX=target.transform.position.x,pacmanY=target.transform.position.y;
                float selfX=transform.position.x,selfY=transform.position.y;
                if(Mathf.Abs(selfY-pacmanY)<=float.Epsilon){
                    nextDir=selfX<pacmanX? manager.RIGHT:manager.LEFT;
                    CheckObstacles(nextDir);

                }//same y==vertical
                else if(Mathf.Abs(selfX-pacmanX)<=float.Epsilon){
                    nextDir=selfY<pacmanY? manager.UP:manager.DOWN;
                    CheckObstacles(nextDir);

                }//same x==horizontal
                else{
                    speed=speedNormal;
                    base.Update();
                }
            }
            else{
                base.Update();
            }
        }
    }

    private void CheckObstacles(in int nextDir){
        GameObject cur=curNode;
        node_control controller=cur.GetComponent<node_control>();

        do{
            if(cur.name==target.name){
                speed=speedUp;
                curNode=cur;
                direction=nextDir;
                eyesRenderer.sprite=eyes[direction];
                break;
            }
            else{
                cur=controller.NodeNearby[nextDir];
                if(cur==null){
                    speed=speedNormal;
                    base.Update();
                    break;
                }
                controller=cur.GetComponent<node_control>();
            }
        }while(true);

    }

}
