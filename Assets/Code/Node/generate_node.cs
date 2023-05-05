using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//once the scene is loaded, fill the playfield with node
public class generate_node : MonoBehaviour{
    public GameObject node;

    void Awake(){
        game_manager manager=GameObject.Find("Main Camera").GetComponent<game_manager>();
        Transform nodes=new GameObject("Nodes").transform;//create a "floder" to have a clean hierarchy
        Vector2 startPos=transform.position;
        int row=manager.row;
        int column=manager.column;
        float offset=1f;
        // spawn the node into the map and creating each instances
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
