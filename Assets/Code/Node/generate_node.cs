using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generate_node : MonoBehaviour{
    public GameObject node;

    private int row,column;
    private int id;
    private float offset=1f;
    private Vector2 startPos;
    private Transform nodes;

    private void CreateNode(int i,int j){
        GameObject newNode=Instantiate(node,startPos+new Vector2(i*offset,j*offset),node.transform.rotation);
        newNode.transform.SetParent(nodes);
        newNode.name="node"+id.ToString();
        id++;
    }

    void Awake(){
        game_manager manager=GameObject.Find("Main Camera").GetComponent<game_manager>();
        row=manager.row;
        column=manager.column;
        startPos=transform.position;
        id=0;
        nodes=new GameObject("nodes").transform;

        int i,j;
        for(i=0;i<column;i++){
            for(j=0;j<row;j++){
                CreateNode(i,j);
            }
        }
        Destroy(gameObject);
    }

}
