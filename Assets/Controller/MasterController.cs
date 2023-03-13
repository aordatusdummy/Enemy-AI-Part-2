#region Using libraries
using UnityEngine; //Import Unity Engine library for Unity functions and types
using System.Collections.Generic; //Import System.Collections.Generic library for List and Dictionary
using TMPro; //Import TextMesh Pro library for text rendering
using System.Collections; //Import System.Collections library for IEnumerator and IEnumerable
using UnityEngine.SceneManagement; //Import Unity SceneManagement library for managing scenes
using System.Text; //Import System.Text library for string manipulation
using System.Reflection; //Import System.Reflection library for obtaining information about an object's type and its members
#endregion



#region Phases of the whole test run
// This enumerated type describes the different phases of the test.
public enum GamePhase
{
    Preparation, // The preparation phase, before runtime
    Execution, // The execution phase, during test runtime
    Intermission, // The intermission phase, pausing test runtime
    Termination // The termination phase, after test runtime
}
#endregion

#region Item class for Preparation settings
// This class represents an item in the higher console, used for preparation settings.
[System.Serializable] // For making a list of these objects
public class HigherConsoleItem
{
    // This enumerated type describes the different types of settings that can be added in the higher console.
    public enum ItemType
    {
        intItem, // An integer setting
        floatItem, // A float setting
        stringItem, // A string setting
        aircraftItem, // An aircraft setting
        enemyItem, // An enemy setting
        situationItem // A situation setting
    }

    // This enumerated type describes the different names of the items in the higher console.
    public enum ItemName
    {
        maxEnemySpawn, // The maximum number of enemy spawns
        enemyAircraftCore, // The core type of enemy aircraft
        enemySituationCore, // The core type of enemy situation
        enemyEnemyCore, // The core type of enemy enemy
        playerAircraftCore, // The core type of player aircraft
        playerSpeedMultiplier // The speed multiplier of the player
    }

    [SerializeField] private ItemType consoleItemType; // The type of the console item
    public ItemType ConsoleItemType { get { return consoleItemType; } }

    [SerializeField] private ItemName consoleItemName; // The name of the console item
    public ItemName ConsoleItemName { get { return consoleItemName; } }

    [SerializeField] private string consoleInstruction; // The instruction of the console item
    public string ConsoleInstruction { get { return consoleInstruction; } }

    public GameObject ItemSpawned { get; set; } // The spawned item in the game
    public HigherConsoleItemSemiCore HCISemiCore { get { return ItemSpawned.GetComponent<HigherConsoleItemSemiCore>(); } } // The semi-core component of the spawned item
}
#endregion

public class MasterController : MonoBehaviour
{
    #region Lower, medium and higher console setup
    /// <summary>
    /// Handles the setup and management of the lower, medium and higher console interfaces.
    /// </summary>
    [Header("[Console]")]
    [SerializeField] private Canvas consoleCanvas; //The canvas containing the console interfaces
    [SerializeField] private TextMeshProUGUI lowerConsoleText; //The text element in the lower console interface
    [SerializeField] private Transform higherConsoleContentTrasform; //The transform containing the items in the higher console interface
    [SerializeField] private List<HigherConsoleItem> higherConsoleItemList; //The list of items in the higher console interface
    [SerializeField] private GameObject higherConsoleItemPrefab; //The prefab used to instantiate new items in the higher console interface
    [SerializeField] private TextMeshProUGUI mediumConsoleText; //The text element in the medium console interface
    [SerializeField] private GameObject higherConsole; //The higher console interface
    [SerializeField] private GameObject lowerConsole; //The lower console interface
    [SerializeField] private GameObject mediumConsole; //The medium console interface

    //Helpers
    private string lowerConsoleDefaultText; //The default text displayed in the lower console interface
    public bool lowerConsoleIsWorking { get; set; } //Indicates if the lower console interface is currently functioning
    public enum LowerConsoleTaskType //The different types of tasks that can be assigned to the lower console interface
    {
        Temp,
        Stay,
        StayDefault,
        StayDefaultOff
    }
    #endregion

    #region General management setup
    [Header("[Level]")]
    [SerializeField] private string nextLevel; //The name of the next level to load

