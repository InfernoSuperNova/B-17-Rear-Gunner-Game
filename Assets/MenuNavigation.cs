
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuNavigation : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levels;
    public GameObject settings;
    public GameObject credits;
    public int levelCount = 5;
    public List<string> levelNames;
    public GameObject levelButtonPrefab;
    public GameObject levelMenuContainer;
    private int levelButtonsX;
    private int levelButtonsY;
    public int levelButtonSpacingX;
    public int levelButtonSpacingY;
    public int levelButtonStartX;
    public int levelButtonStartY;
    public int levelButtonMaxX;


    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Volume", 0.5f);
        ConstructLevelsMenu();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void NavigateToMainMenu()
    {
        mainMenu.SetActive(true);
        levels.SetActive(false);
        settings.SetActive(false);
        credits.SetActive(false);
    }
    public void NavigateToLevels()
    {
        mainMenu.SetActive(false);
        levels.SetActive(true);
        settings.SetActive(false);
        credits.SetActive(false);
    }
    public void NavigateToSettings()
    {
        mainMenu.SetActive(false);
        levels.SetActive(false);
        settings.SetActive(true);
        credits.SetActive(false);
    }
    public void NavigateToCredits()
    {
        mainMenu.SetActive(false);
        levels.SetActive(false);
        settings.SetActive(false);
        credits.SetActive(true);
    }
    public void NavigateToQuit()
    {
        Application.Quit();
    }

    public void UpdateVolume(float volume)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Volume", volume);
    }

    private void ConstructLevelsMenu()
    {
        levelButtonsX = levelButtonStartX;
        levelButtonsY = levelButtonStartY;
        for (int i = 0; i < levelCount; i++)
        {
            int levelIndex = i + 1;
            var level = Instantiate(levelButtonPrefab, new Vector3(levelButtonsX, levelButtonsY, 0), Quaternion.identity, levelMenuContainer.transform);

            string levelName = "level" + levelIndex;
            if (levelNames[i] != null)
            {
                levelName = levelNames[i];
            }
            //get the child gameobject called LevelButton
            var levelButton = level.transform.GetChild(0).gameObject;
            var levelButtonText = levelButton.transform.GetChild(0).gameObject;
            levelButtonText.GetComponent<TextMeshProUGUI>().text = levelName;

            var highScoreText = level.transform.GetChild(1).gameObject;
            highScoreText.GetComponent<TextMeshProUGUI>().text = ("High Score: " + PlayerPrefs.GetInt("level" + levelIndex + "HighScore")).ToString();

            
            //set the callback function for the button
            Button button = levelButton.GetComponent<Button>();
            button.onClick.AddListener(delegate { SetLevel(levelIndex); });
            levelButtonsX += levelButtonSpacingX;
            if (levelButtonsX > levelButtonMaxX)
            {
                levelButtonsX = levelButtonStartX;
                levelButtonsY -= levelButtonSpacingY;
            }
        }
    }

    public void SetLevel(int level)
    {
        Debug.Log(level);
        SceneManager.LoadScene("level" + level);
    }
}
