using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DoorOption doorPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private VerificationManager verificationManager;

    // --- Tarea de optimización (Tabla 10 - Lab14): Object Pooling ---
    // Antes: cada nivel y cada reintento hacían Instantiate() y Destroy() de las puertas.
    // Eso generaba picos de memoria/GC Allocated y, con muchos reintentos, contribuía
    // al comportamiento descrito en BUG-004 (exceso de puertas). Ahora las puertas se
    // reutilizan desde un pool en lugar de destruirse.
    private readonly Queue<DoorOption> doorPool = new Queue<DoorOption>();
    private readonly List<DoorOption> activeDoors = new List<DoorOption>();

    private readonly Dictionary<DoorType, Sprite> lastSpriteByType = new Dictionary<DoorType, Sprite>();

    // --- Tarea de optimización (Tabla 10 - Lab14): reducir GC Allocated ---
    // Antes: GetUniqueSprite y BuildSpawnSelection creaban List<> y HashSet<> nuevos
    // en cada llamada (varias veces por nivel). Ahora se reutilizan buffers de instancia.
    private readonly HashSet<Sprite> spritesUsedThisLevel = new HashSet<Sprite>();
    private readonly List<Sprite> preferredCandidatesBuffer = new List<Sprite>();
    private readonly List<Sprite> availableCandidatesBuffer = new List<Sprite>();
    private readonly List<DoorType> spawnSelectionBuffer = new List<DoorType>();
    private readonly List<int> correctIndicesBuffer = new List<int>();

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

        spritesUsedThisLevel.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            DoorOption door = GetDoorFromPool();
            door.transform.SetPositionAndRotation(spawnPoints[i].position, spawnPoints[i].rotation);
            door.gameObject.SetActive(true);

            bool isCorrect = i == correctIndex;
            Sprite evidenceSprite = null;

            if (levelConfig.theme != null)
            {
                evidenceSprite = GetUniqueSprite(levelConfig.theme, shuffledTypes[i]);

                if (evidenceSprite != null)
                    spritesUsedThisLevel.Add(evidenceSprite);
            }

            door.Initialize(
                shuffledTypes[i],
                isCorrect,
                evidenceSprite
            );

            door.SetVerificationManager(verificationManager);

            activeDoors.Add(door);
        }
    }

    private DoorOption GetDoorFromPool()
    {
        while (doorPool.Count > 0)
        {
            DoorOption pooled = doorPool.Dequeue();

            if (pooled != null)
                return pooled;
        }

        return Instantiate(doorPrefab);
    }

    private Sprite GetUniqueSprite(DoorTheme theme, DoorType doorType)
    {
        Sprite[] pool = theme.GetSpritesByType(doorType);

        if (pool == null || pool.Length == 0)
            return null;

        lastSpriteByType.TryGetValue(doorType, out Sprite previousSprite);

        preferredCandidatesBuffer.Clear();
        availableCandidatesBuffer.Clear();

        foreach (Sprite sprite in pool)
        {
            if (sprite == null || spritesUsedThisLevel.Contains(sprite))
                continue;

            availableCandidatesBuffer.Add(sprite);

            if (sprite != previousSprite)
                preferredCandidatesBuffer.Add(sprite);
        }

        List<Sprite> candidates = preferredCandidatesBuffer.Count > 0
            ? preferredCandidatesBuffer
            : availableCandidatesBuffer;

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
        spawnSelectionBuffer.Clear();
        spawnSelectionBuffer.AddRange(allTypes);

        if (count >= spawnSelectionBuffer.Count)
            return spawnSelectionBuffer;

        correctIndicesBuffer.Clear();

        for (int i = 0; i < spawnSelectionBuffer.Count; i++)
        {
            if (spawnSelectionBuffer[i] == correctType)
                correctIndicesBuffer.Add(i);
        }

        List<DoorType> selected = new List<DoorType>();

        if (correctIndicesBuffer.Count > 0)
        {
            int guaranteedIndex = correctIndicesBuffer[Random.Range(0, correctIndicesBuffer.Count)];
            selected.Add(spawnSelectionBuffer[guaranteedIndex]);
            spawnSelectionBuffer.RemoveAt(guaranteedIndex);
        }
        else
        {
            Debug.LogError(
                "El nivel no tiene ninguna puerta del tipo correcto entre sus doorTypes configurados. " +
                "Revisa correctDoorType en el LevelConfig."
            );
        }

        while (selected.Count < count && spawnSelectionBuffer.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnSelectionBuffer.Count);
            selected.Add(spawnSelectionBuffer[randomIndex]);
            spawnSelectionBuffer.RemoveAt(randomIndex);
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
        foreach (DoorOption door in activeDoors)
        {
            if (door == null)
                continue;

            door.gameObject.SetActive(false);
            doorPool.Enqueue(door);
        }

        activeDoors.Clear();
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
