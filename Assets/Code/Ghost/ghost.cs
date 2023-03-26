using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal struct ListNode{
    public int dir,parent;
    public ListNode(int dir,int parent){
        this.dir=dir;
        this.parent=parent;
    }
}

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

public abstract class ghost : entity{

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
    private List<ListNode> unionfind=new List<ListNode>();
    private Queue<QueueNode> BFSQueue=new Queue<QueueNode>();


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
            RandomMove();//a function of random movement for ghosts
        }
    }

    protected void RandomMove(){
        
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


    protected bool Reach(GameObject node){
        return node==target;
    }

    //return the direction the ghost should follow
    protected int BFS(Comparator comparator){
        if(comparator(curNode)){
            return -1;
        }
        visited.Clear();
        unionfind.Clear();
        BFSQueue.Clear();

        int step=0,i,id=0,pid=0;
        node_control controller=curNode.GetComponent<node_control>();
        GameObject border=curNode;
        QueueNode headNode;

        visited.Add(curNode,1);
        unionfind.Add(new ListNode(0,-1));

        while(true){
            if(comparator(border)){
                while(unionfind[pid].parent!=0){
                    pid=unionfind[pid].parent;
                }
                return unionfind[pid].dir;
            }
            else if(++step<searchRange){
                for(i=0;i<4;i++){
                    GameObject nextNode=controller.NodeNearby[i];
                    if(nextNode!=null&&visited.ContainsKey(nextNode)==false){
                        visited.Add(nextNode,1);
                        id++;
                        BFSQueue.Enqueue(new QueueNode(nextNode,step,id));
                        unionfind.Add(new ListNode(i,pid));
                    }
                }
            }
            if(BFSQueue.Count<1){
                return -1;
            }
            headNode=BFSQueue.Dequeue();
            border=headNode.node;
            pid=headNode.id;
            step=headNode.step;
            controller=border.GetComponent<node_control>();
        }
    }

}