    [Header("[Essentials]")]
    [SerializeField] private Camera emptyCamera; //The camera used for displaying the opening settings canvas
    [SerializeField] private GameObject domainRoot; //The root object for the entire environment
    [SerializeField] private GameObject unitsRoot; //The root object for all units in the game

    //Helpers
    public GamePhase CurrentGamePhase { get; set; } //The current game phase
    private int currentViewpointIndex; //The index of the current viewpoint
    private Transform currentActiveCameraRoot; //The root object for the current active camera
    #endregion

    #region Spawning units setup
    [Header("[Player Spawn]")]
    [SerializeField] GameObject currentPlayer; //The player object
    [SerializeField] private Transform playerSpawnLocation; //The location where the player will spawn
    [SerializeField] private float playerSpeedMultiplier; //The speed multiplier for the player object

    [Header("[Enemy Spawn]")]
    [SerializeField] private int maxEnemyCount = 1; //The maximum number of enemies that can be spawned
    [SerializeField] private GameObject enemyPrefab; //The enemy prefab
    [SerializeField] private Transform enemySpawnLocation; //The location where enemies will spawn

    [Header("[Cores]")]
    [SerializeField] private List<AircraftCore> aircraftCores = new List<AircraftCore>(); //The list of aircraft cores
    [SerializeField] private List<SituationCore> situationCores = new List<SituationCore>(); //The list of situation cores
    [SerializeField] private List<EnemyCore> enemyCores = new List<EnemyCore>(); //The list of enemy cores

    //Helpers
    private int currentEnemyCount = 0; //The current number of enemies
    private bool allowEnemySpawn = true; //Indicates if enemy spawning is allowed
    private List<GameObject> enemySpawnedList = new List<GameObject>(); //The list of spawned enemy objects
    private GameObject playerSpawned; //The player object that has been spawned

    private AircraftCore selectedAircraftCore;
    private SituationCore selectedSituationCore;
    private EnemyCore selectedEnemyCore;

    #endregion

    #region Basic
    // This function is called in the beginning of the session
    private void Awake()
    {
        PreparationRun();
    }

    // This function is called once per frame by Unity
    void Update()
    {
        // If the player presses the "R" key, and the game is in the "Termination" phase, restart the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CurrentGamePhase == GamePhase.Termination)
            {
                // Reload the current scene to restart the game
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        // If the player presses the "M" key, and the game is in the "Intermission" phase, start the "Execution" phase and call the "ExecutionContinue" function
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (CurrentGamePhase == GamePhase.Intermission)
            {
                // Set the current game phase to "Execution"
                CurrentGamePhase = GamePhase.Execution;

                // Call the "ExecutionContinue" function to begin the "Execution" phase
                ExecutionContinue();
            }
        }

        // If the player presses the "J" key...
        if (Input.GetKeyDown(KeyCode.J))
        {
            // ...and the game is in the "Execution" phase and there aren't already the maximum number of enemies in the game, add a new enemy
            if (CurrentGamePhase == GamePhase.Execution && currentEnemyCount < maxEnemyCount)
            {
                // Add a new enemy to the game by instantiating the enemyPrefab at the enemySpawnLocation, and add it to the enemySpawnedList
                SpawnEnemy(enemyPrefab, enemySpawnLocation);
            }

            // ...or if the game is in the "Intermission" phase, switch the viewpoint
            if (CurrentGamePhase == GamePhase.Intermission)
            {
                // Call the "SwitchViewPoint" function to switch the viewpoint
                SwitchViewPoint();
            }
        }

        // If the player presses the "N" key, check the current game phase and execute the appropriate code
        if (Input.GetKeyDown(KeyCode.N))
        {
            // Call the "PhaseCheck" function to check the current game phase and execute the appropriate code
            PhaseCheck();
        }
    }

