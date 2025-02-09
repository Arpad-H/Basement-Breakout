using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    // Textfelder
    public TextMeshPro tutorialText;  // Anzeige des aktuellen Schrittes
    public TextMeshPro pageNumber;    // z. B. "1 / 6"
    public TextMeshPro headerText;    // Überschrift (Tutorial, Game Over, etc.)

    // Bilder für den A-Button (drücke A)
    public Image buttonPromptImage;       // Das Bild, das den A-Button zeigt
    public Sprite normalButtonSprite;     // Standardzustand (A-Button nicht gedrückt)
    public Sprite pressedButtonSprite;    // Gedrückter Zustand (A-Button gedrückt)
    public AudioClip buttonSound;         // Sound, der beim Drücken abgespielt wird

    // Bilder für den Controller-Button je Tutorial-Schritt
    public Image tutorialControllerImage; // Anzeige für den Controller-Button pro Schritt
    public Sprite[] tutorialControllerSprites; // Array mit Sprites für jeden Tutorial-Schritt

    // Main Canvas
    public Canvas mainCanvas;
    
    // Optional: Falls du mehrere Panels/Pages hast, kannst du hier ein Array definieren
    // public GameObject[] pages;

    private CanvasGroup canvasGroup;
    private int currentPage = 0;
    private string[] steps;              // Tutorial-Schritte (Textinhalt)
    private bool isGameOverMode = false; // Kennzeichnet, ob wir im GameOver-/Win-Modus sind
    private GameManager.GameState currentGameState;

    private AudioSource audioSource;     // Zum Abspielen des Button-Sounds

    private void Awake()
    {
        // Hole den CanvasGroup-Komponent, damit du die UI bequem ein-/ausblenden kannst
        canvasGroup = GetComponent<CanvasGroup>();
        HideUI();

        // Stelle sicher, dass ein AudioSource vorhanden ist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        // Abonniere das Event, um über GameState-Änderungen informiert zu werden
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        // Überprüfe, ob der A-Button (OVRInput.Button.One) gedrückt wurde.
        // (Passe die Eingabemethode bei Bedarf an deine Steuerung an)
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            // Starte die Coroutine, die zuerst den Bildwechsel & Sound abspielt und danach die Seite wechselt.
            StartCoroutine(HandleButtonPress());
        }
    }

    /// <summary>
    /// Gibt beim Drücken des A-Buttons visuelles Feedback und spielt einen Sound ab,
    /// bevor die nächste Seite aufgerufen wird.
    /// </summary>
    private IEnumerator HandleButtonPress()
    {
        // Setze das Bild auf den gedrückten Zustand
        if (buttonPromptImage != null && pressedButtonSprite != null)
        {
            buttonPromptImage.sprite = pressedButtonSprite;
        }

        // Spiele den Button-Sound ab
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        // Kurze Wartezeit (z. B. 0,1 Sekunden), damit der gedrückte Zustand sichtbar ist
        yield return new WaitForSeconds(0.1f);

        // Setze das Bild wieder in den Normalzustand
        if (buttonPromptImage != null && normalButtonSprite != null)
        {
            buttonPromptImage.sprite = normalButtonSprite;
        }

        // Wechsel zur nächsten Seite
        NextPage();
    }

    /// <summary>
    /// Reagiert auf GameState-Änderungen und lädt je nach Zustand den entsprechenden Content.
    /// </summary>
    private void OnGameStateChanged(GameManager.GameState newState)
    {
        currentGameState = newState;

        if (newState == GameManager.GameState.Tutorial)
        {
            isGameOverMode = false;
            // Hier deine Tutorial-Schritte (Text)
            steps = new string[]
            {
                "Hallo! Drücke A zum Fortfahren.",
                "Bewege dich mit dem linken Stick.",
                "Drehe die Kamera mit dem rechten Stick.",
                "Greife Objekte mit dem Trigger.",
                "Interagiere mit Gegenständen durch Drücken der X-Taste.",
                "So gewinnst du: Erreiche das Ziel!"
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
                "Du hast das Spiel gewonnen.",
                "Drücke A zum Neustarten."
            };

            currentPage = 0;
            headerText.text = "Gewonnen!";
            UpdateUI();
            ShowUI();
        }
        else if (newState == GameManager.GameState.Drowned || newState == GameManager.GameState.ElectricShock)
        {
            isGameOverMode = true;
            steps = new string[]
            {
                "Oh nein!",
                "Du bist gestorben.",
                "Drücke A zum Neustarten."
            };

            currentPage = 0;
            headerText.text = "Game Over";
            UpdateUI();
            ShowUI();
        }
        else
        {
            // Bei anderen Zuständen wird die UI ausgeblendet.
            HideUI();
        }
    }

    /// <summary>
    /// Aktualisiert den angezeigten Text, den Seitenzähler und das Controller-Bild (falls vorhanden).
    /// </summary>
    private void UpdateUI()
    {
        if (steps != null && steps.Length > 0)
        {
            tutorialText.text = steps[currentPage];
            pageNumber.text = $"{currentPage + 1} / {steps.Length}";
        }

        // Aktualisiere das Bild für den aktuellen Tutorial-Schritt, sofern ein entsprechendes Sprite hinterlegt wurde.
        if (tutorialControllerImage != null && tutorialControllerSprites != null && tutorialControllerSprites.Length > currentPage)
        {
            tutorialControllerImage.sprite = tutorialControllerSprites[currentPage];
        }
    }

    /// <summary>
    /// Schaltet zur nächsten Seite. Nach dem letzten Schritt wird im Tutorial-Modus
    /// lediglich die UI ausgeblendet (statt den GameState zu ändern).
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
            // kontrollausgabe der schritte um zusehen warum hideui nicht wirklich ausgeführt wird
            
            
            
            // Letzter Schritt erreicht.
            if (isGameOverMode)
            {
                // Restart-State – derzeit auskommentiert, falls du ihn später aktivieren möchtest.
                // GameManager.Instance.RestartGame();
            }
            else
            {
                // Im Tutorial: Nur die UI ausblenden.
                HideUI();
            }
        }
    }

    /// <summary>
    /// Blendet die UI ein.
    /// </summary>
    private void ShowUI()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }

    /// <summary>
    /// Blendet die UI aus.
    /// </summary>
    private void HideUI()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
