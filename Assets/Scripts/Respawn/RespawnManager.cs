﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> listOfRespawns;
    [SerializeField] private GameObject character;

    private PlayerInputHandler playerInputHandler;

    void Start()
    {
        playerInputHandler = character.GetComponent<PlayerInputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInputHandler.playerIsDead)
        {
            character.transform.position = listOfRespawns[listOfRespawns.Count -1].transform.position;
            playerInputHandler.playerIsDead = false;
        }
            
    }
    public void AddRespawnPoint(GameObject newRespawnPoint)
    {
        listOfRespawns.Add(newRespawnPoint);
    }
}