using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public TextMeshPro tutorialText;
    public TextMeshPro pageNumber;
    public GameObject[] pages; // Falls du mehrere Panels hast
    
    private int currentPage = 0;
    private string[] tutorialSteps = {
        "Schritt 1: Bewege dich mit dem linken Stick...",
        "Schritt 2: Greife Objekte mit dem Trigger...",
        "Schritt 3: Nutze die A-Taste, um Aktionen auszuführen..."
    };

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)) // "A"-Taste auf dem Quest-Controller
        {
            NextPage();
        }

    }

    void NextPage()
    {
        if (currentPage < tutorialSteps.Length - 1)
        {
            currentPage++;
            tutorialText.text = tutorialSteps[currentPage];
            pageNumber.text = $"{currentPage + 1}/{tutorialSteps.Length}";
        }
    }
}
