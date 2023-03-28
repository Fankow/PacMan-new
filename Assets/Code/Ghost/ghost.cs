using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

internal struct QueueNode{
    public GameObject node;
    public int step;
    public int id;
    public QueueNode(GameObject node,int step,int id){
        this.node=node;
        this.step=step;
        this.id=id;
    }
}

internal class PriorityQueue{
    List<QueueNode> queue;
    List<int> value;
    public int Count;

    public PriorityQueue(){
        queue=new List<QueueNode>();
        value=new List<int>();
        Count=0;
    }

    public void Clear(){
        queue.Clear();
        value.Clear();
        Count=0;
    }

    public void Enqueue(QueueNode a,int val){
        queue.Add(a);
        value.Add(val);

        int now=Count,par=(now-1)/2;
        QueueNode temT;
        int temInt;
        Count++;
        while(value[now]<value[par]){

            temT=queue[now];
            temInt=value[now];
            queue[now]=queue[par];
            value[now]=value[par];
            queue[par]=temT;
            value[par]=temInt;

            now=par;
            par=(now-1)/2;
        }
    }

    public QueueNode Dequeue(){
        if(Count==0){
            return default(QueueNode);
        }
        else{
            QueueNode retNode=queue[0],temNode;
            Count--;
            
            queue[0]=queue[Count];
            value[0]=value[Count];
            queue.RemoveAt(Count);
            value.RemoveAt(Count);
            int now,i=0,le=1,ri,temInt;
            while(le<Count){
                now=i;
                ri=le+1;
                if(value[le]<value[now]){
                    now=le;
                }
                if(ri<Count&&value[ri]<value[now]){
                    now=ri;
                }

                if(now==i){
                    break;
                }
                else{
                    temNode=queue[now];
                    temInt=value[now];
                    queue[now]=queue[i];
                    value[now]=value[i];
                    queue[i]=temNode;
                    value[i]=temInt;

                    i=now;
                    le=(i*2)+1;
                }
            }

            return retNode;
        }
    }

}

public abstract class ghost : entity{
    protected const int NOT_FOUND=-1,SAME_NODE=-2;

    public delegate bool Comparator(GameObject node);
    
    [SerializeField]protected int searchRange;//used in pathfinding
    [SerializeField]protected int respawnDirection;
    protected bool isEdible,pacmanFound;
    protected int respawnTime;

    [SerializeField]protected Sprite[] eyes;
    [SerializeField]protected SpriteRenderer eyesRenderer;
    protected GameObject target;
    protected pacman_control pacman;


    private Dictionary<GameObject,int> visited=new Dictionary<GameObject,int>();
    private Dictionary<GameObject,GameObject> getPath=new Dictionary<GameObject,GameObject>();
    private Queue<QueueNode> BFSQueue=new Queue<QueueNode>();
    private PriorityQueue AStarQueue=new PriorityQueue();


    protected override void Start(){
        base.Start();
        pacman=GameObject.Find("Player").GetComponent<pacman_control>();
        direction=respawnDirection;
        speed=speedNormal;
        isEdible=false;
    }

    protected virtual void Update(){
        if(CanChangeNode()){
            node_control controller=curNode.GetComponent<node_control>();
            RandomMove();
        }
    }


    int[] directionChoices=new int[4];
    protected int RandomMove(){
        int bannedDirection=3-direction;
        node_control controller=curNode.GetComponent<node_control>();
        int i,j;
        for(i=0,j=0;i<4;i++){
            if(i!=bannedDirection&&controller.NodeNearby[i]!=null){
                directionChoices[j++]=i;
            }
        }
        if(j==0){
            return bannedDirection;
        }
        else{
            return directionChoices[Random.Range(0,j)];
        }
    }

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


    //return the direction the ghost should follow
    protected int BFS(){
        if(Reach(curNode)){
            return SAME_NODE;
        }
        visited.Clear();
        getPath.Clear();
        BFSQueue.Clear();

        int step=0,i,id=0,pid=0;
        node_control controller=curNode.GetComponent<node_control>();
        GameObject border=curNode;
        QueueNode headNode;

        visited.Add(curNode,1);
        getPath.Add(curNode,curNode);

        while(true){
            if(Reach(border)){
                while(getPath[border]!=curNode){
                    border=getPath[border];
                }
                controller=curNode.GetComponent<node_control>();
                for(i=0;i<4;i++){
                    if(controller.NodeNearby[i]==border)break;
                }
                return i;
            }
            else if(++step<searchRange){
                for(i=0;i<4;i++){
                    GameObject nextNode=controller.NodeNearby[i];
                    if(nextNode!=null&&visited.ContainsKey(nextNode)==false){
                        visited.Add(nextNode,1);
                        id++;
                        BFSQueue.Enqueue(new QueueNode(nextNode,step,id));
                        getPath.Add(nextNode,border);
                    }
                }
            }
            if(BFSQueue.Count<1){
                return NOT_FOUND;
            }
            headNode=BFSQueue.Dequeue();
            border=headNode.node;
            pid=headNode.id;
            step=headNode.step;
            controller=border.GetComponent<node_control>();
        }
    }


    protected int AStar(){
        if(Reach(curNode)){
            return SAME_NODE;
        }
        visited.Clear();
        getPath.Clear();
        AStarQueue.Clear();

        int step=0,i,id=0,pid=0;
        int estimate,previousEstimate;
        node_control controller=curNode.GetComponent<node_control>();
        GameObject border=curNode;
        QueueNode headNode;

        visited.Add(curNode,1);
        getPath.Add(curNode,curNode);

        while(true){
            if(Reach(border)){
                while(getPath[border]!=curNode){
                    border=getPath[border];
                }
                controller=curNode.GetComponent<node_control>();
                for(i=0;i<4;i++){
                    if(controller.NodeNearby[i]==border)break;
                }
                return i;
            }
            else if(++step<searchRange){
                visited[border]=-1;
                for(i=0;i<4;i++){
                    GameObject nextNode=controller.NodeNearby[i];
                    if(nextNode!=null){
                        estimate=step+Heuristic(border.transform.position);
                        if(visited.TryGetValue(nextNode,out previousEstimate)){
                            if(previousEstimate<=estimate){continue;}
                            visited[nextNode]=estimate;
                            AStarQueue.Enqueue(new QueueNode(nextNode,step,id),estimate);
                            getPath[nextNode]=border;
                        }
                        else{
                            visited.Add(nextNode,estimate);
                            id++;
                            AStarQueue.Enqueue(new QueueNode(nextNode,step,id),estimate);
                            getPath.Add(nextNode,border);
                        }
                    }
                }
            }
            if(AStarQueue.Count<1){
                return NOT_FOUND;
            }
            headNode=AStarQueue.Dequeue();
            border=headNode.node;
            pid=headNode.id;
            step=headNode.step;
            controller=border.GetComponent<node_control>();
        }
    }

    protected virtual int Heuristic(Vector2 nodePosition){
        int dx=(int)Math.Round(Math.Abs(nodePosition.x-target.transform.position.x),0);
        int dy=(int)Math.Round(Math.Abs(nodePosition.y-target.transform.position.y),0);
        return dx+dy;
    }

    protected virtual bool Reach(GameObject node){
        return node==target;
    }
}