    // This function checks the current game phase and executes the appropriate code
    private void PhaseCheck()
    {
        switch (CurrentGamePhase)
        {
            // If the game is in the "Preparation" phase, call the "ExecutionRun" function to begin the "Execution" phase
            case GamePhase.Preparation:
                ExecutionRun();
                break;

            // If the game is in the "Execution" phase, call the "IntermissionRun" function to begin the "Intermission" phase
            case GamePhase.Execution:
                IntermissionRun();
                break;

            // If the game is in the "Intermission" phase, call the "TerminationRun" function to begin the "Termination" phase
            case GamePhase.Intermission:

                TerminationRun();
                break;

            // If the game is in the "Termination" phase, load the next level
            case GamePhase.Termination:
                SceneManager.LoadScene(nextLevel);
                break;
        }
    }

    //This function helps in running phases majorly for intermission and execution
    private void PhaseRunnerHelper1(bool exec)
    {
        //During execution the cursor is locked and during intermisison the time scale is zero 
        PhaseRunnerhelper2(exec);
     
        //Switching between execution and intermission
        mediumConsole.gameObject.SetActive(!exec);
    }

    private void PhaseRunnerhelper2(bool lockCursor)
    {
        //Lock cursor or not
        if (lockCursor) { Cursor.lockState = CursorLockMode.Locked; Time.timeScale = 1; }
        else { Cursor.lockState = CursorLockMode.None; Time.timeScale = 0; }
        Cursor.visible = !lockCursor;
    }
    #endregion

    #region Running preperation
    private void PreparationRun()
    {
        // Check for duplicate ConsoleItemName in higherConsoleItemList
        CheckItemList(higherConsoleItemList);

        // Activate domainRoot and deactivate unitsRoot
        domainRoot.SetActive(true);
        unitsRoot.SetActive(false);

        // Activate emptyCamera and consoleCanvas
        emptyCamera.gameObject.SetActive(true);
        consoleCanvas.gameObject.SetActive(true);

        // Deactivate mediumConsole and set lowerConsoleIsWorking to true
        mediumConsole.gameObject.SetActive(false);
        lowerConsoleIsWorking = true;

        // Instantiate HigherConsoleItemSemiCore for each item in higherConsoleItemList
        foreach (HigherConsoleItem HCI in higherConsoleItemList)
        {
            GameObject newHCI = Instantiate(higherConsoleItemPrefab, higherConsoleContentTrasform);
            HigherConsoleItemSemiCore newHCISemiCore = newHCI.GetComponent<HigherConsoleItemSemiCore>();

            // Set ConsoleInstructionText and input restrictions
            newHCISemiCore.ConsoleInstructionText = HCI.ConsoleInstruction;
            RestrictInput(HCI, newHCISemiCore);

            // Save the spawned HigherConsoleItem as ItemSpawned in HigherConsoleItem
            HCI.ItemSpawned = newHCI;
        }

        // Set initial text for lower console and currentViewpointIndex to 0
        LowerConsoleUpdate($"Welcome to scene - {SceneManager.GetActiveScene().name}. Current Phase - {CurrentGamePhase}.\nChange game settings by using the options above in the scroll view.\nPress N to start the game.", LowerConsoleTaskType.StayDefault);
        currentViewpointIndex = 0;
    }

    // Check for duplicate ConsoleItemName in higherConsoleItemList
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

    // Fill a TMP_Dropdown with options of type T
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
        // Get the input field component of the new console item
        TMP_InputField inputField = newHCISemiCore.ConsoleInputField;

