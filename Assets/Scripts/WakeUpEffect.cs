using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WakeUpEffect : MonoBehaviour
{
    public PostProcessVolume volume;
    private DepthOfField dof;
    private Vignette vignette;
    private float duration = 10f; // Gesamtdauer des Effekts in Sekunden
    private float time = 0f;

    void Start()
    {
        // Post-Processing-Effekte abrufen
        volume.profile.TryGetSettings(out dof);
        volume.profile.TryGetSettings(out vignette);

        // Anfangswerte setzen für einen extremen Effekt
        if (dof != null)
        {
            dof.focusDistance.value = 0.05f; // Maximale Unschärfe
            dof.aperture.value = 1.0f;
            dof.focalLength.value = 100f;
            
        }

        if (vignette != null)
        {
            vignette.intensity.value = 1f; // Vollständige Abdunklung
            vignette.smoothness.value = 0.5f;
            vignette.roundness.value = 1f;
            vignette.rounded.value = true;
        }
    }

    void Update()
    {
        if (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            if (dof != null)
            {
                // Unschärfe allmählich reduzieren
                dof.focusDistance.value = Mathf.Lerp(0.05f, 10f, t);
            }

            if (vignette != null)
            {
                // Blinzeleffekt erzeugen, der etwa 5 Mal blinkt
                float blink = Mathf.Abs(Mathf.Cos(2 * Mathf.PI * 5 * t)); // 5 Blinzler
                vignette.intensity.value = blink * Mathf.Lerp(1f, 0f, t);
            }
        }
        else
        {
            // Nach Ablauf der Dauer Effekte deaktivieren

            // Depth of Field zurücksetzen oder deaktivieren
            if (dof != null)
            {
                dof.enabled.value = false; // Effekt deaktivieren
            }

            // Vignette deaktivieren
            if (vignette != null)
            {
                vignette.enabled.value = false; // Effekt deaktivieren
            }

            // Optional: Post-Processing-Volume deaktivieren
            volume.enabled = false;

            // Skript deaktivieren, falls es nicht mehr benötigt wird
            this.enabled = false;
        }
    }
}
