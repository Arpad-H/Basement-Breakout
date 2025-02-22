using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using UnityEngine.XR;


//TODO raum drrhen ausprobieren

public class PlayerManager : MonoBehaviour
{
    // try jan
    [Header("recenter")]
    [Tooltip("Center Eye Anchor des OVRCameraRig (Headset-Position)")]
    [SerializeField] public Transform Head; 

    [Tooltip("Gesamter Spieler (Parent-Objekt von OVRCameraRig)")]
    [SerializeField] public Transform origin; 

    [Tooltip("Ziel-Transform als Referenz für das Recentering")]
    [SerializeField] public Transform sceneMenueTarget;
    
    [SerializeField] public Transform startSceneTarget;
    
    
    
    private Vector3 STARTSCENEPOS = new Vector3(-1.13f, 0.9593f, 2.1740f);
    private Vector3 STARTSCENEROTAION = new Vector3(0f, 180f, 0f);
    private Vector3 STARTMENUPOS = new Vector3(-16.158f, 3.122f, 4.676f);
    private Vector3 STARTMENUROTATION = new Vector3(0f, 90f, 0f);

    [SerializeField] private GameObject[] rayInteractor;
    [SerializeField] private GameObject[] teleportInteractor;
    [SerializeField] private GameObject PlayerHead;
    [SerializeField] private GameObject WaterheightPlane;
    [SerializeField] public Transform GameMenuRoom;
    [SerializeField] public Transform SceneRoom;
    [SerializeField] public OVRCameraRig cameraRig;
    [SerializeField] public bool startAtMenu = true;

    [SerializeField] private float DROWNINGTIME = 10f;
    private float _timeUnderWater = 0f;
    public AudioMixer drowningMixer; 
    [SerializeField] private GameObject waterPlane;
    [SerializeField] private Material screenMaterial;
    public static event Action<GameManager.GameState> GameStateChangedPlayer;

    public float fadeDuration = 1f; 
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private Coroutine fadeCoroutine;
    
    
    
    [SerializeField]private AudioSource _audioSource;
    [SerializeField]private AudioSource stairsSound;
    [SerializeField]private AudioSource drowiningSound;
    private AudioClip _introducingTVClip;
    private OVRManager _ovrManager;
    private bool _setPlayerAtStartMenue = false;


