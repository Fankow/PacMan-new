using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pellet_delete : MonoBehaviour{
    void Start(){
        StartCoroutine(SelfDestroy());
    }
    IEnumerator SelfDestroy(){
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        yield break;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("node_pellet")){
            Destroy(other.gameObject.GetComponent<SpriteRenderer>());
            other.gameObject.tag="node_normal";
        }
    }
}
