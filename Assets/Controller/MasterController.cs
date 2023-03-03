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
        aircraftItem,
        enemyItem,
        situationItem
    }

    public enum ItemName //Using this enumerated type to know what is this setting
    {
        maxEnemySpawn,
        enemyAircraftCore,
        enemySituationCore,
        enemyEnemyCore,
        playerAircraftCore,
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
}
#endregion

public class MasterController : MonoBehaviour
{
    #region Lower, medium and higher console setup
    [Header("[Console]")]
    [SerializeField] private Canvas consoleCanvas;
    [SerializeField] private TextMeshProUGUI lowerConsoleText;
    [SerializeField] private Transform higherConsoleContentTrasform;
    [SerializeField] private List<HigherConsoleItem> higherConsoleItemList;
    [SerializeField] private GameObject higherConsoleItemPrefab;
    [SerializeField] private TextMeshProUGUI mediumConsoleText;
    [SerializeField] private GameObject higherConsole;
    [SerializeField] private GameObject lowerConsole;
    [SerializeField] private GameObject mediumConsole;

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
    [Header("[Level]")]
    [SerializeField] private string nextLevel;

    [Header("[Essentials]")]
    [SerializeField] private Camera emptyCamera; //This camera is setup for opening settings canvas
    [SerializeField] private GameObject domainRoot; //Whole environment
    [SerializeField] private GameObject unitsRoot;

    //Helpers
    public GamePhase CurrentGamePhase { get; set; }
    #endregion

    #region Spawning units setup
    [Header("[Player Spawn]")]
    [SerializeField] GameObject currentPlayer;
    [SerializeField] private Transform playerSpawnLocation;
    [SerializeField] private float playerSpeedMultiplier;

    [Header("[Enemy Spawn]")] //Later we will be adding arrays of enemies prefab for more variety spawn or we can create a scriptable object for test cases with exact positions and prefabs.
    [SerializeField] private int maxEnemyCount = 1;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemySpawnLocation;
    [SerializeField] private int enemySpawnAllowWhen = 3;

    [Header("[Cores]")]
    [SerializeField] private List<AircraftCore> aircraftCores = new List<AircraftCore>();
    [SerializeField] private List<SituationCore> situationCores = new List<SituationCore>();
    [SerializeField] private List<EnemyCore> enemyCores = new List<EnemyCore>();

