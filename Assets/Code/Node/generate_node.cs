using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generate_node : MonoBehaviour{
    public GameObject node;

    void Awake(){
        game_manager manager=GameObject.Find("Main Camera").GetComponent<game_manager>();
        Transform nodes=new GameObject("Nodes").transform;
        Vector2 startPos=transform.position;
        int row=manager.row;
        int column=manager.column;
        float offset=1f;

        int i,j,id;
        for(i=0,id=0;i<column;i++){
            for(j=0;j<row;j++,id++){
                GameObject newNode=Instantiate(node,startPos+new Vector2(i*offset,j*offset),node.transform.rotation);
                newNode.transform.SetParent(nodes);
                newNode.name="node"+id.ToString();
            }
        }
        Destroy(gameObject);
    }

}
