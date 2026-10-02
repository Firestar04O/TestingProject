using NUnit.Framework;
using UnityEngine;

// Ejercicio 6 - Aserción de igualdad (Assert.That + Is.EqualTo) sobre un método de cálculo.
// Método a probar: IntegritySystem.TakeDamage() (cálculo del daño: integridad - penalización).
public class EqualityAssertionTest
{
    [Test]
    public void TakeDamage_IntegrityEqualsExpected_AfterOneMistake()
    {
        // Arrange
        var system = new GameObject().AddComponent<IntegritySystem>();
        system.ResetIntegrity();
        float expected = system.MaxIntegrity - 25f; // penalización configurada en IntegritySystem.

        // Act
        system.TakeDamage();

        // Assert
        Assert.That(system.CurrentIntegrity, Is.EqualTo(expected));

        Object.DestroyImmediate(system.gameObject);
    }
}

// Ejercicio 7 - Aserción booleana (Is.True / Is.False) sobre una regla de estado.
// Método a probar: StrikeManager.HasReachedMaxStrikes() (límite de errores).
public class BooleanAssertionTest
{
    [Test]
    public void HasReachedMaxStrikes_IsFalse_BelowTheLimit()
    {
        // Arrange
        var manager = new GameObject().AddComponent<StrikeManager>();
        manager.AddStrike();
        manager.AddStrike();

        // Act
        bool reached = manager.HasReachedMaxStrikes();

        // Assert
        Assert.That(reached, Is.False, "Con 2 errores de 3, el acceso aún no debe rechazarse.");

        Object.DestroyImmediate(manager.gameObject);
    }

    [Test]
    public void HasReachedMaxStrikes_IsTrue_AtTheLimit()
    {
        // Arrange
        var manager = new GameObject().AddComponent<StrikeManager>();
        manager.AddStrike();
        manager.AddStrike();
        manager.AddStrike();

        // Act
        bool reached = manager.HasReachedMaxStrikes();

        // Assert
        Assert.That(reached, Is.True, "Al llegar al límite de 3 errores, el acceso debe rechazarse.");

        Object.DestroyImmediate(manager.gameObject);
    }
}

// Ejercicio 8 - Aserción de excepción (Assert.Throws) sobre entrada inválida.
// Método a probar: StrikeManager.SetMaxStrikes() debe rechazar capacidades inválidas.
public class ExceptionAssertionTest
{
    [Test]
    public void SetMaxStrikes_ThrowsArgumentOutOfRange_ForInvalidCapacity()
    {
        // Arrange
        var manager = new GameObject().AddComponent<StrikeManager>();

        // Act & Assert
        Assert.Throws<System.ArgumentOutOfRangeException>(() => manager.SetMaxStrikes(0));
        Assert.Throws<System.ArgumentOutOfRangeException>(() => manager.SetMaxStrikes(-5));

        Object.DestroyImmediate(manager.gameObject);
    }
}