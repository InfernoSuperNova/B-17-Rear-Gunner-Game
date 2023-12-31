using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private string levelName;
    public float enemySpawnTimer;
    public int enemyCount;
    public bool dogFightMode;
    public bool mainMenu;
    public bool respawnEnemiesToMeetCount;
    public bool drawEnemyMarkers = true;
    private float enemySpawnTimerCurrent;
    public GameObject player;
    public GameObject spawnPoint;
    public float enemySpawnConeRadius;
    public float enemySpawnDist;
    public Aircraft Enemy;
    public Canvas mainUI;
    public Camera mainCamera;
    public GameObject hitMarker;
    public int hitMarkerAgeFrames;
    private int hitMarkerAgeCurrent;
    public GameObject resultsText;
    public GameObject crosshair;
    public GameObject enemyMarker;
    public GameObject scoreCounter;
    public Color32 enemyMarkerColour;
    public float enemyMarkerOpaqueDistance;
    public float enemyMarkerTransparentDistance;
    public int timeBonus = 10;
    public int timeMaxScore = 60; //time since level load to get max score
    public int timeMinScore = 180; //time since level load to get min score
    public int scoreHit;
    public int scoreDestroyComponent;
    public int scoreKillAircraft;
    public int score = 0;
    public int hits = 0;
    public int destroyedComponents = 0;
    public int destroyedAircraft = 0;
    public float floorLevel = -3000;
    public int gameOverTime = 10;
    public int timeSinceGameEnded = 0;
    
    private TextMeshProUGUI scoreText;
    
    public List<Aircraft> enemies;
    private Dictionary<Aircraft, GameObject> enemyMarkers;
    private bool stopSpawningEnemies = false;
    private bool resultsAlreadyCalculated = false;
    

    void Start()
    {
        levelName = SceneManager.GetActiveScene().name;
        if (scoreCounter != null)
        {
            scoreText = scoreCounter.GetComponent<TextMeshProUGUI>();
        }
        enemyMarkers = new Dictionary<Aircraft, GameObject>();
        enemies = new List<Aircraft>();
    }
    public void EnemyHit()
    {
        hitMarker.SetActive(true);
        hitMarkerAgeCurrent = hitMarkerAgeFrames;
        score += scoreHit;
        hits++;
    }
    public void EnemyDestroyComponent()
    {
        score += scoreDestroyComponent;
        destroyedComponents++;
    }
    public void EnemyKillAircraft()
    {
        score += scoreKillAircraft;
        destroyedAircraft++;
    }
    private void FixedUpdate()
    {
        if (mainMenu)
        {
            return;
        }
        hitMarkerAgeCurrent--;
        if (hitMarkerAgeCurrent <= 0)
        {
            hitMarker.SetActive(false);
        }
        if (player == null && !mainMenu)
        {
            ShowResultsScreen();
        }
        if (resultsAlreadyCalculated || (player == null && !mainMenu))
        {
            timeSinceGameEnded++;
            if (timeSinceGameEnded >= gameOverTime * 60)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                int currentHighScore = PlayerPrefs.GetInt(levelName + "HighScore");
                if (score > currentHighScore)
                {
                    PlayerPrefs.SetInt(levelName + "HighScore", score);
                }
                SceneManager.LoadScene("MainMenu");

            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!mainMenu)
        {
            scoreText.text = "Score: " + score;
        }
        
        enemySpawnTimerCurrent -= Time.deltaTime;
        if (enemySpawnTimerCurrent <= 0 && enemies.Count < enemyCount && !stopSpawningEnemies)
        {
            if (mainMenu)
            {
                enemySpawnTimerCurrent = enemySpawnTimer;

                Vector3 randomPoint = Random.onUnitSphere;
                Vector3 SpawnPoint = randomPoint * enemySpawnDist;
                Quaternion randomRotation = Random.rotation;
                var enemy = Instantiate(Enemy, SpawnPoint, randomRotation);
                enemy.dogfightMode = dogFightMode;
                enemies.Add(enemy);
            }
            else
            {
                enemySpawnTimerCurrent = enemySpawnTimer;
                Vector3 spawnPointBack = -spawnPoint.transform.forward;
                //extending from the spawn point, get a random point within a cone
                //get a random point on a sphere
                Vector3 randomPoint = Random.onUnitSphere;
                int tries = 0;
                while (Vector3.Dot(randomPoint, spawnPointBack) < enemySpawnConeRadius)
                {
                    tries++;
                    randomPoint = Random.onUnitSphere;
                    if (tries > 100)
                    {
                        Debug.LogError("Failed to find a point in the cone");
                        break;
                    }
                }
                Vector3 SpawnPoint = randomPoint * enemySpawnDist + spawnPoint.transform.position;
                if (SpawnPoint.y < 0)
                {
                    SpawnPoint.y = 0;
                }
                var enemy = Instantiate(Enemy, SpawnPoint, spawnPoint.transform.rotation);
                enemy.dogfightMode = dogFightMode;
                enemies.Add(enemy);
            }

            
        }
        else if (!respawnEnemiesToMeetCount && enemies.Count == enemyCount)
        {
            stopSpawningEnemies = true;
        }
        if (enemies.Count == 0 && stopSpawningEnemies)
        {
            ShowResultsScreen();
        }
        if (drawEnemyMarkers)
        {
            EnemyMarkerManager();
        }
        ReturnToOrigin();
    }
    void EnemyMarkerManager()
    {
        List<Aircraft> list = new List<Aircraft>();
        foreach (var enemy in enemies)
        {
            list.Add(enemy);
        }
        foreach (var enemy in list)
        {
            //remove if no children
            if (enemy.transform.childCount == 0)
            {
                enemies.Remove(enemy);
                GameObject enemyMarker = enemyMarkers[enemy];
                enemyMarkers.Remove(enemy);
                Destroy(enemyMarker);
                Destroy(enemy.gameObject);
                continue;
            }

            float offsetPosY = enemy.transform.position.y + 1.5f;

            // Final position of marker above GO in world space
            Vector3 offsetPos = new Vector3(enemy.transform.position.x, offsetPosY, enemy.transform.position.z);

            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);



            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainUI.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);
            if (enemyMarkers.ContainsKey(enemy))
            {
                enemyMarkers[enemy].transform.localPosition = canvasPos;
                Vector3 playerToEnemyDist = enemy.transform.position - player.transform.position;
                Vector3 playerToEnemyDirNorm = playerToEnemyDist.normalized;
                Vector3 cameraDir = mainCamera.transform.forward;
                float enemyDistance = playerToEnemyDist.magnitude;
                //get a float between 0 and 1 based on the current distance, 0 with it at the opaque distance, 1 with it at the transparent distance
                float alpha = 1 - Mathf.Clamp((enemyDistance - enemyMarkerOpaqueDistance) / (enemyMarkerTransparentDistance - enemyMarkerOpaqueDistance), 0, 1);
                var textMesh = enemyMarkers[enemy].GetComponent<TextMeshProUGUI>();
                Color32 colour = enemyMarkerColour;
                colour.a = (byte)(alpha * 255);
                textMesh.color = colour;
                float dot = Vector3.Dot(playerToEnemyDirNorm, cameraDir);
                if (dot < 0)
                {
                    enemyMarkers[enemy].SetActive(false);
                }
                else
                {
                    enemyMarkers[enemy].SetActive(true);
                }
            }
            else
            {
                GameObject uiObject = Instantiate(enemyMarker, canvasPos, Quaternion.identity);
                Color32 colour = enemyMarkerColour;
                colour.a = (byte)(0);
                uiObject.GetComponent<TextMeshProUGUI>().color = colour;
                uiObject.transform.SetParent(mainUI.transform, false);
                enemyMarkers.Add(enemy, uiObject);
            }
            
        }
    }
    void ReturnToOrigin()
    {
        if (mainMenu)
        {
            return;
        }
        //Get the location of the player
        Vector3 playerPos = player.transform.position;
        //get a list of every gameobject in the scene
        var objects = FindObjectsOfType<GameObject>();
        var newObjects = new List<GameObject>();
        floorLevel -= playerPos.y;
        //we want to make sure the objects aren't children
        foreach (var obj in objects)
        {
            if (obj.transform.parent == null)
            {
                newObjects.Add(obj);
            }
        }
        foreach (var obj in newObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x - playerPos.x, obj.transform.position.y - playerPos.y, obj.transform.position.z - playerPos.z);
        }
        
    }
    void ShowResultsScreen()
    {
        if (resultsAlreadyCalculated)
        {
            return;
        }
        float timeMul = ((float)Time.timeSinceLevelLoad - timeMaxScore) / (timeMinScore - timeMaxScore);
        float timeScore = 1 - Mathf.Clamp(timeMul, 0, 1);
        Debug.Log(timeMul);
        Debug.Log(timeScore);
        score += (int)(timeScore * timeBonus);
        string results = "";
        results += "Results\n\n";
        results += "Hits: " + hits + "------------" + hits * scoreHit + "\n";
        results += "Components Destroyed: " + destroyedComponents + "------------" + destroyedComponents * scoreDestroyComponent + "\n";
        results += "Aircraft Destroyed: " + destroyedAircraft + "------------" + destroyedAircraft * scoreKillAircraft + "\n";
        results += "Time bonus: " + (int)(Time.timeSinceLevelLoad) + " seconds------------" + (int)(timeScore * timeBonus) + "\n";
        results += "Total: " + score;
        crosshair.SetActive(false);
        scoreCounter.SetActive(false);
        resultsText.SetActive(true);
        resultsText.GetComponent<TextMeshProUGUI>().text = results;
        
        resultsAlreadyCalculated = true;
    }
    public void RemoveAircraft(GameObject aircraft)
    {
        if (enemies.Contains(aircraft.GetComponent<Aircraft>()))
        {
            enemies.Remove(aircraft.GetComponent<Aircraft>());
            if (drawEnemyMarkers)
            {
                GameObject enemyMarker = enemyMarkers[aircraft.GetComponent<Aircraft>()];
                enemyMarkers.Remove(aircraft.GetComponent<Aircraft>());
                Destroy(enemyMarker);
            }
            Destroy(aircraft);
        }
    }
}
