using NUnit.Framework;
using UnityEngine;

// Método a (Ejercicio 1) + casos límite del Ejercicio 3.
public class CoreGameplayTests
{
    // Ejercicio 2a / 3: Registrar un error o intento fallido
    [Test]
    public void AddStrike_IncrementsFailureCounterFromTwoToThree()
    {
        var manager = new GameObject().AddComponent<StrikeManager>();
        manager.AddStrike();
        manager.AddStrike();

        manager.AddStrike();

        Assert.AreEqual(3, manager.CurrentStrikes, "El contador debe pasar de 2 a 3 errores.");
        Object.DestroyImmediate(manager.gameObject);
    }

    // Ejercicio 3: Caso límite / entrada inválida: contador en el máximo permitido
    [Test]
    public void AddStrike_DoesNotExceedMaxStrikes()
    {
        var manager = new GameObject().AddComponent<StrikeManager>();

        for (int i = 0; i < manager.MaxStrikes + 3; i++)
            manager.AddStrike();

        Assert.AreEqual(manager.MaxStrikes, manager.CurrentStrikes,
            "El contador no debe superar el máximo permitido.");
        Object.DestroyImmediate(manager.gameObject);
    }

    // Ejercicio 3: Caso límite / entrada inválida: integridad no puede quedar negativa
    [Test]
    public void TakeDamage_ClampsIntegrityAtZero()
    {
        var system = new GameObject().AddComponent<IntegritySystem>();
        system.ResetIntegrity();

        for (int i = 0; i < 10; i++)
            system.TakeDamage();

        Assert.AreEqual(0f, system.CurrentIntegrity, "La integridad debe mantenerse en 0%.");
        Assert.IsTrue(system.CurrentIntegrity >= 0f, "La integridad nunca debe ser negativa.");
        Object.DestroyImmediate(system.gameObject);
    }
}