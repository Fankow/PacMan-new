//#define TEST_MAP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class game_manager:MonoBehaviour,Igame_manager{
    public const int UP=0,DOWN=3,LEFT=1,RIGHT=2;//sum of opposite direction==3

    [Header("sprites and images")]
    public Sprite pelletSprite;
    public Sprite energizerSprite;
    public Image energizerEffectBar,block;
    [Header("text")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI liveText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI highestScoresText;
    public TextMeshProUGUI gameEndText;
    [Header("play related")]
    public press_shift pressShift;
    public GameObject menu;
    public pacman_control pacman;
    [Header("buttons")]
    public GameObject pauseButton;
    [Header("Misc")]
    public int row;
    public int column;
    [HideInInspector]public int frameRate;
    [HideInInspector]public bool gameActive;

    
    private ghost[] ghosts;
    private GameObject[] pellets;
    private Vector2 energizerNodeSize,pelletNodeSize;

    private int pelletAte,pelletNumber;
    [SerializeField]private int energizerSpawnNumber,energizerLeast;
    private int energizerEffectTime,countDown;
    private int lives=3,scores=0;
    private int scoresPerPellet=10,scoresPerGhost=100;
    private int level=1,maxLevel=10;
    private int sceneIndex;
    
    private Dictionary<int,int> banned;
    private List<int> energizersRemain;
    

    void Awake(){
        gameActive=false;
        frameRate=25;//reduce the workload of my gpu....
        Application.targetFrameRate=frameRate;
        sceneIndex=SceneManager.GetActiveScene().buildIndex;

        GameObject[] ghostsInScene=GameObject.FindGameObjectsWithTag("ghost");
        ghosts=new ghost[ghostsInScene.Length];
        int i;
        for(i=0;i<ghostsInScene.Length;i++){
            ghosts[i]=ghostsInScene[i].GetComponent<ghost>();
        }
    }

    void Start(){
        energizersRemain=new List<int>();
        banned=new Dictionary<int,int>();
        energizerNodeSize=new Vector2(0.8f,0.8f);
        pelletNodeSize=new Vector2(0.5f,0.5f);
        
        #if TEST_MAP
        #else
        highestScoresText.text=string.Format("Highest\nScores:{0}",main_manager.instance.GetHighestScores(sceneIndex).ToString());
        #endif
        levelText.text=string.Format("Levels:{0}\nScores:",level);
        scoreText.text=scores.ToString();
        liveText.text=string.Format("x {0}",lives);

        energizerEffectBar.fillAmount=0;
        energizerEffectTime=7*frameRate;

        StartCoroutine(WaitForGenerator());
    }
    IEnumerator WaitForGenerator(){
        while(GameObject.Find("Node Generator")){
            yield return null;
        }//wait for finish generation of all nodes
        yield return new WaitForSeconds(0.2f);

        pellets=GameObject.FindGameObjectsWithTag("node_pellet");
        pelletNumber=pellets.Length;
        SpawnEnergizer(pelletNumber);
        
        yield break;
    }
    
    private void SpawnEnergizer(int choices){
        //randomly pick energizerSpawnNumber GameObjects from pellets and
        //turn their tag to "node_energizer" and sprite to energizerSprite
        int i,idx,x;
        int choice;
        //Debug.Log($"choices={choices}");
        for(i=0;i<energizerSpawnNumber;i++){
            idx=Random.Range(0,choices);
            //Debug.Log($"get {idx}");
            choices--;

            if(banned.TryGetValue(idx,out x)){
                if(idx==choices){
                    banned.Remove(idx);
                    //Debug.Log($"mapped, get {x}");
                }
                else{
                    int y;
                    choice=choices;
                    while(banned.TryGetValue(choice,out y)){choice=y;}
                    //see if choices was mapped
                    banned[idx]=choice;
                    //Debug.Log($"mapped, get {x} mapped {choice}");
                }
                idx=x;
                //pellets[idx] already becomes node_energizer
            }
            else{
                if(idx==choices){}
                else{
                    choice=choices;
                    while(banned.TryGetValue(choice,out x)){choice=x;}
                    //see if choices was mapped
                    banned.Add(idx,choice);
                    //Debug.Log($"map {idx} with {choice}");
                }
            }

            pellets[idx].tag="node_energizer";
            pellets[idx].GetComponent<SpriteRenderer>().sprite=energizerSprite;
            pellets[idx].transform.localScale=energizerNodeSize;
        }

        pelletNumber-=energizerSpawnNumber;
        banned.Clear();
        energizersRemain.Clear();
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)&&lives>0&&level<maxLevel){
            PauseGameButton();
        }
        if(countDown>0){
            countDown--;
            energizerEffectBar.fillAmount-=1f/energizerEffectTime;
            if(countDown==0){
                for(int i=0;i<ghosts.Length;i++){
                    ghosts[i].UnsetEdible();
                }
            }
        }
    }

    //action when level up
    IEnumerator LevelUp(){
        yield return new WaitForSeconds(0.2f);
        pelletAte=0;
        scoresPerPellet++;
        scoresPerGhost+=40;
        level++;
        levelText.text=string.Format("Levels:{0}\nScores:",level);

        if(energizerSpawnNumber>energizerLeast){
            energizerSpawnNumber--;
        }

        int choices=pellets.Length;
        int i;

        for(i=0;i<pellets.Length;i++){
            pellets[i].GetComponent<SpriteRenderer>().enabled=true;
            pellets[i].GetComponent<BoxCollider2D>().enabled=true;
            if(pellets[i].CompareTag("node_energizer")){
                banned.Add(i,-1);
                energizersRemain.Add(i);
            }
        }
        pelletNumber=pellets.Length-energizersRemain.Count;

        for(i=0;i<energizersRemain.Count;i++){
            if(energizersRemain[i]<pelletNumber){
                do{
                    choices--;
                }while(banned.ContainsKey(choices));
                banned[energizersRemain[i]]=choices;
                //remap the last energizersRemain.Count indices to previous banned indices
            }
        }

        SpawnEnergizer(pelletNumber);
        //pelletNumber on map=total pellets-energizer left in previous level-number of energizer that will spawn
        pressShift.enabled=true;
        pacman.LevelUp();
        for(i=0;i<ghosts.Length;i++){
            ghosts[i].LevelUp();
        }
        energizerEffectBar.fillAmount=0;
        yield break;
    }

    public void EatPellet(){
        pelletAte++;
        ChangeScores(scoresPerPellet);
        if(pelletAte==pelletNumber){
            gameActive=false;
            if(level==maxLevel){
                gameEndText.gameObject.SetActive(true);
                gameEndText.color=Color.green;
                gameEndText.text=string.Format("Win\nyou reach level:{0}",level);
                menu.SetActive(true);
                pauseButton.SetActive(false);
            }
            else{
                StartCoroutine(LevelUp());
            }
        }
    }
    public bool EatPacman(){
        ChangeLives(-1);
        ChangeScores(-150);
        if(lives<=0){
            gameEndText.gameObject.SetActive(true);
            gameEndText.color=Color.red;
            gameEndText.text=string.Format("Game Over\nyou reach level:{0}",level-1);
            menu.SetActive(true);
            pauseButton.SetActive(false);
            return false;
        }
        else{
            return true;
        }
    }
    public void EatGhost(){
        ChangeLives(1);
        ChangeScores(scoresPerGhost);
    }
    public void EatEnergizer(GameObject node){
        node.transform.localScale=pelletNodeSize;
        node.GetComponent<SpriteRenderer>().sprite=pelletSprite;
        node.tag="node_pellet";
        energizerEffectBar.fillAmount=1;
        countDown=energizerEffectTime;
        for(int i=0;i<ghosts.Length;i++){
            ghosts[i].SetEdible();
        }
    }

    //two helper function to change the text
    private void ChangeLives(int val){
        lives+=val;
        liveText.text=string.Format("x {0}",lives);
    }

    private void ChangeScores(int val){
        scores+=val;
        scoreText.text=scores.ToString();
    }


    public void PauseGameButton(){
        menu.SetActive(gameActive);
        gameActive=!gameActive;
    }


    public void ClearHighestScoreButton(){
        block.gameObject.SetActive(true);
        highestScoresText.text="Highest\nScores:0";
        #if TEST_MAP
        #else
        main_manager.instance.SelectMap();
        main_manager.instance.ClearData(sceneIndex);
        main_manager.instance.UnLoadAllCanvas();
        #endif
        block.gameObject.SetActive(false);
    }


    public void RestartGameButton(){
        block.gameObject.SetActive(true);
        #if TEST_MAP
        #else
        main_manager.instance.SelectMap();
        main_manager.instance.SaveScoreAndLive(sceneIndex,scores,lives);
        main_manager.instance.UnLoadAllCanvas();
        #endif
        SceneManager.LoadScene(sceneIndex);
    }

    public void ExitButton(){
        block.gameObject.SetActive(true);
        #if TEST_MAP
        #else
        main_manager.instance.SelectMap();
        main_manager.instance.SaveScoreAndLive(sceneIndex,scores,lives);
        #endif
        SceneManager.LoadScene(0);
    }

}
