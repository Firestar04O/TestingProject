using System.Collections;
using UnityEngine;

public class VerificationManager : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool verificationActive;
    [SerializeField] private bool verificationCompleted;
    [SerializeField] private bool choosingLocked;

    [Header("References")]
    [SerializeField] private GameObject consoleObject;
    [SerializeField] private SlidingDoorGroup entranceDoor;
    [SerializeField] private LockedLabDoor labDoor;
    [SerializeField] private VerificationUI verificationUI;
    [SerializeField] private TabletFocusController tabletController;
    [SerializeField] private IntegritySystem integritySystem;
    [SerializeField] private StrikeManager strikeManager;
    [SerializeField] private TimerSystem timerSystem;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private MinigameTutorialUI tutorialUI;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform verificationCheckpoint;

    [Header("Level Config")]
    [SerializeField] private LevelConfig[] levels;
    [SerializeField] private int currentLevelIndex = 0;

    [Header("Spawner")]
    [SerializeField] private DoorSpawner doorSpawner;

    [Header("Timing")]
    [SerializeField] private float feedbackDelay = 1.5f;

    public bool VerificationActive => verificationActive;
    public bool VerificationCompleted => verificationCompleted;
    public int CurrentLevelIndex => currentLevelIndex;
    public int LevelCount => levels != null ? levels.Length : 0;

    public void ConfigureLevels(LevelConfig[] newLevels)
    {
        levels = newLevels;
    }

    private bool TryGetCurrentLevel(out LevelConfig level)
    {
        if (levels != null && currentLevelIndex >= 0 && currentLevelIndex < levels.Length)
        {
            level = levels[currentLevelIndex];
            return true;
        }

        level = null;
        return false;
    }

    private void SpawnCurrentLevel()
    {
        if (doorSpawner == null)
            return;

        if (TryGetCurrentLevel(out LevelConfig level))
            doorSpawner.SpawnLevel(level);
    }

    public void StartVerification()
    {
        if (verificationActive)
            return;

        if (verificationCompleted)
        {
            FeedbackUI.Message("La verificación ya fue completada.", 2f);
            return;
        }

        if (LevelCount == 0)
        {
            FeedbackUI.Message("No hay niveles configurados.", 2f);
            return;
        }

        if (verificationUI != null)
            verificationUI.ShowVerificationUI();

        if (integritySystem != null)
            integritySystem.ResetIntegrity();

        if (tabletController != null)
            tabletController.ShowTablet();

        verificationActive = true;
        verificationCompleted = false;
        choosingLocked = true;
        currentLevelIndex = 0;

        if (consoleObject != null)
            consoleObject.SetActive(false);

        if (entranceDoor != null)
        {
            entranceDoor.Close();
            entranceDoor.Lock();
        }

        if (labDoor != null)
            labDoor.SetUnlocked(false);

        ResetPlayerToCheckpoint();

        if (tutorialUI != null)
        {
            FeedbackUI.Message("Revisa las instrucciones antes de comenzar.", 2f);
            tutorialUI.Show(BeginFirstLevel);
        }
        else
        {
            BeginFirstLevel();
        }
    }

    private void BeginFirstLevel()
    {
        if (!verificationActive)
            return;

        choosingLocked = false;

        if (verificationUI != null)
            verificationUI.UpdateLevel(currentLevelIndex + 1);

        FeedbackUI.Message("Verificación iniciada. Analiza las tres evidencias.", 2f);
        SpawnCurrentLevel();

        if (timerSystem != null)
            timerSystem.StartTimer();
    }

    public void ChooseDoor(DoorOption selectedDoor)
    {
        if (!verificationActive)
            return;

        if (choosingLocked)
            return;

        choosingLocked = true;

        if (selectedDoor.IsCorrectDoor())
            StartCoroutine(HandleCorrectChoice());
        else
            StartCoroutine(HandleWrongChoice());
    }

    private IEnumerator HandleCorrectChoice()
    {
        if (timerSystem != null)
            timerSystem.StopTimer();

        FeedbackUI.Message("Acceso permitido.", 2f);

        yield return new WaitForSeconds(feedbackDelay);

        ResetPlayerToCheckpoint();
        NextLevel();
    }

    private IEnumerator HandleWrongChoice()
    {
        if (timerSystem != null)
            timerSystem.StopTimer();

        FeedbackUI.Message("Acceso denegado.", 2f);

        if (integritySystem != null)
            integritySystem.TakeDamage();

        yield return new WaitForSeconds(feedbackDelay);

        if (!verificationActive)
            yield break;

        ResetPlayerToCheckpoint();

        SpawnCurrentLevel();

        if (timerSystem != null)
            timerSystem.StartTimer();

        choosingLocked = false;
    }

    private void ResetPlayerToCheckpoint()
    {
        if (player != null && verificationCheckpoint != null)
            player.position = verificationCheckpoint.position;
    }

    public void NextLevel()
    {
        currentLevelIndex++;

        if (!TryGetCurrentLevel(out LevelConfig level))
        {
            CompleteVerification();
            return;
        }

        if (verificationUI != null)
            verificationUI.UpdateLevel(currentLevelIndex + 1);

        FeedbackUI.Message($"Nivel {currentLevelIndex + 1}", 2f);

        if (doorSpawner != null)
            doorSpawner.SpawnLevel(level);

        if (timerSystem != null)
            timerSystem.StartTimer();

        choosingLocked = false;
    }

    public void CompleteVerification()
    {
        verificationActive = false;
        verificationCompleted = true;
        choosingLocked = false;

        if (labDoor != null)
            labDoor.UnlockAndOpen();

        if (doorSpawner != null)
            doorSpawner.ClearDoors();

        if (verificationUI != null)
            verificationUI.ShowExplorationUI();

        if (tabletController != null)
            tabletController.HideTablet();

        if (timerSystem != null)
            timerSystem.StopTimer();

        FeedbackUI.Message("Verificación completada. Acceso al laboratorio habilitado.", 3f);
    }

    public void FailVerification()
    {
        verificationActive = false;
        verificationCompleted = false;
        choosingLocked = false;

        if (strikeManager != null)
            strikeManager.AddStrike();

        if (doorSpawner != null)
            doorSpawner.ClearDoors();

        if (verificationUI != null)
            verificationUI.ShowExplorationUI();

        if (tabletController != null)
            tabletController.HideTablet();

        if (timerSystem != null)
            timerSystem.StopTimer();

        ResetPlayerToCheckpoint();

        if (strikeManager != null && strikeManager.HasReachedMaxStrikes())
        {
            // No reactivamos la consola aquí: si se habilita de inmediato, el jugador puede volver
            // a interactuar con ella durante los 2s de espera de ReturnToLogin y reiniciar la
            // verificación antes de que la escena cambie (BUG-003). Se reactiva recién dentro de
            // ReturnToLogin, justo antes de ir a Login.
            FeedbackUI.Message("Demasiados intentos fallidos. Regresando al login.", 2f);

            StartCoroutine(ReturnToLogin());

            return;
        }

        if (consoleObject != null)
            consoleObject.SetActive(true);

        FeedbackUI.Message("Verificación fallida. Puedes intentarlo otra vez.", 2f);
    }
    public void TimeExpired()
    {
        if (!verificationActive)
            return;

        if (choosingLocked)
            return;

        choosingLocked = true;

        StartCoroutine(HandleTimeExpired());
    }
    private IEnumerator HandleTimeExpired()
    {
        if (timerSystem != null)
            timerSystem.StopTimer();

        FeedbackUI.Message("Tiempo agotado.", 2f);

        if (integritySystem != null)
            integritySystem.TakeDamage();

        yield return new WaitForSeconds(feedbackDelay);

        if (!verificationActive)
            yield break;

        ResetPlayerToCheckpoint();

        SpawnCurrentLevel();

        if (timerSystem != null)
            timerSystem.StartTimer();

        choosingLocked = false;
    }
    private IEnumerator ReturnToLogin()
    {
        yield return new WaitForSeconds(2f);

        if (consoleObject != null)
            consoleObject.SetActive(true);

        if (sceneController != null)
            sceneController.GoToScene("Login");
    }
}
