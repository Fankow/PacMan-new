using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//destory all nodes spawned inside its' collider2D
public class node_delete : MonoBehaviour{
    void Start(){
        if(gameObject.CompareTag("map")){
            Destroy(gameObject.GetComponent<TilemapCollider2D>(),2f);
            Destroy(gameObject.GetComponent<Rigidbody2D>(),2f);
            Destroy(gameObject.GetComponent<node_delete>(),2f);
        }
        else if(gameObject.CompareTag("node_deleter")){
            Destroy(gameObject,2f);
        }
    }


    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("node_pellet")||other.gameObject.CompareTag("node_normal")){
            Destroy(other.gameObject);
        }
    }
}
