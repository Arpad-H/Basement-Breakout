using UnityEngine;
using UnityEngine.Events;

public class LeverInteractable : MonoBehaviour
{
    [SerializeField] private float angleThreshold = 45f;
    [SerializeField] public UnityEvent onLeverActivated = new UnityEvent();
    [SerializeField] public UnityEvent onLeverDeactivated = new UnityEvent();

    private bool isActivated = false;
    private bool wasOverThreshold = false;
    private Quaternion initialRotation;
    private Transform handleTransform;

    public UnityEvent OnLeverActivated => onLeverActivated;
    public UnityEvent OnLeverDeactivated => onLeverDeactivated;

    private void Awake()
    {
        handleTransform = GetComponent<Transform>();
        if (handleTransform == null)
        {
            Debug.LogError("A Transform component is required on the same GameObject!");
        }
        initialRotation = handleTransform.localRotation;
    }

    private void Update()
    {
        // Calculate the difference in rotation from the initial rotation
        Quaternion currentRotation = handleTransform.localRotation;
        Quaternion deltaRotation = Quaternion.Inverse(initialRotation) * currentRotation;
        
        // Convert delta rotation to angle
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        
        bool isOverThreshold = Mathf.Abs(angle) > angleThreshold;
        Debug.Log($"[LeverInteractable]: Angle threashold caculation: + {isActivated} currentRotation = {currentRotation} isOverThreshold {isOverThreshold}");
        if (isOverThreshold != wasOverThreshold)
        {
            isActivated = isOverThreshold;

            if (isActivated)
            {
                 
                onLeverActivated.Invoke();
            }
            else
            {
                onLeverDeactivated.Invoke();
            }
        }

        wasOverThreshold = isOverThreshold;
    }
}