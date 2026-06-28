using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// All-in-one: la victorie (GameEvents.OnLevelWin) isi creeaza SINGUR un ecran
// (Canvas + poza pe tot ecranul, deasupra HUD-ului), opreste jocul si, la orice
// tasta/click, se intoarce in meniu. NU trebuie sa modifici niciun UI/HUD.
// Se pune pe orice GameObject din scena de joc. Asignezi doar sprite-ul win_screen.
public class LevelWinManager : MonoBehaviour
{
    [Header("Ecran de victorie")]
    [Tooltip("Trage aici sprite-ul win_screen (Texture Type = Sprite (2D and UI)).")]
    [SerializeField] private Sprite winImage;

    [Header("Settings")]
    [SerializeField] private string menuSceneName = "MainMenu"; // numele exact al scenei de meniu
    [SerializeField] private bool freezeGame = true;            // opreste jocul (Time.timeScale = 0)
    [SerializeField] private float autoReturnSeconds = 0f;      // 0 = doar la tasta; >0 = revine si singur

    private bool won = false;
    private float timer = 0f;
    private GameObject overlay;

    void OnEnable() { GameEvents.OnLevelWin += HandleWin; }
    void OnDisable() { GameEvents.OnLevelWin -= HandleWin; }

    private void HandleWin()
    {
        if (won) return;
        won = true;
        timer = 0f;

        BuildOverlay();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (freezeGame) Time.timeScale = 0f;
    }

    // Construieste in cod un Canvas propriu cu poza, fara sa atinga UI-ul existent.
    private void BuildOverlay()
    {
        overlay = new GameObject("WinScreenOverlay");
        var canvas = overlay.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // deasupra HUD-ului

        // fundal negru (ca sa arate curat pe orice raport de ecran)
        var bg = new GameObject("BG");
        bg.transform.SetParent(overlay.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.black;
        Stretch(bgImg.rectTransform);

        // poza de victorie
        var imgGO = new GameObject("WinImage");
        imgGO.transform.SetParent(overlay.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.sprite = winImage;
        img.preserveAspect = true;
        Stretch(img.rectTransform);

        if (winImage == null)
            Debug.LogWarning("LevelWinManager: nu ai asignat 'Win Image' (sprite-ul win_screen).");
    }

    private void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void Update()
    {
        if (!won) return;

        timer += Time.unscaledDeltaTime; // merge si cu Time.timeScale = 0

        bool key = timer > 0.5f && Input.anyKeyDown;
        bool timeout = autoReturnSeconds > 0f && timer >= autoReturnSeconds;

        if (key || timeout)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }
    }
}