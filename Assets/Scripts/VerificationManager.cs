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
        choosingLocked = false;
        currentLevelIndex = 0;

        if (verificationUI != null)
            verificationUI.UpdateLevel(currentLevelIndex + 1);

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

        FeedbackUI.Message("Verificación iniciada.", 2f);

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
            labDoor.SetUnlocked(true);

        if (doorSpawner != null)
            doorSpawner.ClearDoors();

        if (verificationUI != null)
            verificationUI.ShowExplorationUI();

        if (tabletController != null)
            tabletController.HideTablet();

        if (timerSystem != null)
            timerSystem.StopTimer();

        FeedbackUI.Message("Verificación completada. Laboratorio desbloqueado.", 3f);
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

        if (consoleObject != null)
            consoleObject.SetActive(true);

        if (timerSystem != null)
            timerSystem.StopTimer();

        ResetPlayerToCheckpoint();

        if (strikeManager != null && strikeManager.HasReachedMaxStrikes())
        {
            FeedbackUI.Message("Demasiados intentos fallidos. Regresando al login.", 2f);

            StartCoroutine(ReturnToLogin());

            return;
        }

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

        if (sceneController != null)
            sceneController.GoToScene("Login");
    }
}