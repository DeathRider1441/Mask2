using UnityEngine;
using UnityEngine.SceneManagement;

// Se aboneaza la GameEvents.OnLevelWin (cand jucatorul scapa).
// Dupa ce apare "LEVEL ESCAPED", te duce inapoi in meniul principal
// la primul click / orice tasta, SAU automat dupa cateva secunde.
// Se pune pe orice obiect din scena de joc (ex: un GameObject gol numit "WinManager").
public class LevelWinManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string menuSceneName = "MainMenu"; // numele scenei de meniu
    [SerializeField] private float autoReturnSeconds = 5f;      // dupa atatea secunde se intoarce singur

    private bool won = false;
    private float timer = 0f;

    void OnEnable()  { GameEvents.OnLevelWin += HandleWin; }
    void OnDisable() { GameEvents.OnLevelWin -= HandleWin; }

    private void HandleWin()
    {
        if (won) return;
        won = true;
        timer = 0f;

        // Eliberam cursorul ca sa se poata da click.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!won) return;

        // Folosim timp nescalat, ca sa mearga si daca jocul a fost oprit (Time.timeScale = 0).
        timer += Time.unscaledDeltaTime;

        bool clickedAfterGrace = timer > 0.5f && (Input.GetMouseButtonDown(0) || Input.anyKeyDown);
        bool timedOut = timer >= autoReturnSeconds;

        if (clickedAfterGrace || timedOut)
        {
            won = false;
            Time.timeScale = 1f;                 // ne asiguram ca meniul nu ramane pe pauza
            SceneManager.LoadScene(menuSceneName);
        }
    }
}