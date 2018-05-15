using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UxMainMenu : UxPanel {
    [Header("UI Reference")]
    public Button deathMatchButton;
    public Button titleMatchButton;
    public Button optionsButton;
    public Button scoresButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Prefabs")]
    public GameObject scoresPrefab;

    [Header("State Variables")]
    public GameInfo gameInfo;

    [Header("Events")]
    public GameEvent modeSelected;            // used to notify main menu selection
    public GameEvent onScoreBack;             // used to return to main menu once score screen has been cleared

    public void Start() {
        // start by displaying and setting up panel
        Display();
        OnModeSelection();
    }

    public void OnModeSelection() {
        // setup button callbacks
        deathMatchButton.onClick.AddListener(()=>{ OnGameModeClick(GameMode.DeathMatch); });
        titleMatchButton.onClick.AddListener(()=>{ OnGameModeClick(GameMode.Championship); });
        optionsButton.onClick.AddListener(OnOptionsClick);
        scoresButton.onClick.AddListener(OnScoresClick);
        creditsButton.onClick.AddListener(OnCreditsClick);
        quitButton.onClick.AddListener(OnQuitClick);
    }

    public void OnGameModeClick(GameMode gameMode) {
        Debug.Log("OnGameModeClick");
        // game mode is stored in ScriptableObject for GameInfo, shared across scripts and scenes
        gameInfo.gameMode = gameMode;
        // raise notification that game mode has been selected
        modeSelected.Raise();
        // hide
        Hide();
    }

    public void OnQuitClick() {
        Application.Quit();
    }

    public void OnScoresClick() {
        StartCoroutine(StateWaitScores());
    }

    IEnumerator StateWaitScores() {
        // instantiate scores prefab (under canvas)
        var panelGo = Instantiate(scoresPrefab, GetCanvas().gameObject.transform);
        yield return null;      // wait a frame for panel initialization

        // create listener for back event
        var scoreDone = false;
        var listener = panelGo.AddComponent<GameEventListener>();
        listener.SetEvent(onScoreBack);
        listener.Response.AddListener(()=>{scoreDone = true;});

        // wait for gameModeSelected event
        yield return new WaitUntil(() => scoreDone);

        // clean up
        Destroy(panelGo);
    }

    public void OnOptionsClick() {
    }

    public void OnCreditsClick() {
    }

    Canvas GetCanvas() {
        // canvas should always be tagged
        var canvasGo = GameObject.FindWithTag("canvas");
        if (canvasGo != null) {
            return canvasGo.GetComponent<Canvas>();
        }
        return null;
    }

}
