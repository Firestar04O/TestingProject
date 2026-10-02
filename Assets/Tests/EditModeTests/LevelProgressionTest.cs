using NUnit.Framework;
using UnityEngine;

// Ejercicio 2e / 9 - Avanzar al siguiente nivel del reto.
// Método a probar: VerificationManager.NextLevel() (regla de progresión de niveles).
public class LevelProgressionTest
{
    [Test]
    public void NextLevel_AdvancesFromLevel2ToLevel3()
    {
        // Arrange: jugador en el nivel 1 (índice 0) dentro de un reto de 3 niveles.
        var manager = new GameObject().AddComponent<VerificationManager>();
        manager.ConfigureLevels(new[]
        {
            new LevelConfig(),
            new LevelConfig(),
            new LevelConfig()
        });

        // Act: superar el nivel 1 y luego el nivel 2.
        manager.NextLevel();
        int indexAfterLevel1 = manager.CurrentLevelIndex;
        manager.NextLevel();
        int indexAfterLevel2 = manager.CurrentLevelIndex;

        // Assert: la progresión es nivel 1 -> 2 -> 3 (índices 0 -> 1 -> 2).
        Assert.That(indexAfterLevel1, Is.EqualTo(1));
        Assert.That(indexAfterLevel2, Is.EqualTo(2));

        Object.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void NextLevel_CompletesVerification_WhenLastLevelIsCleared()
    {
        // Arrange: reto de 3 niveles, jugador ya en el último (índice 2).
        var manager = new GameObject().AddComponent<VerificationManager>();
        manager.ConfigureLevels(new[]
        {
            new LevelConfig(),
            new LevelConfig(),
            new LevelConfig()
        });
        manager.NextLevel();
        manager.NextLevel();

        // Act: superar el último nivel.
        manager.NextLevel();

        // Assert: la progresión se detiene y la verificación queda completada,
        // sin acceder a un nivel fuera del rango establecido.
        Assert.That(manager.VerificationCompleted, Is.True);

        Object.DestroyImmediate(manager.gameObject);
    }

    // Ejercicio 10 - Guarda de configuración inválida.
    // levels == null provocaba un NullReferenceException al evaluar levels.Length.
    [Test]
    public void StartVerification_WithNullLevels_DoesNotStart()
    {
        // Arrange: ningún nivel configurado (levels queda en null).
        var manager = new GameObject().AddComponent<VerificationManager>();

        // Act: intentar iniciar la verificación.
        Assert.DoesNotThrow(() => manager.StartVerification());

        // Assert: la verificación no arranca y el estado inicial se conserva.
        Assert.That(manager.VerificationActive, Is.False);
        Assert.That(manager.LevelCount, Is.Zero);

        Object.DestroyImmediate(manager.gameObject);
    }

    // Ejercicio 10 - levels vacío: NextLevel no debe indexar fuera de rango.
    [Test]
    public void NextLevel_WithEmptyLevels_CompletesVerification()
    {
        // Arrange: reto sin niveles.
        var manager = new GameObject().AddComponent<VerificationManager>();
        manager.ConfigureLevels(new LevelConfig[0]);

        // Act: avanzar con el array vacío.
        Assert.DoesNotThrow(() => manager.NextLevel());

        // Assert: se completa en lugar de lanzar IndexOutOfRange.
        Assert.That(manager.VerificationCompleted, Is.True);

        Object.DestroyImmediate(manager.gameObject);
    }
}