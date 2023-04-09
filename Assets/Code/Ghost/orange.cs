using UnityEngine;

public class orange : ghost{
    private GameObject forward1,forward2,forward3;

    protected override void Start(){
        speedFast=11;
        base.Start();
    }

    public override void LevelUp(){
        speedFast+=0.2f;
        base.LevelUp();
    }

    protected override bool Reach(GameObject node){
        return node==target||node==forward1||node==forward2||node==forward3;
    }

    protected override int Heuristic(Vector2 nodePosition){
        int dist=base.Heuristic(nodePosition),tem;
        if(forward1!=null){
            tem=(int)Mathf.Round(Mathf.Abs(nodePosition.x-forward1.transform.position.x))+
                (int)Mathf.Round(Mathf.Abs(nodePosition.y-forward1.transform.position.y));
            dist=tem>dist?dist:tem;
        }
        else{
            goto Return_;
        }
        if(forward2!=null){
            tem=(int)Mathf.Round(Mathf.Abs(nodePosition.x-forward2.transform.position.x))+
                (int)Mathf.Round(Mathf.Abs(nodePosition.y-forward2.transform.position.y));
            dist=tem>dist?dist:tem;
        }
        else{
            goto Return_;
        }
        if(forward3!=null){
            tem=(int)Mathf.Round(Mathf.Abs(nodePosition.x-forward3.transform.position.x))+
                (int)Mathf.Round(Mathf.Abs(nodePosition.y-forward3.transform.position.y));
            dist=tem>dist?dist:tem;
        }
        Return_:return dist;
    }


    protected override void Update(){
        if(CanUpdate()){
            if(pacmanFound&&!isEdible){
                if(speed<speedFast){
                    speed+=Time.deltaTime*2.25f;
                }
            }
            else{
                speed=speedNormal;
            }
            GetPacmanForwardNode();
            base.Update();
        }
    }

    private void GetPacmanForwardNode(){
        node_control controller=target.GetComponent<node_control>();
        int forwardDirection=pacman.Direction;
        forward1=null;
        forward2=null;
        forward3=null;
        if(forwardDirection<0){
            goto Return_;
        }
        if((forward1=controller.NodeNearby[forwardDirection])!=null&&curNode!=forward1){
            controller=forward1.GetComponent<node_control>();
        }
        else{
            goto Return_;
        }
        if((forward2=controller.NodeNearby[forwardDirection])!=null&&curNode!=forward2){
            controller=forward2.GetComponent<node_control>();
        }
        else{
            goto Return_;
        }
        if((forward3=controller.NodeNearby[forwardDirection])!=null&&curNode!=forward3){
            controller=forward3.GetComponent<node_control>();
        }
        Return_:return;
    }
}
