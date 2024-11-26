using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class LeverInteractable : MonoBehaviour
{
    [SerializeField] private float angleThreshold = 45f;
    //TODO: change to nativ c# events
    [SerializeField] public UnityEvent onLeverActivated = new UnityEvent();
    [SerializeField] public UnityEvent onLeverDeactivated = new UnityEvent();
    
    private HingeJoint hingeJoint;
    private bool isActivated = false;
    private bool wasOverThreshold = false;

    public UnityEvent OnLeverActivated => onLeverActivated;
    public UnityEvent OnLeverDeactivated => onLeverDeactivated;

    private void Awake()
    {
        hingeJoint = GetComponent<HingeJoint>();
        if (hingeJoint == null)
        {
            Debug.LogError("HingeJoint component is required on the same GameObject!");
        }
    }

    private void Update()
    {
        // Variante 1: Verwendung des HingeJoint
        float leverAngle = hingeJoint.angle;
        bool isOverThreshold = Mathf.Abs(leverAngle) > angleThreshold;
        
        
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