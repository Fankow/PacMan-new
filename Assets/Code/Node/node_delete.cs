using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//destory all nodes spawned in collider
public class node_delete : MonoBehaviour{
    
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("node_pellet")){
            Destroy(other.gameObject);
        }
    }
}
