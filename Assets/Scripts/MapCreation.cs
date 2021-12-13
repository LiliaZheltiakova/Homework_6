using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreation : MonoBehaviour
{
    [SerializeField] private GameObject levelGen;
    private GameObject currentMap;
    void Start()
    {
        currentMap = (GameObject) Instantiate(levelGen, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }
    }

    public void ResetLevel()
    {
        Destroy(currentMap.gameObject);
        currentMap = (GameObject) Instantiate(levelGen, transform.position, Quaternion.identity);
    }
}