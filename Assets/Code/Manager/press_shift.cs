using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class press_shift:MonoBehaviour{
    public TextMeshProUGUI pressToStartText;
    public game_manager manager;

    private void Update(){
        if(Input.GetKeyDown(KeyCode.LeftShift)||Input.GetKeyDown(KeyCode.RightShift)||manager.gameActive){
            manager.gameActive=true;
            pressToStartText.gameObject.SetActive(false);
            this.enabled=false;
        }
    }

    private void OnEnable(){
        manager.gameActive=false;
        pressToStartText.gameObject.SetActive(true);
    }
}
