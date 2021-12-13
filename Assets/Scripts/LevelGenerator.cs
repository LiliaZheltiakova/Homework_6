using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public GameObject map;
    [Header("Main Map Settings")]
    [SerializeField] private int blocksInHeight;
    [SerializeField] private int blocksInWidth;
    
    [Header("Predefined Blocks Settings")]
    [SerializeField] private GameObject startPosPrefab;
    [SerializeField] private GameObject restRoomsStartPrefab;
    [SerializeField] private LayerMask room;
    [SerializeField] private float moveAmount;
    public float startTimeBtwRoom;
    public bool stopGen;
    public GameObject[] rooms;

    [Header("Cellular Backgroubd Settings")] 
    [Range(0, 100)] 
    [SerializeField] private int threshold;

    [Range(0, 25)] 
    [SerializeField] private int obstaclesProb;
    
    [SerializeField] private GameObject[] tilePrefab; 
    [SerializeField] private ContactFilter2D bg_layer;
    [SerializeField] private int minNeighbors;
    [SerializeField] private int smoothSteps;
    
    private float minX;
    private float maxX;
    private float minY;
    private GameObject[] startPoints;
    private GameObject[] restRoomsPoints;
    
  
    private LineRenderer borders;
    private int direction;
    private int downCounter;
    private float timeBtwRoom;
    
    void Start()
    {
        //draw a border line
        borders = GetComponent<LineRenderer>();
        borders.SetPosition(1, new Vector3(0, blocksInHeight * moveAmount, 0));
        borders.SetPosition(2, new Vector3(blocksInWidth * moveAmount, blocksInHeight * moveAmount, 0));
        borders.SetPosition(3, new Vector3(blocksInWidth * moveAmount, 0, 0));
        //map borders
        minX = transform.position.x + (moveAmount / 2f);
        maxX = (blocksInWidth * moveAmount) - (moveAmount / 2f);
        minY = transform.position.y + (moveAmount / 2f);
        //set startings points
        startPoints = new GameObject[blocksInWidth];
        for (int i = 0; i < startPoints.Length; i++)
        {
            startPoints[i] = Instantiate(startPosPrefab,
                new Vector3((transform.position.x * i * moveAmount) + (moveAmount / 2f),blocksInHeight * moveAmount -
                    (moveAmount / 2f), 0f), 
                       Quaternion.identity);
            startPoints[i].transform.SetParent(map.transform);
        }

        restRoomsPoints = new GameObject[blocksInHeight * blocksInWidth];
        for (int row = 0; row < blocksInWidth; row++)
        {
            for (int column = 0; column < blocksInHeight; column++)
            {
                restRoomsPoints[row * blocksInHeight + column] =
                    Instantiate(restRoomsStartPrefab, new Vector3((row * moveAmount) + (moveAmount / 2f), (column * moveAmount) +
                        (moveAmount / 2f), 0f), Quaternion.identity);
                restRoomsPoints[row * blocksInHeight + column].transform.SetParent(map.transform);
            }
        }
        
        ResetStartingPos();
        SetBG();
        stopGen = false;
        downCounter = 0;
        direction = Random.Range(1, 6);
    }
    
    void Update()
    {
        if(!stopGen)
        {
            if (timeBtwRoom <= .0f)
            {
                Move();
                timeBtwRoom = startTimeBtwRoom;
            }
            else
            {
                timeBtwRoom -= Time.deltaTime;
            }
        }
    }

    private void ResetStartingPos()
    {
        transform.position = startPoints[Random.Range(0, blocksInWidth)].transform.position;
        GameObject block = Instantiate(rooms[0], transform.position, Quaternion.identity);
        block.transform.SetParent(map.transform);
    }

    private void SetBG()
    {
        Stack<GameObject> cells = new Stack<GameObject>();
        
        //create all cells
        for (float row = 0; row < blocksInWidth * moveAmount; row += 0.5f)
        {
            for (float col = 0; col < blocksInHeight * moveAmount; col += 0.5f)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < threshold / 100f)
                {
                    GameObject cell;
                    float prob = Random.Range(0f, 1f);
                    if (prob < obstaclesProb / 100f)
                    {
                        cell = Instantiate(tilePrefab[Random.Range(1, tilePrefab.Length)], new Vector3(row + .25f, col + .25f, 0f),
                            Quaternion.identity);
                    }

                    else
                    {
                        cell = Instantiate(tilePrefab[0], new Vector3(row + .25f, col + .25f, 0f),
                            Quaternion.identity);
                    }
                    
                    cell.transform.SetParent(map.transform);
                    cells.Push(cell);
                }
            }
        }
        //smooth cells
        for (int step = 0; step < smoothSteps; step++)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                GameObject cell = cells.Pop();
                int count = Physics2D.OverlapCollider(cell.GetComponent<Collider2D>(), bg_layer, new List<Collider2D>()); //??
                if (count < minNeighbors)
                {
                    Destroy(cell.gameObject);
                }
            } 
        }
    }
    
    private void Move()
    {
        if (direction == 1 || direction == 2)
        {
            if (transform.position.x < maxX)
            {
                downCounter = 0;
                Vector2 pos = new Vector2(transform.position.x + moveAmount, transform.position.y);
                this.transform.position = pos;

                GameObject block = Instantiate(rooms[Random.Range(0, rooms.Length - 1)], transform.position, Quaternion.identity);
                block.transform.SetParent(map.transform);
                direction = Random.Range(1, 6);
                
                if (direction == 3)
                {
                    direction = 2;
                }
                else if (direction == 4)
                {
                    direction = 5;
                }
            }
            else
            {
                direction = 5;
            }
        }
        
        else if (direction == 3 || direction == 4)
        {
            if(transform.position.x > minX)
            {
                downCounter = 0;
                Vector2 pos = new Vector2(transform.position.x - moveAmount, transform.position.y);
                this.transform.position = pos;
                
                GameObject block =  Instantiate(rooms[Random.Range(0, rooms.Length - 1)], transform.position, Quaternion.identity);
                block.transform.SetParent(map.transform);
                direction = Random.Range(3, 6);
            }
            else
            {
                direction = 5;
            }
        }
        
        else if (direction == 5)
        {
            downCounter++;
            if(transform.position.y > minY)
            {
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
                int rType = roomDetection.GetComponent<RoomType>().type;
                if (rType != 1 && rType != 3)
                {
                    if (downCounter >= 1)
                    {
                        roomDetection.GetComponent<RoomType>().RoomDestruction();
                        GameObject block = Instantiate(rooms[3], transform.position, Quaternion.identity);
                        block.transform.SetParent(map.transform);
                        
                    }
                    else
                    {
                        int randomBottomRoom = Random.Range(1, 4);
                        if (randomBottomRoom == 2)
                        {
                            randomBottomRoom = 1;
                        }

                        GameObject block1 = Instantiate(rooms[randomBottomRoom], transform.position, Quaternion.identity);
                        block1.transform.SetParent(map.transform);
                    }
                }
                Vector2 pos = new Vector2(transform.position.x, transform.position.y - moveAmount);
                this.transform.position = pos;
                
                GameObject block2 = Instantiate(rooms[Random.Range(2, 4 )], transform.position, Quaternion.identity);
                block2.transform.SetParent(map.transform);
                direction = Random.Range(1, 6);
            }
            else
            {
                stopGen = true;
            }
        }
    }
}