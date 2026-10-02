using UnityEngine;

[System.Serializable]
public class DoorTheme
{
    [Header("Theme")]
    public string themeName;

    [Header("Sprites")]
    public Sprite[] verySafeSprites;
    public Sprite[] intermediateSprites;
    public Sprite[] veryFakeSprites;

    public Sprite GetRandomSprite(DoorType doorType)
    {
        Sprite[] selectedSprites = GetSpritesByType(doorType);

        if (selectedSprites == null || selectedSprites.Length == 0)
        {
            Debug.LogWarning($"El tema {themeName} no tiene sprites para {doorType}.");
            return null;
        }

        int randomIndex = Random.Range(0, selectedSprites.Length);
        return selectedSprites[randomIndex];
    }

    private Sprite[] GetSpritesByType(DoorType doorType)
    {
        switch (doorType)
        {
            case DoorType.VerySafe:
                return verySafeSprites;

            case DoorType.Intermediate:
                return intermediateSprites;

            case DoorType.VeryFake:
                return veryFakeSprites;

            default:
                return null;
        }
    }
}