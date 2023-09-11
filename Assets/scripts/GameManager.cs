using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float enemySpawnTimer;
    public int maxEnemies;
    private float enemySpawnTimerCurrent;
    public GameObject player;
    public GameObject spawnPoint;
    public Enemy Enemy;
    [Range(0, 1)]
    public float volume;
    public Canvas mainUI;
    public Camera mainCamera;
    public GameObject hitMarker;
    public int hitMarkerAgeFrames;
    private int hitMarkerAgeCurrent;
    public GameObject enemyMarker;
    public List<Enemy> enemies;
    private Dictionary<Enemy, GameObject> enemyMarkers;
    public int scoreHit;
    public int scoreDestroyComponent;
    public int scoreKillAircraft;
    //create a volume slider for use in the editor


    public int score = 0;
    void Start()
    {
        enemyMarkers = new Dictionary<Enemy, GameObject>();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Volume", volume);
        enemies = new List<Enemy>();
    }
    public void EnemyHit()
    {
        hitMarker.SetActive(true);
        hitMarkerAgeCurrent = hitMarkerAgeFrames;
        score += scoreHit;
    }
    public void EnemyDestroyComponent()
    {
        score += scoreDestroyComponent;
    }
    public void EnemyKillAircraft()
    {
        score += scoreKillAircraft;
    }
    private void FixedUpdate()
    {
        hitMarkerAgeCurrent--;
        if (hitMarkerAgeCurrent <= 0)
        {
            hitMarker.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        enemySpawnTimerCurrent -= Time.deltaTime;
        if (enemySpawnTimerCurrent <= 0 && enemies.Count <= maxEnemies)
        {
            enemySpawnTimerCurrent = enemySpawnTimer;
            Vector3 SpawnPoint = new Vector3(spawnPoint.transform.position.x + Random.Range(-100, 100), spawnPoint.transform.position.y + Random.Range(-100, 100), spawnPoint.transform.position.z + Random.Range(-100, 100));
            var enemy = Instantiate(Enemy, SpawnPoint, spawnPoint.transform.rotation);
            enemies.Add (enemy);
        }
        List<Enemy> list = new List<Enemy>();
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
                Vector3 playerToEnemyDirNorm = (enemy.transform.position - player.transform.position).normalized;
                Vector3 cameraDir = mainCamera.transform.forward;
                float dot = Vector3.Dot(playerToEnemyDirNorm, cameraDir);
                if (dot < 0)
                {
                    enemyMarkers[enemy].SetActive(false);
                }
                else
                {
                    enemyMarkers[enemy].SetActive(true);
                }
                continue;
            }
            GameObject uiObject = Instantiate(enemyMarker, canvasPos, Quaternion.identity);

            uiObject.transform.SetParent(mainUI.transform, false);
            enemyMarkers.Add(enemy, uiObject);
        }
        }

    }
}
