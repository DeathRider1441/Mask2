using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // Necesar pentru a schimba scena

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string gameSceneName = "Level1"; // Scrie aici numele scenei de joc exact

    private VisualElement root;
    
    // Panouri (Containere)
    private VisualElement mainMenuPanel;
    private VisualElement optionsPanel;

    // Butoane
    private Button playButton;
    private Button optionsButton;
    private Button quitButton;
    private Button backButton; // Butonul de "Back" din Options

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("MenuManager: Lipsește componenta UIDocument!");
            return;
        }

        root = uiDocument.rootVisualElement;

        // 1. Găsim Panourile după nume (Setate în UI Builder)
        mainMenuPanel = root.Q<VisualElement>("Panel-Main");
        optionsPanel = root.Q<VisualElement>("Panel-Options");

        // 2. Găsim Butoanele după nume
        playButton = root.Q<Button>("btn-play");
        optionsButton = root.Q<Button>("btn-options");
        quitButton = root.Q<Button>("btn-quit");
        
        // Căutăm butonul Back în interiorul panoului de opțiuni (dacă există)
        backButton = root.Q<Button>("btn-back");

        // 3. Conectăm funcțiile la click
        if (playButton != null) playButton.clicked += OnPlayClicked;
        if (optionsButton != null) optionsButton.clicked += OnOptionsClicked;
        if (quitButton != null) quitButton.clicked += OnQuitClicked;
        if (backButton != null) backButton.clicked += OnBackClicked;

        // Start: Asigură-te că meniul principal e vizibil și opțiunile ascunse
        ShowMainMenu();
    }

    // --- FUNCȚIILE BUTOANELOR ---

    private void OnPlayClicked()
    {
        Debug.Log("Loading Game...");
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnOptionsClicked()
    {
        // Ascundem meniul principal, arătăm opțiunile
        if (mainMenuPanel != null) mainMenuPanel.style.display = DisplayStyle.None;
        if (optionsPanel != null) optionsPanel.style.display = DisplayStyle.Flex;
    }

    private void OnBackClicked()
    {
        ShowMainMenu();
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Ca să meargă și în Editor
        #endif
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.style.display = DisplayStyle.Flex;
        if (optionsPanel != null) optionsPanel.style.display = DisplayStyle.None;
    }
}