using NUnit.Framework;

// Comprobacion de configuracion de niveles: un nivel solo es resoluble si
// doorTypes contiene el tipo declarado en correctDoorType.
public class LevelSolvabilityTest
{
    [Test]
    public void IsLevelSolvable_ReturnsTrue_WhenCorrectDoorTypeIsPresent()
    {
        // Arrange: nivel con una puerta VerySafe y correcta = VerySafe.
        var level = new LevelConfig
        {
            doorTypes = new[] { DoorType.VeryFake, DoorType.Intermediate, DoorType.VerySafe },
            correctDoorType = DoorType.VerySafe
        };

        // Act + Assert: la puerta correcta existe, luego el nivel es resoluble.
        Assert.That(DoorSpawner.IsLevelSolvable(level), Is.True);
    }

    [Test]
    public void IsLevelSolvable_ReturnsFalse_WhenCorrectDoorTypeIsMissing()
    {
        // Arrange: nivel sin ninguna puerta VerySafe pero con VerySafe como correcta.
        var level = new LevelConfig
        {
            doorTypes = new[] { DoorType.Intermediate, DoorType.Intermediate, DoorType.Intermediate },
            correctDoorType = DoorType.VerySafe
        };

        // Act + Assert: sin puerta correcta el nivel no es resoluble.
        Assert.That(DoorSpawner.IsLevelSolvable(level), Is.False);
    }

    [Test]
    public void IsLevelSolvable_ReturnsFalse_WhenConfigIsNull()
    {
        Assert.That(DoorSpawner.IsLevelSolvable(null), Is.False);
    }

    [Test]
    public void IsLevelSolvable_ReturnsFalse_WhenDoorTypesIsNull()
    {
        var level = new LevelConfig
        {
            doorTypes = null,
            correctDoorType = DoorType.VerySafe
        };

        Assert.That(DoorSpawner.IsLevelSolvable(level), Is.False);
    }
}
