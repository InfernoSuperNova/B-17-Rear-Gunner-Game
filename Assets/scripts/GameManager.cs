using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float enemySpawnTimer;
    public int maxEnemies;
    private float enemySpawnTimerCurrent;
    public GameObject player;
    public Enemy Enemy;
    public List<Enemy> enemies;
    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        enemySpawnTimerCurrent -= Time.deltaTime;
        if (enemySpawnTimerCurrent <= 0 && enemies.Count <= maxEnemies)
        {
            enemySpawnTimerCurrent = enemySpawnTimer;
            var enemy = Instantiate(Enemy, new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)), Quaternion.identity);
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
                Destroy(enemy.gameObject);
            }
        }

    }
}