        // Depending on the type of the console item, restrict the input field to the appropriate data type and/or fill a dropdown list
        switch (HCI.ConsoleItemType)
        {
            case HigherConsoleItem.ItemType.intItem:
                // If the console item is an integer item, enable the input field and restrict it to integer numbers
                newHCISemiCore.InputOrDropdown(true);
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                break;
            case HigherConsoleItem.ItemType.floatItem:
                // If the console item is a float item, enable the input field and restrict it to decimal numbers
                newHCISemiCore.InputOrDropdown(true);
                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                break;
            case HigherConsoleItem.ItemType.stringItem:
                // If the console item is a string item, enable the input field and allow standard text input
                newHCISemiCore.InputOrDropdown(true);
                inputField.contentType = TMP_InputField.ContentType.Standard;
                break;
            case HigherConsoleItem.ItemType.aircraftItem:
                // If the console item is an aircraft item, disable the input field and fill the dropdown with available aircraft cores
                newHCISemiCore.InputOrDropdown(false);
                FillDropdown(newHCISemiCore.ConsoleDropdown, aircraftCores);
                break;
            case HigherConsoleItem.ItemType.situationItem:
                // If the console item is a situation item, disable the input field and fill the dropdown with available situation cores
                newHCISemiCore.InputOrDropdown(false);
                FillDropdown(newHCISemiCore.ConsoleDropdown, situationCores);
                break;
            case HigherConsoleItem.ItemType.enemyItem:
                // If the console item is an enemy item, disable the input field and fill the dropdown with available enemy cores
                newHCISemiCore.InputOrDropdown(false);
                FillDropdown(newHCISemiCore.ConsoleDropdown, enemyCores);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Running lower console
    public void LowerConsoleUpdate(string newText, LowerConsoleTaskType taskType, int deleteWhen = 0, bool? featureToTurnOff = null)
    {
        // If lower console is not working, don't update it.
        if (!lowerConsoleIsWorking) { return; }

        // Set the new text to the lower console text object.
        lowerConsoleText.text = newText;

        // Handle different task types.
        if (taskType == LowerConsoleTaskType.Stay) { return; } // Keep the current text.
        if (taskType == LowerConsoleTaskType.StayDefault) { lowerConsoleDefaultText = newText; return; } // Set default text to the current text.
        if (taskType == LowerConsoleTaskType.StayDefaultOff) { lowerConsoleDefaultText = newText; lowerConsoleIsWorking = false; return; } // Set default text to the current text and turn off the lower console.

        // Turn off the feature, if specified.
        if (featureToTurnOff != null)
        {
            featureToTurnOff = false;
        }

        // Start the coroutine to delete the text after a specified delay.
        StartCoroutine(TextUpdateWorker(Mathf.Clamp(deleteWhen, 0, Mathf.Abs(deleteWhen)), featureToTurnOff));
    }

    private IEnumerator TextUpdateWorker(int deleteWhen, bool? featureToTurnOff)
    {
        // Turn off the feature, if specified.
        if (featureToTurnOff != null)
        {
            featureToTurnOff = false;
        }

        // Wait for the specified delay.
        yield return new WaitForSeconds(deleteWhen);

        // Clear the lower console text to the default text.
        ClearConsole();

        // Turn the feature back on, if specified.
        if (featureToTurnOff != null)
        {
            featureToTurnOff = true;
        }
    }

    private void ClearConsole()
    {
        // Set the lower console text to the default text.
        lowerConsoleText.text = lowerConsoleDefaultText;
    }
    #endregion

    #region Running execution
    private void ExecutionRun()
    {
        CurrentGamePhase = GamePhase.Execution;
        // Hide empty camera
        emptyCamera.gameObject.SetActive(false);

        // Activate PhaseRunnerHelper1
        PhaseRunnerHelper1(true);

        // Hide higher console and activate lower console
        higherConsole.gameObject.SetActive(false);
        lowerConsoleIsWorking = true;

        // Activate units root
        unitsRoot.SetActive(true);

        // Spawn player
        playerSpawned = Instantiate(currentPlayer, playerSpawnLocation);

        // Get current active camera root
        currentActiveCameraRoot = playerSpawned.GetComponent<PlayerController>().CameraRoot;

        // Read settings after spawning
        ReadSettings();

        // Update lower console with session start message
        LowerConsoleUpdate($"Session has started successfully. Current Phase - {CurrentGamePhase}\nPress N for Intermission. Press J for Enemy Spawn {currentEnemyCount}/{maxEnemyCount}.", LowerConsoleTaskType.StayDefault);
    }

    private void SpawnEnemy(GameObject enemy, Transform where, bool console = true)
    {
         if (!allowEnemySpawn) { return; }

        // Add spawned enemy to the list and increase enemy count
        GameObject newEnemy = Instantiate(enemy, where);
        newEnemy.GetComponent<EnemyController>().Init(selectedAircraftCore, selectedSituationCore, selectedEnemyCore);
        enemySpawnedList.Add(newEnemy);
        currentEnemyCount += 1;

        if (!console) { return; }
        LowerConsoleUpdate($"Enemy Spawned. Current Phase - {CurrentGamePhase}\nPress N for Intermission. Press J for Enemy Spawn {currentEnemyCount}/{maxEnemyCount}.", LowerConsoleTaskType.StayDefault);
    }

    private void ExecutionContinue()
    {
        CurrentGamePhase = GamePhase.Execution;
        // Activate PhaseRunnerHelper1
        PhaseRunnerHelper1(true);

        // Update lower console with session start message
        LowerConsoleUpdate($"Session has resumed successfully. Current Phase - {CurrentGamePhase}\nPress N for Intermission. Press J for Enemy Spawn {currentEnemyCount}/{maxEnemyCount}.", LowerConsoleTaskType.StayDefault);
    }

    private void SetSettings<T>(T what, ref T where)
    {
        // Check if 'what' is empty or not
        if (EqualityComparer<T>.Default.Equals(what, default(T)))
        {
            // If empty, return
            return;
        }
        // Assign 'what' to 'where'
        where = what;
    }

    private void ReadSettings()
    {
        // Loop through all the Higher Console items in the list.
        foreach (HigherConsoleItem HCI in higherConsoleItemList)
        {
            // If the Higher Console item is for the max enemy spawn setting:
            if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.maxEnemySpawn)
            {
                // Try to parse the value from the input field as an integer.
                if (int.TryParse(HCI.HCISemiCore.ConsoleInputField.text, out int a))
                {
                    // If the parsing was successful, set the 'maxEnemyCount' variable to the parsed value.
                    SetSettings(a, ref maxEnemyCount);
                }
            }
            // If the Higher Console item is for the player speed multiplier setting:
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.playerSpeedMultiplier)
            {
                // Try to parse the value from the input field as a float.
                if (float.TryParse(HCI.HCISemiCore.ConsoleInputField.text, out float a))
                {
                    // If the parsing was successful, set the 'playerSpeedMultiplier' variable to the parsed value.
                    SetSettings(a, ref playerSpeedMultiplier);
                }
            }
            // If the Higher Console item is for the enemy aircraft core setting:
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.enemyAircraftCore)
            {
                // Get the selected option index from the dropdown.
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;

                // Get the selected aircraft core from the 'aircraftCores' list using the selected option index.
                AircraftCore selectedAircraftCore = aircraftCores[selectedOptionIndex];
                this.selectedAircraftCore = selectedAircraftCore;

            }
            // If the Higher Console item is for the enemy situation core setting:
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.enemySituationCore)
            {
                // Get the selected option index from the dropdown.
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;

                // Get the selected situation core from the 'situationCores' list using the selected option index.
                SituationCore selectedSituationCore = situationCores[selectedOptionIndex];
                this.selectedSituationCore = selectedSituationCore;
            }
            // If the Higher Console item is for the enemy enemy core setting:
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.enemyEnemyCore)
            {
                // Get the selected option index from the dropdown.
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;

                // Get the selected enemy core from the 'enemyCores' list using the selected option index.
                EnemyCore selectedEnemyCore = enemyCores[selectedOptionIndex];

                this.selectedEnemyCore = selectedEnemyCore;

            }
            // if the console item is for player aircraft core
            else if (HCI.ConsoleItemName == HigherConsoleItem.ItemName.playerAircraftCore)
            {
                // get the selected option index from the dropdown
                int selectedOptionIndex = HCI.HCISemiCore.ConsoleDropdown.value;
                // get the corresponding aircraft core from the list
                AircraftCore selectedAircraftCore = aircraftCores[selectedOptionIndex];
                // update the player's aircraft core
                playerSpawned.GetComponent<PlayerController>().CoreUpdate(selectedAircraftCore);
            }
        }
    }
    #endregion

    #region Running intermission
    // This function is called when the game is in the "Intermission" phase
    // During this phase the session has been paused and the user can do data analysis of different enemies spawned by switching viewpoints
    private void IntermissionRun()
    {
        CurrentGamePhase = GamePhase.Intermission;

        // Call the "PhaseRunnerHelper1" function with "false" as the argument to disable or enable certain UI elements and features
        PhaseRunnerHelper1(false);

        // Disable the higher console game object
        higherConsole.gameObject.SetActive(false);

        MediumConsoleUpdate();

        // Update the lower console with a message indicating that the session has been paused and telling the player how to proceed
        LowerConsoleUpdate($"Session has been paused successfully. Current Phase - {CurrentGamePhase}\nPress N for Termination. Press M for continuing Execution. Press J for switching viewpoint.", LowerConsoleTaskType.StayDefault);
    }

    // This function is called when the player presses the "J" key during the "Execution" or "Intermission" phases to switch their viewpoint
    public void SwitchViewPoint()
    {
        // Increment the current viewpoint index by 1
        currentViewpointIndex += 1;

        // This function works like there is a list of all the aircrafts, 0 being player and 1-x being enimies where x is the number of enemies spawned
        // Number of aircrafts = Number of enemies (Index wise) because we have included 0 as player in the former

        // If the current viewpoint index is greater than the number of enemies in the "enemySpawnedList", reset the index to 0
        if (currentViewpointIndex > enemySpawnedList.Count)
        {
            currentViewpointIndex = 0;
        }

        // If there is a currently active camera root, disable it
        if (currentActiveCameraRoot != null)
        {
            currentActiveCameraRoot.gameObject.SetActive(false);
        }

        // If the current viewpoint index is 0, set the active camera root to the player's camera root
        if (currentViewpointIndex == 0)
        {
            currentActiveCameraRoot = playerSpawned.GetComponent<PlayerController>().CameraRoot;
        }

        // Otherwise, set the active camera root to the camera root of the enemy at the current viewpoint index minus 1 (since the first index is the player)
        else
        {
            currentActiveCameraRoot = enemySpawnedList[currentViewpointIndex - 1].GetComponent<EnemyController>().CameraRoot;
        }

        // Enable the game object associated with the current active camera root
        currentActiveCameraRoot.gameObject.SetActive(true);

        MediumConsoleUpdate();
    }

    private void MediumConsoleUpdate()
    {
        string res = "";

        // If current viewpoint index is 0, display the cores of player
        if (currentViewpointIndex == 0)
        {
            res += ReadCore(playerSpawned.GetComponent<PlayerController>().MyAicraftCore);
        }
        // Otherwise,  display the cores of enemy
        else
        {
            res += ReadCore(enemySpawnedList[currentViewpointIndex - 1].GetComponent<EnemyController>().MyAicraftCore);
            res += "\n" + ReadCore(enemySpawnedList[currentViewpointIndex - 1].GetComponent<EnemyController>().MyEnemyCore);
        }

        // Update the text of the medium console with the result
        mediumConsoleText.text = res;
    }

    // Read and return a string representation of the properties of the given ScriptableObject
    private string ReadCore(ScriptableObject scriptableObject)
    {
        StringBuilder sb = new StringBuilder();

        // Get all the fields (including private and public) of the given ScriptableObject type
        var properties = scriptableObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // For each field, append its name and value to the StringBuilder
        foreach (var property in properties)
        {
            var value = property.GetValue(scriptableObject);
            sb.AppendLine($"{property.Name} = {value}");
        }

        // Return the final string representation of the properties
        return sb.ToString();
    }

    #endregion

    #region Running termination
    // This function is called when the game is in the "Termination" phase
    // During this phase the session has ended and the user just needs to choose between restarting the current scene or loading next scene
    public void TerminationRun()
    {
        CurrentGamePhase = GamePhase.Termination;

        PhaseRunnerhelper2(false); 

        // Disable the medium console game object as viewpoint data analyisis feature is not required in termination
        mediumConsole.gameObject.SetActive(false);

        // Disable the "domainRoot" and "unitsRoot" game objects
        domainRoot.SetActive(false);
        unitsRoot.SetActive(false);

        // Enable the "emptyCamera" game object
        emptyCamera.gameObject.SetActive(true);

        // Update the lower console with a message indicating that the session has been terminated successfully and telling the player how to proceed
        LowerConsoleUpdate($"Session has been terminated successfully. Current Phase - {CurrentGamePhase}\nPress N for going to Scene - {nextLevel}. Press R for restarting current scene.", LowerConsoleTaskType.StayDefault);

        // Set the "lowerConsoleIsWorking" variable to false
        lowerConsoleIsWorking = false;
    }
    #endregion
}