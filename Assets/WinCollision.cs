using UnityEngine;

public class WinCollision : MonoBehaviour
{
    [SerializeField] private GameObject player; // Spieler-Referenz
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("WinCollision: GameManager not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Überprüfen, ob das kollidierende Objekt das Tag "Door" hat
        if (other.CompareTag("door"))
        {
            Debug.Log("WinCollision: Triggered by an object with the tag 'Door'!");

            // GameState auf Win setzen
            if (gameManager != null)
            {
                gameManager.UpdateGameState(GameManager.GameState.Win);
                Debug.Log("WinCollision: GameState set to Win.");
            }
            else
            {
                Debug.LogError("WinCollision: GameManager is missing!");
            }

            // Spielerposition ändern
            if (player != null)
            {
                player.transform.position = new Vector3(50, 10, 0);
                Debug.Log("WinCollision: Player position set to (50, 10, 0).");
            }
            else
            {
                Debug.LogError("WinCollision: Player GameObject is not assigned!");
            }
            Debug.Log("WinCollision: Player has won the game!");
        }
    }
}