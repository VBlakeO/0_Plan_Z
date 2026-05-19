using UnityEngine;

public class InterectionSystem : MonoBehaviour
{
    public static InterectionSystem Instance = null;

    [SerializeField] private float range = 3f;
    [SerializeField] private Transform head = null;
    [SerializeField] private LayerMask attainableLayers;

    [SerializeField] private InteractiveObject tempObject = null;
    [SerializeField] private HudManager hudManager = null;


    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.position, head.forward, out hit, range, attainableLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hit.transform.GetComponent<InteractiveObject>())
            {
                hudManager.SetInteractionBaseState(false);
                return;
            }

            if (!hit.transform.GetComponent<InteractiveObject>().canInteract)
            {
                hudManager.SetInteractionBaseState(false);
                return;
            }

            hudManager.SetInteractionBaseState(true);

            tempObject = hit.transform.GetComponentInChildren<InteractiveObject>();
            if (hudManager && tempObject)
            {
                hudManager.SetInterectionBarProgress(tempObject.GetProgress());
                hudManager.SetInteractionText(tempObject.GetName() + "...");
            }

            if (Input.GetKey(KeyCode.E))
            {
                tempObject = hit.transform.GetComponentInChildren<InteractiveObject>();
             
                if (!tempObject)
                    return;

                tempObject.OnFinishInteractionAction += ClearTempObject;
                tempObject.Interacting();
            }
            else
            {
                if (!tempObject)
                    return;

                tempObject.OnFinishInteractionAction -= ClearTempObject;

                tempObject.StopInteraction();
                hudManager.SetInterectionBarProgress(tempObject.GetProgress());
            }

        }
        else
        {
            hudManager.SetInteractionText("");
            hudManager.SetInterectionBarProgress(0f);

            hudManager.SetInteractionBaseState(false);
            ClearTempObject();
        }
    }
    private void ClearTempObject()
    {
        if (!tempObject)
            return;

        tempObject.OnFinishInteractionAction -= ClearTempObject;
        hudManager.SetInterectionBarProgress(0f);
        hudManager.SetInteractionBaseState(false);
        tempObject.StopInteraction();

        tempObject = null;
    }
}
