using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node_control : MonoBehaviour{
    private GameObject[] nodeNearby;
    public GameObject[] NodeNearby{get {return nodeNearby;}}

    delegate bool Comparator(GameObject node);

    void Start(){
        nodeNearby=new GameObject[4];//four direction, up,down,left,right
        StartCoroutine(WaitForGenerator());
    }
    IEnumerator WaitForGenerator(){
        while(GameObject.Find("Node Generator")){
            yield return null;
        }//wait for finish generation of all nodes
        yield return new WaitForSeconds(0.2f);
        //give node deleter some time, wait for it finishes deleting 

        if(gameObject.CompareTag("node_ghost")){
            nodeNearby[game_manager.UP]=Search(Vector2.up,CmpGhost);
            nodeNearby[game_manager.LEFT]=Search(-Vector2.right,CmpGhost);
            nodeNearby[game_manager.RIGHT]=Search(Vector2.right,CmpGhost);
            nodeNearby[game_manager.DOWN]=Search(-Vector2.up,CmpGhost);
        }
        else{
            nodeNearby[game_manager.UP]=Search(Vector2.up,Cmp);
            nodeNearby[game_manager.LEFT]=Search(-Vector2.right,Cmp);
            nodeNearby[game_manager.RIGHT]=Search(Vector2.right,Cmp);
            nodeNearby[game_manager.DOWN]=Search(-Vector2.up,Cmp);
        }
        //since node_ghost can connect to other nodes but not vice verse,
        //the search function are different;
        yield break;
    }
    
    bool Cmp(GameObject x){
        return x.gameObject.CompareTag("node_ghost")==false&&x.name!=gameObject.name;
    }
    bool CmpGhost(GameObject x){
        return x.name!=gameObject.name;
    }

    private GameObject Search(Vector2 direction,Comparator cmp){
        RaycastHit2D[] all=Physics2D.RaycastAll(transform.position,direction,1.1f,0x1<<3);
        InsertionSort(all,all.Length);
        GameObject ret=null,x;

        int i;
        for(i=0;i<all.Length;i++){
            x=all[i].collider.gameObject;
            if(cmp(x)){
                ret=x;
            }
        }
        return ret;
    }

    private void InsertionSort(RaycastHit2D[] all,int len){
        RaycastHit2D tem;
        int i,j;
        for(i=0;i<len;i++){
            for(j=i+1;j<len;j++){
                if(all[j].distance<all[j-1].distance){
                    tem=all[j];
                    all[j]=all[j-1];
                    all[j-1]=tem;
                }
                else{
                    break;
                }
            }
        }
        
    }

}
