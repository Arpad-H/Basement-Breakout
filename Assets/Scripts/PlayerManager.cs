using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using UnityEngine.XR;


//TODO raum drrhen ausprobieren

public class PlayerManager : MonoBehaviour
{
   
    private Vector3 STARTSCENEPOS = new Vector3(-1.6f, 1f, 2f);
    private Vector3 STARTMENUPOS = new Vector3(50f, 10f, 0f);
    private Vector3 GAMEOVERMENUPOS = new Vector3(54f, 10f, 1f);

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

    public static event Action<GameManager.GameState> GameStateChangedPlayer;

    
    private AudioSource _audioSource;
    private AudioClip _introducingTVClip;
    private OVRManager _ovrManager;


    private void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void Start()
    {
        //DeactivateTeleportInteractor();
        ActivateTeleportInteractor();
        DeactivateRayInteractor();
        _audioSource = GetComponent<AudioSource>();
        _ovrManager = GetComponent<OVRManager>();
        if (startAtMenu && startAtMenu)
        {
            //setPlayerPosToStartMenu();
        }else if (!startAtMenu)
        {
            loadGamePlayScene();
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

        Debug.LogWarning($" [PlayerManager] currentAngle Y: {cameraRig.centerEyeAnchor.rotation.eulerAngles.y} //  Frame: {_counter}");
        _counter++;
    }

    public void SetPlayerPositionToStartGame()
    {
        
        transform.position = STARTSCENEPOS;
        RotateEnvironment(SceneRoom);
        CenterEnvironment(STARTSCENEPOS, SceneRoom);
    }

    public void loadGamePlayScene()
    {
        // //ActivateTeleportInteractor();
        // DeactivateTeleportInteractor();
        // //SetPlayerPositionToStartGame();
        // DeactivateRayInteractor();
        // //DeactivatePassthrough();
        _audioSource.Play();
        GameStateChangedPlayer?.Invoke(GameManager.GameState.Tutorial);
    }


    public void setPlayerPosToStartMenu()
    {
        CenterEnvironment(STARTMENUPOS, GameMenuRoom);
        RotateEnvironment(GameMenuRoom);
        transform.position = STARTMENUPOS;
        InputTracking.Recenter();
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
        _ovrManager.isInsightPassthroughEnabled = true;
    }

    private void DeactivatePassthrough()
    {
        _ovrManager.isInsightPassthroughEnabled = false;
    }

    public void SetPlayerPosToGameOverMenu()
    {
        CenterEnvironment(GAMEOVERMENUPOS, GameMenuRoom);
        RotateEnvironment(GameMenuRoom);
        transform.position = GAMEOVERMENUPOS;
        
    }

    public bool Drawing()
    {
        if (PlayerHead.transform.position.y < WaterheightPlane.transform.position.y)
        {
            _timeUnderWater += Time.deltaTime;
        }
        else
        {
            _timeUnderWater = 0;
        }

        return _timeUnderWater > DROWNINGTIME;
    }

    private void HandleGameStateChanged(GameManager.GameState gameState)
    {
        Debug.Log($"[PlayerManager]: GameState changed to {gameState}");
        if (gameState is GameManager.GameState.Drowned or GameManager.GameState.Win
            or GameManager.GameState.ElectricShock)
        {
            //AcivatePassthrough();
            DeactivateTeleportInteractor();
            ActivateRayInteractor();
            SetPlayerPosToGameOverMenu();
        }
        else if (gameState == GameManager.GameState.Tutorial)
        {
            //DeactivatePassthrough();
            DeactivateRayInteractor();
            SetPlayerPositionToStartGame();
        }
        else if (gameState == GameManager.GameState.Game)
        {
            ActivateTeleportInteractor();
        }
    }

    
    
    private void RotateEnvironment(Transform environmentParent)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
        float currentYRotation = cameraRig.centerEyeAnchor.rotation.eulerAngles.y;
        Quaternion currentRotation = Quaternion.Euler(0, currentYRotation, 0);
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(currentRotation);
        environmentParent.rotation = deltaRotation;
        Debug.LogWarning($"[PlayerManager]: Rotating environment by {environmentParent.rotation.eulerAngles} // DelatRotation: {deltaRotation.eulerAngles} // CurrentRotation: {currentRotation.eulerAngles}");
    }
    
    public void CenterEnvironment(Vector3 targetPosition, Transform environmentParent)
    {
        //environmentParent.position += targetPosition - transform.position;
    }
    
    
    
}