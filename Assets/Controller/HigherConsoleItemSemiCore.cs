using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HigherConsoleItemSemiCore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consoleInstructionText;
    public string ConsoleInstructionText { get { return consoleInstructionText.text; } set { consoleInstructionText.text = value; } }

    [SerializeField] private TMP_InputField consoleInputField;
    public TMP_InputField ConsoleInputField { get { return consoleInputField; } set { consoleInputField = value; } }

    [SerializeField] private TMP_Dropdown consoleDropdown;
    public TMP_Dropdown ConsoleDropdown { get { return consoleDropdown; } set { consoleDropdown = value; } }
    public void InputOrDropdown(bool inputOpen)
    {
        consoleDropdown.gameObject.SetActive(!inputOpen);
        consoleInputField.gameObject.SetActive(inputOpen);
    }

}
