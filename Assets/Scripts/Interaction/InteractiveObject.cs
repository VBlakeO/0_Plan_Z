using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private float interactionTime = 1f;
    [Tooltip("Tempo até poder fazer uma nova interação")] [SerializeField] private float newInteractionDelay = 1f; 
    [SerializeField] private float finishInteractionEventDelay = 0f; 
    [Space]

    [SerializeField] private string interectionName = "Interagir"; 
    [Space]


    [Tooltip("Pode interagir")] 
    public bool canInteract = true; 

    [Tooltip("A interação é instantania")] 
    [SerializeField] private bool quickInteraction = true;

    [Tooltip("Se a intereção for unica ela não acontecera de novo")]
    [SerializeField] private bool uniqueInteraction = false; 

    [Tooltip("Se a intereção for contunua o jogador pode parar de interagir e voltar depois sem perder o progresso")]
    [SerializeField] private bool continuousInteraction = false; 
    [Space]

    public UnityEvent OnFinishInteractionEvent = null;
    public UnityAction OnFinishInteractionAction = null;

    public UnityEvent OnFinishInteractionEventWithDelay = null;
    public UnityAction OnFinishInteractionActionWithDelay = null;

    public UnityEvent OnStopInteractionEvent = null;
    public UnityAction OnStopInteractionAction = null;

    public UnityEvent OnInteractingEvent = null;
    public UnityAction OnInteractingAction = null;



    [SerializeField] private float currentInteractionTime = 0f;
    [Space]

    private bool exhaustedInteractions = false;
    private bool interactionInterval = false;
    private bool interacted = false;

    public float GetInterectionTime()
    {
        return currentInteractionTime;
    }

    public float GetProgress()
    {
        return currentInteractionTime / interactionTime;
    }

    public string GetName()
    {
        return interectionName;
    }

    public void Interacting()
    {
        if (!canInteract || exhaustedInteractions || interactionInterval)
            return;

        OnInteractingEvent?.Invoke();
        OnInteractingAction?.Invoke();

        if (quickInteraction)
        {
            if (!interacted)
            {
                interacted = true;
                OnInteract();
            }
        }
        else
        {
            currentInteractionTime = GetProgress() < 1 ? currentInteractionTime += Time.deltaTime : currentInteractionTime = 1;

            if (GetProgress() >= 1 && !interacted)
            {
                interacted = true;

                OnInteract();
                currentInteractionTime = 0;
            }
        }
    }

    public void StopInteraction()
    {
        if (currentInteractionTime == 0)
            return;

        OnStopInteractionEvent?.Invoke();
        OnStopInteractionAction?.Invoke();

        if (!continuousInteraction)
            currentInteractionTime = 0f;
    }

    protected virtual void OnInteract()
    {
        OnFinishInteractionEvent?.Invoke();
        OnFinishInteractionAction?.Invoke();

        interacted = false;
        interactionInterval = true;
        currentInteractionTime = 0f;

        if (finishInteractionEventDelay > 0)
            Invoke(nameof(OnInteractDelay), finishInteractionEventDelay);

        if (newInteractionDelay > 0)
            Invoke(nameof(InteractionCooldown), newInteractionDelay);
        else
            InteractionCooldown();

        if (uniqueInteraction)
            exhaustedInteractions = true;
    }

    private void OnInteractDelay()
    {
        OnFinishInteractionEventWithDelay?.Invoke();
        OnFinishInteractionActionWithDelay?.Invoke();
    }

    public void ResetInteraction()
    {
        exhaustedInteractions = false;
    }

    public void SetCanInteract(bool state)
    {
        canInteract = state;
    }

    private void InteractionCooldown()
    {
        interactionInterval = false;
    }
}
