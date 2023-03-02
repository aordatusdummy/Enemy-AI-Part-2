using UnityEngine; //Connective bridge with game engine
using System.Collections.Generic; //Lists and dictionaries
using TMPro; //Text Mesh Pro for better text GUI
using System.Collections; //Functions like IEnumerators
using UnityEngine.SceneManagement;

#region Phases of the whole test run
public enum GamePhase //Using this enumerated type to describe the test phases will be useful to maintain clarity of the current status in the game
{
    Preparation, //Before runtime
    Execution, //During test runtime
    Intermission, //Pausing test runtime
    Termination //After test runtime
}
#endregion

#region Item class for Preparation settings
[System.Serializable] //For making a list of these objects
public class HigherConsoleItem
{
    public enum ItemType //Using this enumerated type to know what type of setting option will be added in higher console
    {
        intItem,
        floatItem,
        stringItem,
        aircraftItem
    }

    public enum ItemName //Using this enumerated type to know what is this setting
    {
        maxEnemySpawn,
        enemyAircraft,
        playerAircraft,
        playerSpeedMultiplier
    }

    [SerializeField] private ItemType consoleItemType;
    public ItemType ConsoleItemType { get { return consoleItemType; } }

    [SerializeField] private ItemName consoleItemName;
    public ItemName ConsoleItemName { get { return consoleItemName; } }

    [SerializeField] private string consoleInstruction;
    public string ConsoleInstruction { get { return consoleInstruction; } }
    public GameObject ItemSpawned { get; set; }
    public HigherConsoleItemSemiCore HCISemiCore { get { return ItemSpawned.GetComponent<HigherConsoleItemSemiCore>(); } }

    public 
}
#endregion

public class MasterController : MonoBehaviour
{
    #region Lower and higher console setup
    [Header("Console")]
    [SerializeField] private Canvas consoleCanvas;
    [SerializeField] private TextMeshProUGUI lowerConsoleText;
    [SerializeField] private Transform higherConsoleContentTrasform;
    [SerializeField] private List<HigherConsoleItem> higherConsoleItemList;
    [SerializeField] private GameObject higherConsoleItemPrefab;

    //Helpers
    private string lowerConsoleDefaultText;
    public bool lowerConsoleIsWorking { get; set; }
    public enum LowerConsoleTaskType
    {
        Temp,
        Stay,
        StayDefault,
        StayDefaultOff
    }
    #endregion

    #region General management setup
    [Header("Level")]
    [SerializeField] private string nextLevel;

    [Header("Essentials")]
    [SerializeField] private Camera emptyCamera; //This camera is setup for opening settings canvas
    [SerializeField] private GameObject domainRoot; //Whole environment

    //Helpers
    public GamePhase CurrentGamePhase { get; set; }
    #endregion

    #region Spawning units setup
    [Header("Player Spawn")]
    [SerializeField] GameObject currentPlayer;
    [SerializeField] private Transform playerSpawnLocation;

    [Header("Enemy Spawn")] //Later we will be adding arrays of enemies prefab for more variety spawn or we can create a scriptable object for test cases with exact positions and prefabs.
    [SerializeField] private int maxEnemyCount = 1;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemySpawnLocation;
    [SerializeField] private int enemySpawnAllowWhen = 3;

    //Helpers
    private int currentEnemyCount = 0;
    private bool allowEnemySpawn = false;
    #endregion

