using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExtraRoomsSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsRoom;
    [SerializeField] private LevelGenerator levelGen;
    
    [Range(0, 25)] 
    [SerializeField] private int bonusRoomProb;

    private void Start()
    {
        levelGen = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();
    }
    
    void Update()
    {
        Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1f, whatIsRoom);
        if (roomDetection == null && levelGen.stopGen)
        {
            float prob = Random.Range(0f, 1f);
            if (prob < bonusRoomProb / 100f)
            {
                GameObject block = Instantiate(levelGen.rooms[Random.Range(0, levelGen.rooms.Length)], transform.position, Quaternion.identity);
                block.transform.SetParent(levelGen.map.transform);
                Destroy(gameObject);
            }
        }
    }
}