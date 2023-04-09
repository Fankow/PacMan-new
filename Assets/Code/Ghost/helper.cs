using UnityEngine;

public class helper:MonoBehaviour{
    [Header("ghost sprites")]
    public Sprite bodyAfraid;
    public Sprite[] eyes;
    private void Awake(){
        ghost.bodyAfraid=bodyAfraid;
        ghost.eyes=eyes;
    }
}