    //Helpers
    private int currentEnemyCount = 0;
    private bool allowEnemySpawn = false;
    private List<GameObject> enemySpawnedList = new List<GameObject>();
    private GameObject playerSpawned;
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
                    SceneManager.LoadScene(nextLevel);
                    break;
            }
            PhaseRun();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(CurrentGamePhase == GamePhase.Termination) { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
        }
    }
    private void PhaseRun() //Checking which phase to run by reading current Game Phase
    {
        if (CurrentGamePhase == GamePhase.Preparation) { PreparationRun(); }
        if (CurrentGamePhase == GamePhase.Execution) { ExecutionRun(); }
        if (CurrentGamePhase == GamePhase.Intermission) { IntermissionRun(); }
        if (CurrentGamePhase == GamePhase.Termination) { TerminationRun(); }

    }
    #endregion

    #region Running preperation
    private void PreparationRun()
    {
        CheckItemList(higherConsoleItemList);
        domainRoot.SetActive(true);
        unitsRoot.SetActive(false);
        emptyCamera.gameObject.SetActive(true);
        consoleCanvas.gameObject.SetActive(true);
        mediumConsole.gameObject.SetActive(false);
        lowerConsoleIsWorking = true;
        foreach (HigherConsoleItem HCI in higherConsoleItemList)
        {
            GameObject newHCI = Instantiate(higherConsoleItemPrefab, higherConsoleContentTrasform);
            HigherConsoleItemSemiCore newHCISemiCore = newHCI.GetComponent<HigherConsoleItemSemiCore>();
            newHCISemiCore.ConsoleInstructionText = HCI.ConsoleInstruction;
            RestrictInput(HCI, newHCISemiCore);
            HCI.ItemSpawned = newHCI;
        }
        LowerConsoleUpdate($"Welcome to scene - {SceneManager.GetActiveScene().name}. Current Phase - {CurrentGamePhase}.\nChange game settings by using the options above in the scroll view.\nPress N to start the game.", LowerConsoleTaskType.StayDefault);
    }

    private void CheckItemList(List<HigherConsoleItem> HCIL)
    {
        HashSet<HigherConsoleItem.ItemName> names = new HashSet<HigherConsoleItem.ItemName>();
        foreach (HigherConsoleItem item in HCIL)
        {
            if (!names.Add(item.ConsoleItemName))
            {
                Debug.LogError("Fail#1: Two or more HigherConsoleItems have the same ConsoleItemName: " + item.ConsoleItemName);
            }
        }
    }

    public void FillDropdown<T>(TMP_Dropdown dropdown, List<T> options) where T : UnityEngine.Object
    {
        // Clear the dropdown's current options
        dropdown.ClearOptions();

        // Create a new list of dropdown options
        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

        // Loop through the list of options and add them to the dropdown
        foreach (T option in options)
        {
            string optionName = option.name;
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData(optionName);
            dropdownOptions.Add(newOption);
        }

        // Add the options to the dropdown
        dropdown.AddOptions(dropdownOptions);
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
                FillDropdown(newHCISemiCore.ConsoleDropdown, aircraftCores);
                break;
            case HigherConsoleItem.ItemType.situationItem:
                newHCISemiCore.InputOrDropdown(false);
                FillDropdown(newHCISemiCore.ConsoleDropdown, situationCores);
                break;
            case HigherConsoleItem.ItemType.enemyItem:
                newHCISemiCore.InputOrDropdown(false);
                FillDropdown(newHCISemiCore.ConsoleDropdown, enemyCores);
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
        emptyCamera.gameObject.SetActive(false);
        higherConsole.gameObject.SetActive(false);
        mediumConsole.gameObject.SetActive(false);
        lowerConsoleIsWorking = true;
        unitsRoot.SetActive(true);

        playerSpawned = Instantiate(currentPlayer, playerSpawnLocation);
        enemySpawnedList.Add(Instantiate(enemyPrefab, enemySpawnLocation));

        //This needs to be after spawning
        ReadSettings();

        LowerConsoleUpdate($"Session has started successfully. Current Phase - {CurrentGamePhase}\nPress N for Intermission. Press J for Enemy Spawn.", LowerConsoleTaskType.StayDefault);
    }
    private void SetSettings<T>(T what, ref T where)
    {
        //We can later stop absurd ranges through this method.

        if (EqualityComparer<T>.Default.Equals(what, default(T))) // check if 'what' is empty or not
        {
            return;
        }
        where = what;
    }

    private void ReadSettings()
    {
        foreach(HigherConsoleItem HCI in higherConsoleItemList)
        {
            if(HCI.ConsoleItemName == HigherConsoleItem.ItemName.maxEnemySpawn)
            {
                if (int.TryParse(HCI.HCISemiCore.ConsoleInputField.text, out int a)) // parse input as int
                {
                    SetSettings(a, ref maxEnemyCount); // assign parsed value to 'maxEnemyCount'
                }
            }
            else if(HCI.ConsoleItemName == HigherConsoleItem.ItemName.playerSpeedMultiplier)
            {
                if (float.TryParse(HCI.HCISemiCore.ConsoleInputField.text, out float a)) // parse input as int
                {
                    SetSettings(a, ref playerSpeedMultiplier); // assign parsed value to 'playerSpeedMultiplier'
                }
            }
            else if(HCI.ConsoleItemName == HigherConsoleItem.ItemName.enemyAircraftCore)
            {
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;
                AircraftCore selectedAircraftCore = aircraftCores[selectedOptionIndex];
                foreach (GameObject enemySpawned in enemySpawnedList)
                {
                    enemySpawned.GetComponent<EnemyController>().CoreUpdate(selectedAircraftCore);
                }
            }
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.enemySituationCore)
            {
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;
                SituationCore selectedSituationCore = situationCores[selectedOptionIndex];
                foreach (GameObject enemySpawned in enemySpawnedList)
                {
                    enemySpawned.GetComponent<EnemyController>().CoreUpdate(selectedSituationCore);
                }
            }
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.enemyEnemyCore)
            {
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;
                EnemyCore selectedEnemyCore = enemyCores[selectedOptionIndex];
                foreach(GameObject enemySpawned in enemySpawnedList)
                {
                    enemySpawned.GetComponent<EnemyController>().CoreUpdate(selectedEnemyCore);
                }
            }
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.playerAircraftCore)
            {
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;
                AircraftCore selectedAircraftCore = aircraftCores[selectedOptionIndex];
                playerSpawned.GetComponent<PlayerController>().CoreUpdate(selectedAircraftCore);
            }
        }
    }
    #endregion

    #region Running intermission
    private void IntermissionRun()
    {
        higherConsole.gameObject.SetActive(false);
        mediumConsole.gameObject.SetActive(true);
        lowerConsoleIsWorking = true;

        Time.timeScale = 0;
        LowerConsoleUpdate($"Session has been paused successfully. Current Phase - {CurrentGamePhase}\nPress N for Termination. Press J for Enemy Spawn.", LowerConsoleTaskType.StayDefault);
    }
    #endregion

    #region Running termination
    private void TerminationRun()
    {
        Time.timeScale = 1;
        mediumConsole.gameObject.SetActive(false);
        lowerConsoleIsWorking = true;
        domainRoot.SetActive(false);
        unitsRoot.SetActive(false);
        emptyCamera.gameObject.SetActive(true);
        LowerConsoleUpdate($"Session has been terminated successfully. Current Phase - {CurrentGamePhase}\nPress N for going to Scene - {nextLevel}. Press R for restarting current scene.", LowerConsoleTaskType.StayDefault);
    }
    #endregion
}