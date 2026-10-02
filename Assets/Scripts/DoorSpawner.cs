using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DoorOption doorPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private VerificationManager verificationManager;
    private readonly List<GameObject> spawnedDoors = new List<GameObject>();
    public void SpawnLevel(LevelConfig levelConfig)
    {
        ClearDoors();

        if (levelConfig == null)
        {
            Debug.LogWarning("LevelConfig vacío.");
            return;
        }

        List<DoorType> shuffledTypes = new List<DoorType>(levelConfig.doorTypes);
        Shuffle(shuffledTypes);

        int correctIndex = GetCorrectDoorIndex(shuffledTypes, levelConfig.correctDoorType);

        for (int i = 0; i < spawnPoints.Length && i < shuffledTypes.Count; i++)
        {
            DoorOption door = Instantiate(
                doorPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            bool isCorrect = i == correctIndex;
            Sprite evidenceSprite = null;

            if (levelConfig.theme != null)
                evidenceSprite = levelConfig.theme.GetRandomSprite(shuffledTypes[i]);

            door.Initialize(
                shuffledTypes[i],
                isCorrect,
                evidenceSprite
            );

            door.SetVerificationManager(verificationManager);

            spawnedDoors.Add(door.gameObject);
        }
    }
    public static bool IsLevelSolvable(LevelConfig levelConfig)
    {
        if (levelConfig == null || levelConfig.doorTypes == null)
            return false;

        for (int i = 0; i < levelConfig.doorTypes.Length; i++)
        {
            if (levelConfig.doorTypes[i] == levelConfig.correctDoorType)
                return true;
        }

        return false;
    }

    private int GetCorrectDoorIndex(List<DoorType> doorTypes, DoorType correctDoorType)
    {
        List<int> possibleCorrectDoors = new List<int>();

        for (int i = 0; i < doorTypes.Count; i++)
        {
            if (doorTypes[i] == correctDoorType)
                possibleCorrectDoors.Add(i);
        }

        if (possibleCorrectDoors.Count == 0)
        {
            Debug.LogError(
                "El nivel no tiene ninguna puerta del tipo correcto. Se elegirá una al azar, " +
                "por lo que la solución deja de ser determinista. Revisa doorTypes y correctDoorType."
            );
            return Random.Range(0, doorTypes.Count);
        }

        int randomIndex = Random.Range(0, possibleCorrectDoors.Count);
        return possibleCorrectDoors[randomIndex];
    }
    public void ClearDoors()
    {
        foreach (GameObject door in spawnedDoors)
        {
            if (door != null)
                Destroy(door);
        }

        spawnedDoors.Clear();
    }
    private void Shuffle(List<DoorType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            DoorType temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}