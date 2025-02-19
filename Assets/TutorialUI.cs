using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    // UI-Elemente (im Inspector zuweisen)
    public TextMeshPro tutorialText;      // Text für den aktuellen Tutorial-Schritt
    public TextMeshPro pageNumber;        // z. B. "1 / 6"
    public TextMeshPro headerText;        // Überschrift (z. B. "Tutorial", "Game Over", "Gewonnen!")
    
    // Button-Feedback: Hier ein Bild, das den A-Button zeigt
    public Image buttonPromptImage;           // Zeigt den A-Button (Standard und gedrückt)
    public Sprite normalButtonSprite;         // Standardzustand (A-Button nicht gedrückt)
    public Sprite pressedButtonSprite;        // Gedrückter Zustand (A-Button gedrückt)
    public AudioClip buttonSound;             // Sound, der beim Drücken abgespielt wird

    // Controller-Bild pro Tutorial-Schritt
    public Image tutorialControllerImage;     // Bild, das den entsprechenden Controller-Button anzeigt
    public Sprite[] tutorialControllerSprites;  // Array mit den Sprites für jeden Tutorial-Schritt

    public CanvasGroup canvasGroup;           
    
    
    // Private Variablen
    private int currentPage = 0;
    private string[] steps;                   // Tutorial-Schritte (Text)
    private bool isGameOverMode = false;      // Kennzeichnet, ob wir im GameOver-/Win-Modus sind
    private GameManager.GameState currentGameState;
    private AudioSource audioSource;          // Für den Button-Sound

    private void Awake()
    {
        // Stelle sicher, dass ein AudioSource vorhanden ist.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Zu Beginn ausblenden (das GameObject, an dem dieses Script hängt, deaktivieren)
        //HideUI();
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        // Überprüfe, ob der A-Button gedrückt wurde (OVRInput; ggf. an deine Eingabe anpassen)
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            StartCoroutine(HandleButtonPress());
        }
    }

    /// <summary>
    /// Gibt beim Drücken des A-Buttons visuelles Feedback (Bildwechsel) und spielt einen Sound ab.
    /// Anschließend wird zur nächsten Seite gewechselt.
    /// </summary>
    private IEnumerator HandleButtonPress()
    {
        // Setze das Buttonbild auf den gedrückten Zustand
        if (buttonPromptImage != null && pressedButtonSprite != null)
        {
            buttonPromptImage.sprite = pressedButtonSprite;
        }

        // Spiele den Sound ab
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        // Kurze Wartezeit, damit der gedrückte Zustand sichtbar ist
        yield return new WaitForSeconds(0.1f);

        // Setze das Buttonbild wieder in den Normalzustand
        if (buttonPromptImage != null && normalButtonSprite != null)
        {
            buttonPromptImage.sprite = normalButtonSprite;
        }

        // Wechsle zur nächsten Seite
        NextPage();
    }

/// <summary>
/// Wird aufgerufen, wenn sich der GameState ändert. Je nach Zustand (Tutorial, Win, Game Over)
/// wird der entsprechende Inhalt in die UI geladen.
/// </summary>
private void OnGameStateChanged(GameManager.GameState newState)
{
    currentGameState = newState;

    if (newState == GameManager.GameState.Menu)
    {
        isGameOverMode = false;
        steps = new string[]
        {
            "Willkommen! Drücke A, um fortzufahren.",
            "Schiebe den Analogstick nach vorne um zu sehen wohin du dich teleportierst",
            "Drehe die Kamera, indem du den Stick nach links oder rechts bewegst.",
            "Greife Objekte mit der unteren Taste.",
            "Interagiere mit objeketen während du sie in der Hand hälst und, indem du die hintere Taste drückst.",
            "Gehe durch die Tür, um das Spiel zu starten!"
        };
        currentPage = 0;
        headerText.text = "Tutorial";
        UpdateUI();
        ShowUI();
    }
    else if (newState == GameManager.GameState.Win)
    {
        isGameOverMode = true;
        steps = new string[]
        {
            "Herzlichen Glückwunsch!",
            "Du hast gewonnen.",
            "Drücke A, um neu zu starten."
        };
        currentPage = 0;
        headerText.text = "Gewonnen!";
        UpdateUI();
        ShowUI();
    }
    else if (newState == GameManager.GameState.Drowned)
    {
        isGameOverMode = true;
        steps = new string[]
        {
            "Oh nein!",
            "Du bist ertrunken.",
            "Versuche beim nächsten mal eher zu fliehen.",
            "Drücke A, um neu zu starten."
        };
        currentPage = 0;
        headerText.text = "Game Over";
        UpdateUI();
        ShowUI();
    }
    else if (newState == GameManager.GameState.ElectricShock)
    {
        isGameOverMode = true;
        steps = new string[]
        {
            "Oh nein!",
            "Ein Stromschlag hat dich getötet.",
            "Schalte beim nächsten mal unbedingt den Strom ab!",
            "Drücke A, um neu zu starten."
        };
        currentPage = 0;
        headerText.text = "Game Over";
        UpdateUI();
        ShowUI();
    }
    else
    {
        // Für alle anderen Zustände: UI ausblenden
        HideUI();
    }
}


    /// <summary>
    /// Aktualisiert den angezeigten Text, den Seitenzähler und das Controller-Bild für den aktuellen Schritt.
    /// </summary>
    private void UpdateUI()
    {
        if (steps != null && steps.Length > 0)
        {
            tutorialText.text = steps[currentPage];
            pageNumber.text = $"{currentPage + 1} / {steps.Length}";
        }

        // Aktualisiere das Controller-Bild, sofern ein entsprechendes Sprite vorhanden ist.
        if (tutorialControllerImage != null && tutorialControllerSprites != null && tutorialControllerSprites.Length > currentPage)
        {
            tutorialControllerImage.sprite = tutorialControllerSprites[currentPage];
        }
    }

    /// <summary>
    /// Wechselt zur nächsten Seite. Beim letzten Schritt:
    /// - Im Tutorial-Modus: Wird die UI ausgeblendet.
    /// - Im GameOver-/Win-Modus: Bleibt der Restart-Code (auskommentiert) erhalten.
    /// </summary>
    private void NextPage()
    {
        if (steps == null || steps.Length == 0)
            return;

        if (currentPage < steps.Length - 1)
        {
            currentPage++;
            UpdateUI();
        }
        else
        {
            // Letzter Schritt erreicht
            if (isGameOverMode)
            {
                // Restart-Code (derzeit auskommentiert, wie gewünscht)
                SceneManager.LoadScene("NewWaterScene");
                //GameManager.restartGame();
            }
            else
            {
                // Im Tutorial: Nur die UI ausblenden
                HideUI();
            }
        }
    }

    /// <summary>
    /// Blendet die UI ein, indem das gesamte GameObject aktiviert wird.
    /// </summary>
    private void ShowUI()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// Blendet die UI aus, indem das gesamte GameObject deaktiviert wird.
    /// </summary>
    private void HideUI()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}
