using UnityEngine;

public class pink : ghost{

    protected override void Start(){
        base.Start();
        speedFast=speedNormal*1.9f;
    }

    public override void LevelUp(){
        base.LevelUp();
        speedFast=speedNormal*1.9f;
    }

    protected override void Update(){
        if(CanUpdate()){
            if(isEdible==false){
                int nextDir;
                float pacmanX=target.transform.position.x,pacmanY=target.transform.position.y;
                float selfX=transform.position.x,selfY=transform.position.y;
                if(Mathf.Abs(selfY-pacmanY)<=float.Epsilon){
                    nextDir=selfX<pacmanX? game_manager.RIGHT:game_manager.LEFT;
                    CheckObstacles(nextDir);

                }//same y==vertical
                else if(Mathf.Abs(selfX-pacmanX)<=float.Epsilon){
                    nextDir=selfY<pacmanY? game_manager.UP:game_manager.DOWN;
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
                speed=speedFast;
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
