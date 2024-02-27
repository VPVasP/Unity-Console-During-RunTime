using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;


public class Console : MonoBehaviour
{
    //prefab for console text
    [SerializeField] private TextMeshProUGUI consoleTextPrefab;
    private DateTime currentTime;

    //panels to display the logs
    [Header("Panels")]
    [SerializeField] private Transform debugPanel;
    [SerializeField] private Transform warningPanel;
    [SerializeField] private Transform errorPanel;

    //positions for the texts to spawn into
    [Header("Spawn Positions")]
    [SerializeField] private Transform debugPanelSpawnPosition;
    [SerializeField] private Transform warningPanelSpawnPosition;
    [SerializeField] private Transform errorPanelSpawnPosition;

    //counters for log types
    [Header("Counters")]
    private int debugCount;
    private int warningCount;
    private int errorCount;

    //texts to display the debugs,warnings,erros
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private TextMeshProUGUI errorText;

    //buttons for console left
    [Header("Buttons")]
    [SerializeField] private Button clearConsoleButton;
    [SerializeField] private Button collapseButton;
    [SerializeField] private Button errorPauseButton;

    //buttons for console right
    [SerializeField] private Button debugConsoleButton;
    [SerializeField] private Button warningConsoleButton;
    [SerializeField] private Button errorConsoleButton;

    //colors for the buttons
    [Header("Button Colors")]
    [SerializeField] private Color normalButtonColor;
    [SerializeField] private Color pressedButtonColor;


    //dictionary to display the log types and the colors
    private Dictionary<LogType, Color> logColors = new Dictionary<LogType, Color>()
    {
        { LogType.Error, Color.red },
        { LogType.Assert, Color.red },
        { LogType.Warning, Color.yellow },
        { LogType.Log, Color.white },
        { LogType.Exception, Color.red }
    };

    void Awake()
    {

        Application.logMessageReceived += HandleLog;
        //add listeners to our buttons
        clearConsoleButton.onClick.AddListener(ClearConsoleButton);
        collapseButton.onClick.AddListener(CollapseConsoleButton);
        errorPauseButton.onClick.AddListener(ErrorPauseButton);

        debugConsoleButton.onClick.AddListener(EnableDebugPanel);
        warningConsoleButton.onClick.AddListener(EnableWarningPanel);
        errorConsoleButton.onClick.AddListener(EnableErrorPanel);
    }

    private void Update()
    {
        //if the debugs,warnings,errors are zero update the text

        if (debugCount == 0)
        {
            debugText.text = "Debug: 0";
        }
        if (warningCount == 0)
        {
            warningText.text = "Warning: 0";
        }
        if (errorCount == 0)
        {
            errorText.text = "Error: 0";
        }

    }
    #region logHandling
    private class UnityLogHandler : ILogHandler
    {
        private Console console;

        public UnityLogHandler(Console console)
        {
            this.console = console;
        }

        // log format
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            string message = string.Format(format, args);
            console.HandleLog(message, "", logType);
        }
        //log expection
        public void LogException(Exception exception, UnityEngine.Object context)
        {
            console.HandleLog(exception.Message, exception.StackTrace, LogType.Exception);
        }
    }
    //function to handle incoming logs
    public void HandleLog(string logText, string stackTrace, LogType type)
    {
        Color color;
        //get the color of the logs
        if (!logColors.TryGetValue(type, out color))
        {
            color = Color.white;
        }

        //text to spawn
        TextMeshProUGUI ConsoleTextToSpawn = Instantiate(consoleTextPrefab);
        //place the text in the correct panel based on the log
        switch (type)
        {
            case LogType.Log:
                ConsoleTextToSpawn.transform.SetParent(debugPanelSpawnPosition, false);
                debugCount++;
                debugText.text = "Debug: " + debugCount.ToString();
                break;
            case LogType.Warning:
                ConsoleTextToSpawn.transform.SetParent(warningPanelSpawnPosition, false);
                warningCount++;
                warningText.text = "Warning: " + warningCount.ToString();
                break;
            case LogType.Error:
            case LogType.Exception:
                ConsoleTextToSpawn.transform.SetParent(errorPanelSpawnPosition, false);
                errorCount++;
                errorText.text = "Error: " + errorCount.ToString();
                break;
        }
        //current time for the log
        currentTime = DateTime.Now;

        string timeText = currentTime.ToString();
        //setting up the text and the color of the text
        ConsoleTextToSpawn.text = "[" + timeText + "]" + " " + logText + " ";
        ConsoleTextToSpawn.color = color;
    }
    #endregion loghandling
    #region buttons
    //function to clear the console
    public void ClearConsoleButton()
    {
        foreach (Transform child in debugPanelSpawnPosition)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in warningPanelSpawnPosition)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in errorPanelSpawnPosition)
        {
            Destroy(child.gameObject);
        }
        //reseting the texts and the counters
        debugCount = 0;
        warningCount = 0;
        errorCount = 0;
        debugText.text = "Debug: 0";
        warningText.text = "Warning: 0";
        errorText.text = "Error: 0";
    }
    //freeze and unfreeze the game when pressing this button
    public void CollapseConsoleButton()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1f;
        }
        else
        {
            Time.timeScale = 0f;
        }

    }

    public void ErrorPauseButton()
    {
        Time.timeScale = 0;
        EnableErrorPanel();
    }
    #endregion buttons
    #region panels
    //enable the debug panel and disable the others
    public void EnableDebugPanel()
    {
        debugPanel.gameObject.SetActive(true);
        warningPanel.gameObject.SetActive(false);
        errorPanel.gameObject.SetActive(false);

        //change the color of that button and reset the others
        ChangeButtonColor(debugConsoleButton);
        ResetButtonColor(warningConsoleButton);
        ResetButtonColor(errorConsoleButton);
    }

    public void EnableWarningPanel()
    {
        debugPanel.gameObject.SetActive(false);
        warningPanel.gameObject.SetActive(true);
        errorPanel.gameObject.SetActive(false);

        //change the color of that button and reset the others
        ResetButtonColor(debugConsoleButton);
        ResetButtonColor(errorConsoleButton);
        ChangeButtonColor(warningConsoleButton);
    }

    public void EnableErrorPanel()
    {
        debugPanel.gameObject.SetActive(false);
        warningPanel.gameObject.SetActive(false);
        errorPanel.gameObject.SetActive(true);

        //change the color of that button and reset the others
        ResetButtonColor(debugConsoleButton);
        ResetButtonColor(warningConsoleButton);
        ChangeButtonColor(errorConsoleButton);
    }
    //reset the button colors back to normal
    public void ResetButtonColor(Button button)
    {
        ColorBlock block = button.colors;
        block.normalColor = normalButtonColor;
        block.highlightedColor = normalButtonColor;
        block.pressedColor = normalButtonColor;
        block.selectedColor = normalButtonColor;
        block.disabledColor = normalButtonColor;
        button.colors = block;
    }
    //change the button colors to the pressed button color
    public void ChangeButtonColor(Button button)
    {
        ColorBlock block = button.colors;
        block.normalColor = pressedButtonColor;
        block.highlightedColor = pressedButtonColor;
        block.pressedColor = pressedButtonColor;
        block.selectedColor = pressedButtonColor;
        block.disabledColor = pressedButtonColor;
        button.colors = block;

    }
  
}
#endregion panels