    private void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void Start()
    {
        //DeactivateTeleportInteractor();
        ActivateTeleportInteractor();
        DeactivateRayInteractor();
      
        
        _ovrManager = GetComponent<OVRManager>();
        if (startAtMenu)
        {
            setPlayerPosToStartMenu();
        }else if (!startAtMenu)
        {
            GameStateChangedPlayer?.Invoke(GameManager.GameState.Tutorial);
        }
        
        //_introducingTV = Resources.Load<AudioClip>("Audio/voice/Line1fin.mp3");
        //_audioSource.clip = _introducingTV;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
    int _counter = 0;
    private void Update()
    {
        if (Drawing())
        {
            GameStateChangedPlayer?.Invoke(GameManager.GameState.Drowned);
        }
        _counter++;
        if (Head.position != origin.position && !_setPlayerAtStartMenue)
        {
            setPlayerPosToStartMenu();
            _setPlayerAtStartMenue = true;
        }
    }

    public void SetPlayerPositionToStartGame()
    {
        
        recenterPlayer(startSceneTarget);
    }

    


    public void setPlayerPosToStartMenu()
    {
        recenterPlayer(sceneMenueTarget);
    }

    private void DeactivateRayInteractor()
    {
        foreach (GameObject gameObject in rayInteractor)
        {
            gameObject.SetActive(false);
        }
    }

    private void ActivateRayInteractor()
    {
        foreach (GameObject gameObject in rayInteractor)
        {
            gameObject.SetActive(true);
        }
    }

    private void DeactivateTeleportInteractor()
    {
        foreach (GameObject gameObject in teleportInteractor)
        {
            gameObject.SetActive(false);
        }
    }

    private void ActivateTeleportInteractor()
    {
        foreach (GameObject gameObject in teleportInteractor)
        {
            gameObject.SetActive(true);
        }
    }

    private void AcivatePassthrough()
    {
        //_ovrManager.isInsightPassthroughEnabled = true;
    }

    private void DeactivatePassthrough()
    {
        // _ovrManager.isInsightPassthroughEnabled = false;
    }

    public void SetPlayerPosToGameOverMenu()
    {
        recenterPlayer(sceneMenueTarget);
    }

    public bool Drawing()
    {
        
        if (IsUnderwater()) 
        {
            _timeUnderWater += Time.deltaTime;

            if (_timeUnderWater > DROWNINGTIME / 2 && !isFadingIn)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine); 
                fadeCoroutine = StartCoroutine(FadeDrowningSound(-80f, 0f)); // Fade in
                isFadingIn = true;
                isFadingOut = false;
            }
        }
        else
        {
            if (isFadingIn && !isFadingOut) 
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine); 
                fadeCoroutine = StartCoroutine(FadeDrowningSound(0f, -80f)); // Fade out
                isFadingOut = true;
                isFadingIn = false;
            }
            _timeUnderWater = 0f; // Reset underwater time when above water
        }

        return _timeUnderWater > DROWNINGTIME;
    }
    bool IsUnderwater()
    {
        return Head.position.y < WaterheightPlane.transform.position.y;
    }
    IEnumerator FadeDrowningSound(float startVolume, float targetVolume)
    {
        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, t);
            drowningMixer.SetFloat("DrowningVolume", newVolume);
            yield return null;
        }
        
        drowningMixer.SetFloat("DrowningVolume", targetVolume);
    }
    
    private void HandleGameStateChanged(GameManager.GameState gameState)
    {
        Debug.Log($"[PlayerManager]: GameState changed to {gameState}");
        if (gameState is GameManager.GameState.Drowned or GameManager.GameState.Win
            or GameManager.GameState.ElectricShock)
        {
            //AcivatePassthrough();
            
            //DeactivateTeleportInteractor();
            //ActivateRayInteractor();
            SetPlayerPosToGameOverMenu();
            Debug.Log($"[PlayerManager]: SetPlayerPosToGameOverMenu();");
        }
        else if (gameState == GameManager.GameState.Tutorial)
        {
            StartCoroutine(TransitionToBlack()); //TODO potentially deactivate player movement
            DeactivateTeleportInteractor();
         
            
        }
        else if (gameState == GameManager.GameState.Game)
        {
            ActivateTeleportInteractor();
        }
    }
    
    IEnumerator TransitionToBlack()
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f, 0.5f)); // Fade to full black over 0.5s

        // Play a different sound
        stairsSound.Play();
        DeactivateRayInteractor();
        SetPlayerPositionToStartGame();

        // Wait a few seconds
        yield return new WaitForSeconds(2f); // Adjust delay as needed
    stairsSound.Stop();
    ActivateTeleportInteractor();
    _audioSource.Play();
        // Fade back to normal
        yield return StartCoroutine(Fade(0f, 0.5f)); // Fade back over 0.5s
    }

    IEnumerator Fade(float targetAlpha, float duration)
    {
        
        float startAlpha = screenMaterial.GetFloat("_alpha");
        float elapsedTime = 0f;
    
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
           
            screenMaterial.SetFloat("_alpha", alpha);
            yield return null;
        }

        // Ensure final alpha is set
       
        screenMaterial.SetFloat("_alpha", targetAlpha);
    }
    

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("GameBorder"))
        {
            Debug.Log($"[PlayerManager]: Entering collision with {other.gameObject.name}");
            AcivatePassthrough();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GameBorder"))
        {
            Debug.Log($"[PlayerManager]: Exiting collision with {other.gameObject.name}");
            DeactivatePassthrough();
        }
    }
    
    
    /*
     * Quelle
     * https://www.youtube.com/watch?v=NOCXB_ETKrM
     */
    public void recenterPlayer(Transform target)
    {
        Vector3 offset = Head.position - origin.position;
        offset.y = 0; // keep the same height
        origin.position = target.position - offset;
        
        // rotate
        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        Vector3 cameraForward = Head.forward;
        cameraForward.y = 0;
        
        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);
        
        origin.RotateAround(target.position, Vector3.up, angle);
        
    }
    
}