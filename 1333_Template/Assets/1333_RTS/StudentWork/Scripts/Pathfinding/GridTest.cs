using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridTest : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathfinder;
    [SerializeField] private Transform startMarker;
    [SerializeField] private Transform endMarker;
    [SerializeField] private LineRenderer pathLine;

    [Header("Randomization Settings")] 
    [SerializeField] private float markerHeight = 0.5f;
    [SerializeField] private TerrainType[] availableTerrainTypes;
    [SerializeField] private int currentSeed = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI controlsText;

    private System.Random seededRandom;

    private void Start()
    {
        /*
        if(!ValidateReferences())
        {
            Debug.LogError("GridTest: Missing required references. Please assign all required components");
            enabled = false;    
            return;
        }


        gridManager.InitializeGrid();
        /*
        GenerateNewSeed();
        StartCoroutine(RandomizeAndPathFind());
        */
        UpdateControlsText();
    }

    private void UpdateControlsText()
    {
        if (controlsText != null)
        {
            controlsText.text = "Controls:\n" +
                "R- Generate new random seed and resert\n" +
                "T- Reset using current seed\n" +
                "Space - Pause/Resume pathfinding\n" +
                "Right Arrow- Step forward (when paused)\n" +
                $"Current Seed: {currentSeed}";
        }
    }


}
