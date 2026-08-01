using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    // Player Position Coordinates
    public float pX, pY, pZ;

    // Flashlight Parameters
    public float batteryPercentage;

    // Inventory Matrix Slots
    public List<string> storedItems = new List<string>();

    // Facility State Tracking
    // We store the unique IDs of doors that have been permanently opened
    public List<string> openedDoorIDs = new List<string>();

    // Keypad Puzzle Tracking
    public List<string> solvedKeypadIDs = new List<string>();

    // Enemy Position Tracking
    public float eX, eY, eZ;
    public int enemyWaypointIndex;

    public List<string> usedSaveTerminalIDs = new List<string>();
}