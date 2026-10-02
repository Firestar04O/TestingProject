using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    [Header("Level")]
    public int levelNumber = 1;

    [Header("Theme")]
    public DoorTheme theme;

    [Header("Doors")]
    public DoorType[] doorTypes = new DoorType[3];

    [Header("Correct Door")]
    public DoorType correctDoorType = DoorType.VerySafe;
}