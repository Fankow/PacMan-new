using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//destory all nodes spawned inside collider2D
public class node_delete : MonoBehaviour{
    void Start(){
        StartCoroutine(SelfDestroy());
    }
    IEnumerator SelfDestroy(){
        yield return new WaitForSeconds(2);
        if(gameObject.CompareTag("map")){
            Destroy(gameObject.GetComponent<TilemapCollider2D>());
            Destroy(gameObject.GetComponent<Rigidbody2D>());
            Destroy(gameObject.GetComponent<node_delete>());
        }
        else if(gameObject.CompareTag("node_deleter")){
            Destroy(gameObject);
        }
        yield break;
    }


    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("node_pellet")||other.gameObject.CompareTag("node_normal")){
            Destroy(other.gameObject);
        }
    }
}
