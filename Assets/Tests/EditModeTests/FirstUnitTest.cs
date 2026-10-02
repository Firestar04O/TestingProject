using NUnit.Framework;
using UnityEngine;

// Ejercicio 5 - Primera prueba unitaria
// Método a probar: DoorOption.IsSafe() (el más sencillo de la lista del Ejercicio 1).
public class FirstUnitTest
{
    [Test]
    public void IsSafe_ReturnsTrue_ForACorrectDoor()
    {
        // Arrange: preparar el estado inicial del sujeto de la prueba.
        var door = new GameObject().AddComponent<DoorOption>();
        door.Initialize(DoorType.VerySafe, true, null);

        // Act: ejecutar la operación que se quiere verificar.
        bool isSafe = door.IsSafe();

        // Assert: comprobar que el resultado coincide con lo esperado.
        Assert.IsTrue(isSafe, "Una opción correcta debe considerarse segura.");

        Object.DestroyImmediate(door.gameObject);
    }
}