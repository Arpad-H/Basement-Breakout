using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Simuliert einen Aufwacheffekt durch Steuerung von Tiefenschärfe und Vignette.
/// Folgende Werte werden empfohlen:
///     post Volume:
///         wheight: 0.85
///     DoF:
///         aperture: 0.1 - 17.5
///         focalLength: 1
///         maxBlurSize: off
///     Vignette:
///         
///     
/// Autor: jasteink
/// </summary>
public class WakeUpEffect : MonoBehaviour
{
    public PostProcessVolume volume;
    private DepthOfField dof;
    private Vignette vignette;

    [Header("Effekt-Einstellungen")]
    [Tooltip("Gesamtdauer des Effekts in Sekunden")]
    public float duration = 10f;
    private float time = 0f;

    [Header("Tiefenschärfe-Einstellungen")]
    [Tooltip("Kurve zur Steuerung der Blendenöffnung über die Zeit")]
    public AnimationCurve apertureCurve = AnimationCurve.Linear(0f, 1f, 1f, 5.6f);


    [Header("Vignette-Einstellungen")]
    [Tooltip("Kurve zur Steuerung der Vignette-Intensität über die Zeit")]
    public AnimationCurve vignetteIntensityCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    /// <summary>
    /// Initialisiert die Post-Processing-Effekte und setzt die Anfangswerte basierend auf den Kurven.
    /// </summary>
    void Start()
    {
        // Post-Processing-Effekte abrufen
        volume.profile.TryGetSettings(out dof);
        volume.profile.TryGetSettings(out vignette);

        // Anfangswerte setzen basierend auf den Kurven bei t=0
        if (dof != null)
        {
            
            dof.aperture.value = apertureCurve.Evaluate(0f);
            
            
        }

        if (vignette != null)
        {
            vignette.intensity.value = vignetteIntensityCurve.Evaluate(0f);
            vignette.smoothness.value = 0.5f;
            vignette.roundness.value = 1f;
            vignette.rounded.value = true;
        }
    }

    /// <summary>
    /// Aktualisiert die Effekte in jeder Frame basierend auf der verstrichenen Zeit und den Animation Curves.
    /// </summary>
    void Update()
    {
        if (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            if (dof != null)
            {
                // Unschärfe dynamisch basierend auf der Kurve reduzieren
                dof.aperture.value = apertureCurve.Evaluate(t);
            }

            if (vignette != null)
            {
                // Vignette-Intensität dynamisch basierend auf der Kurve reduzieren
                vignette.intensity.value = vignetteIntensityCurve.Evaluate(t);
            }
        }
        else
        {
            // Effekte deaktivieren nach Ablauf der Dauer
            if (dof != null)
            {
                dof.enabled.value = false;
            }

            if (vignette != null)
            {
                vignette.enabled.value = false;
            }

            volume.enabled = false;
            this.enabled = false;
        }
    }
}
