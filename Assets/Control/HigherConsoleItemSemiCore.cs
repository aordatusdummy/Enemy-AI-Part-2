using UnityEngine; //Provides functionality for working with Unity game engine.
using TMPro; //Provides TextMeshPro assets for rendering high-quality text in Unity projects.

public class HigherConsoleItemSemiCore : MonoBehaviour
{
    // This class manages the UI elements of the console items in the higher console.

    // Text element that displays the instruction for the console item.
    [SerializeField] private TextMeshProUGUI consoleInstructionText;
    // Property to get or set the text of the instruction.
    public string ConsoleInstructionText { get { return consoleInstructionText.text; } set { consoleInstructionText.text = value; } }

    // Input field for console items that take input.
    [SerializeField] private TMP_InputField consoleInputField;
    // Property to get or set the input field.
    public TMP_InputField ConsoleInputField { get { return consoleInputField; } set { consoleInputField = value; } }

    // Dropdown for console items that take a selection from a list.
    [SerializeField] private TMP_Dropdown consoleDropdown;
    // Property to get or set the dropdown.
    public TMP_Dropdown ConsoleDropdown { get { return consoleDropdown; } set { consoleDropdown = value; } }

    // Method to switch between input field and dropdown based on parameter.
    public void InputOrDropdown(bool inputOpen)
    {
        consoleDropdown.gameObject.SetActive(!inputOpen);
        consoleInputField.gameObject.SetActive(inputOpen);
    }
}