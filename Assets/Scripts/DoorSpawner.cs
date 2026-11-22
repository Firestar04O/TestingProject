using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DoorOption doorPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private VerificationManager verificationManager;
    private readonly List<GameObject> spawnedDoors = new List<GameObject>();
    private readonly Dictionary<DoorType, Sprite> lastSpriteByType = new Dictionary<DoorType, Sprite>();

    public void SpawnLevel(LevelConfig levelConfig)
    {
        ClearDoors();

        if (levelConfig == null)
        {
            Debug.LogWarning("LevelConfig vacío.");
            return;
        }

        int spawnCount = Mathf.Min(spawnPoints.Length, levelConfig.doorTypes.Length);

        if (spawnPoints.Length < levelConfig.doorTypes.Length)
        {
            Debug.LogWarning(
                $"Hay más tipos de puerta ({levelConfig.doorTypes.Length}) que puntos de aparición " +
                $"({spawnPoints.Length}). Se seleccionará un subconjunto que garantiza incluir la puerta correcta."
            );
        }

        List<DoorType> shuffledTypes = BuildSpawnSelection(
            levelConfig.doorTypes,
            levelConfig.correctDoorType,
            spawnCount
        );
        Shuffle(shuffledTypes);

        int correctIndex = GetCorrectDoorIndex(shuffledTypes, levelConfig.correctDoorType);

        HashSet<Sprite> spritesUsedThisLevel = new HashSet<Sprite>();

        for (int i = 0; i < spawnCount; i++)
        {
            DoorOption door = Instantiate(
                doorPrefab,
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            bool isCorrect = i == correctIndex;
            Sprite evidenceSprite = null;

            if (levelConfig.theme != null)
            {
                evidenceSprite = GetUniqueSprite(
                    levelConfig.theme,
                    shuffledTypes[i],
                    spritesUsedThisLevel
                );

                if (evidenceSprite != null)
                    spritesUsedThisLevel.Add(evidenceSprite);
            }

            door.Initialize(
                shuffledTypes[i],
                isCorrect,
                evidenceSprite
            );

            door.SetVerificationManager(verificationManager);

            spawnedDoors.Add(door.gameObject);
        }
    }

    private Sprite GetUniqueSprite(
        DoorTheme theme,
        DoorType doorType,
        HashSet<Sprite> spritesUsedThisLevel
    )
    {
        Sprite[] pool = theme.GetSpritesByType(doorType);

        if (pool == null || pool.Length == 0)
            return null;

        lastSpriteByType.TryGetValue(doorType, out Sprite previousSprite);

        List<Sprite> preferredCandidates = new List<Sprite>();
        List<Sprite> availableCandidates = new List<Sprite>();

        foreach (Sprite sprite in pool)
        {
            if (sprite == null || spritesUsedThisLevel.Contains(sprite))
                continue;

            availableCandidates.Add(sprite);

            if (sprite != previousSprite)
                preferredCandidates.Add(sprite);
        }

        List<Sprite> candidates = preferredCandidates.Count > 0
            ? preferredCandidates
            : availableCandidates;

        if (candidates.Count == 0)
        {
            Debug.LogWarning(
                $"El tema {theme.themeName} no dispone de suficientes evidencias distintas " +
                $"para evitar repeticiones en el nivel ({doorType})."
            );
            return theme.GetRandomSprite(doorType);
        }

        Sprite selectedSprite = candidates[Random.Range(0, candidates.Count)];
        lastSpriteByType[doorType] = selectedSprite;
        return selectedSprite;
    }
    /// <summary>
    /// Elige exactamente "count" tipos de puerta a partir de todos los tipos configurados en el nivel,
    /// garantizando que al menos una puerta del tipo correcto quede incluida en el subconjunto.
    /// Evita que la puerta correcta quede fuera cuando hay más tipos que puntos de aparición.
    /// </summary>
    private List<DoorType> BuildSpawnSelection(DoorType[] allTypes, DoorType correctType, int count)
    {
        List<DoorType> pool = new List<DoorType>(allTypes);

        if (count >= pool.Count)
            return pool;

        List<int> correctIndices = new List<int>();

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] == correctType)
                correctIndices.Add(i);
        }

        List<DoorType> selected = new List<DoorType>();

        if (correctIndices.Count > 0)
        {
            int guaranteedIndex = correctIndices[Random.Range(0, correctIndices.Count)];
            selected.Add(pool[guaranteedIndex]);
            pool.RemoveAt(guaranteedIndex);
        }
        else
        {
            Debug.LogError(
                "El nivel no tiene ninguna puerta del tipo correcto entre sus doorTypes configurados. " +
                "Revisa correctDoorType en el LevelConfig."
            );
        }

        while (selected.Count < count && pool.Count > 0)
        {
            int randomIndex = Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return selected;
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
