using System.Collections;
using System.Collections.Generic;using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public class RoomType : MonoBehaviour
{
    public int type;
    
    public bool isBonusRoom = false; //closed room to open which player needs to use a bomb
    [SerializeField] private GameObject[] bonuses;

    [SerializeField] private GameObject[] enemies;
    [Range(0, 25)]
    [SerializeField] private int enemySpawnProb;

    private Vector3 objectSpawnPoint = new Vector3(2.5f, -2.5f, 0f); //hardcoded now, will random it later

    void Start()
    {
        if (isBonusRoom)
        {
            GameObject bonus =  Instantiate(bonuses[Random.Range(0, bonuses.Length)], transform.position, Quaternion.identity);
            bonus.transform.SetParent(this.transform);
            bonus.transform.localPosition = objectSpawnPoint;
        }

        float prob = Random.Range(0f, 1f);
        if (prob < enemySpawnProb / 100f)
        {
           GameObject enemie =  Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position, Quaternion.Euler(0f, 0f, 90f));
           enemie.transform.SetParent(this.transform);
           enemie.transform.localPosition = objectSpawnPoint;
        }
    }
    public void RoomDestruction()
    {
        Destroy(gameObject);
    }
}