    #region Basic
    private void Awake()
    {
        PhaseRun();
    }
    void Update()
    {
        PhaseCheck();
    }
    private void PhaseCheck() //Press N and go to the next data
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            switch (CurrentGamePhase)
            {
                case GamePhase.Preparation:
                    CurrentGamePhase = GamePhase.Execution;
                    break;
                case GamePhase.Execution:
                    CurrentGamePhase = GamePhase.Intermission;
                    break;
                case GamePhase.Intermission:
                    CurrentGamePhase = GamePhase.Termination;
                    break;
                case GamePhase.Termination:
                    Debug.Log("Cannot go to next phase from Termination.");
                    break;
            }
            PhaseRun();
        }
    }
    private void PhaseRun() //Checking which phase to run by reading current Game Phase
    {
        if (CurrentGamePhase == GamePhase.Preparation) { PreparationRun(); }
        if (CurrentGamePhase == GamePhase.Execution) { ExecutionRun(); }
    }
    #endregion

    #region Running preperation
    private void PreparationRun()
    {
        domainRoot.SetActive(true);
        emptyCamera.gameObject.SetActive(true);
        consoleCanvas.gameObject.SetActive(true);
        lowerConsoleIsWorking = true;
        foreach (HigherConsoleItem HCI in higherConsoleItemList)
        {
            GameObject newHCI = Instantiate(higherConsoleItemPrefab, higherConsoleContentTrasform);
            HigherConsoleItemSemiCore newHCISemiCore = newHCI.GetComponent<HigherConsoleItemSemiCore>();
            newHCISemiCore.ConsoleInstructionText = HCI.ConsoleInstruction;
            RestrictInput(HCI, newHCISemiCore);
            HCI.ItemSpawned = newHCI;
        }
        LowerConsoleUpdate($"Welcome to scene - {SceneManager.GetActiveScene().name}.\nChange game settings by using the options above in the scroll view.\nPress N to start the game.", LowerConsoleTaskType.StayDefault);
    }
    public void RestrictInput(HigherConsoleItem HCI, HigherConsoleItemSemiCore newHCISemiCore)
    {
        TMP_InputField inputField = newHCISemiCore.ConsoleInputField;

        switch (HCI.ConsoleItemType)
        {
            case HigherConsoleItem.ItemType.intItem:
                newHCISemiCore.InputOrDropdown(true);
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                break;
            case HigherConsoleItem.ItemType.floatItem:
                newHCISemiCore.InputOrDropdown(true);
                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                break;
            case HigherConsoleItem.ItemType.stringItem:
                newHCISemiCore.InputOrDropdown(true);
                inputField.contentType = TMP_InputField.ContentType.Standard;
                break;
            case HigherConsoleItem.ItemType.aircraftItem:
                newHCISemiCore.InputOrDropdown(false);
                // Set content type for aircraft item
                break;
            default:
                break;
        }
    }
    #endregion

    #region Running lower console
    public void LowerConsoleUpdate(string newText, LowerConsoleTaskType taskType, int deleteWhen = 0)
    {
        if (!lowerConsoleIsWorking) { return; }
        lowerConsoleText.text = newText;

        if (taskType == LowerConsoleTaskType.Stay) { return; }
        if (taskType == LowerConsoleTaskType.StayDefault) { lowerConsoleDefaultText = newText; return; }
        if (taskType == LowerConsoleTaskType.StayDefaultOff) { lowerConsoleDefaultText = newText; lowerConsoleIsWorking = false; return; }

        StartCoroutine(TextUpdateWorker(Mathf.Clamp(deleteWhen, 0, Mathf.Abs(deleteWhen))));
    }
    private IEnumerator TextUpdateWorker(int deleteWhen)
    {
        yield return new WaitForSeconds(deleteWhen);
        ClearConsole();
    }

    private void ClearConsole()
    {
        lowerConsoleText.text = lowerConsoleDefaultText;
    }
    #endregion

    #region Running execution
    private void ExecutionRun()
    {
        ReadSettings();
        emptyCamera.gameObject.SetActive(false);
        consoleCanvas.gameObject.SetActive(false);
        lowerConsoleIsWorking = true;

        Instantiate(currentPlayer, playerSpawnLocation);
        Instantiate(enemyPrefab, enemySpawnLocation);
    }

    private void ReadSettings()
    {
        foreach(HigherConsoleItem HCI in higherConsoleItemList)
        {
            if(HCI.ConsoleItemName == HigherConsoleItem.ItemName.maxEnemySpawn)
            {
                maxEnemyCount = int.Parse(HCI.HCISemiCore.ConsoleInputField.text);
            }
            else if(HCI.ConsoleItemName == HigherConsoleItem.ItemName.playerSpeedMultiplier)
            {
                var a = int.Parse(HCI.HCISemiCore.ConsoleInputField.text);
                //Do Something
            }
            else if(HCI.ConsoleItemName == HigherConsoleItem.ItemName.enemyAircraft)
            {
                var a = HCI.HCISemiCore.ConsoleDropdown.value;
                //Do Something
            }
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.playerAircraft)
            {
                var a = HCI.HCISemiCore.ConsoleDropdown.value;
                //Do Something
            }
        }
    }
    #endregion
}
