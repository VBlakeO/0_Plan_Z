using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] private bool equiped = false;
    [SerializeField] private Camera mainCamera = null;
    [Space]

    [SerializeField] private Transform obj = null;
    [SerializeField] private LayerMask attainableLayers = 1;
    [Space]

    [SerializeField] private WeaponManager weaponManager = null;
    [SerializeField] private PlayerInventory inventory = null;
    [Space]

    [SerializeField] private Material matRed = null;
    [SerializeField] private Material matGreen = null;
    [Space]

    [SerializeField] private float range = 8f;
    [SerializeField] private float hight = 1f;
    [Space]

    [SerializeField] private GameObject[] prefabs;
    [Space]

    private MeshFilter mesh = null;
    private MeshRenderer render = null;
    private DetectCollision detectCollision = null;

    private int currentItem = 0;
    private GameObject prefab = null;

    public int rotationState = 0;

    void Awake()
    {
        if (obj)
        {
            mesh = obj.GetComponent<MeshFilter>();
            render = obj.GetComponent<MeshRenderer>();
            detectCollision = obj.GetComponent<DetectCollision>();
        }
    }

    public void EquipObject(bool equip)
    {
        equiped = equip;

        if (equip)
            weaponManager.LockAllWeapoms();
        else
            weaponManager.UnlockAllWeapoms();
    }

    public void SelectObject(int item)
    {
        if (inventory.equipment[item] > 0)
        {
            prefab = prefabs[item];
            currentItem = item;
            SetMesh();
            EquipObject(true);
        }
    }

    private void SetMesh()
    {
        mesh.mesh = prefab.GetComponent<MeshFilter>().sharedMesh;

        Collider collider = obj.GetComponent<Collider>();
        Collider newCollider = null;
        if (collider != null)
        {
            Destroy(collider); // Remove o componente
            newCollider = (Collider)obj.gameObject.AddComponent(collider.GetType()); // Adiciona um novo do mesmo tipo
        }

        obj.GetComponent<DetectCollision>().Exit();

        if (newCollider != null)
            newCollider.isTrigger = true;
    }

    void Update()
    {
        if (!equiped)
        {
            obj.gameObject.SetActive(false);
            return;
        }

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, range, attainableLayers, QueryTriggerInteraction.Ignore))
        {
            obj.gameObject.SetActive(true);
            obj.position = hit.point + Vector3.up * hight;

            if (Input.GetKeyDown(KeyCode.Mouse1))// && inventory.equipment[currentItem] > 0)
            {   
                switch (rotationState)
                {
                    case 0:
                        obj.transform.rotation = Quaternion.Euler(0, 45, 0);
                        break;

                    case 1:
                        obj.transform.rotation = Quaternion.Euler(0, 90, 0);
                        break;                 
                    
                    case 2:
                        obj.transform.rotation = Quaternion.Euler(0, 135, 0);
                        break;

                    case 3:
                        obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                        break;

                    default:
                        obj.transform.rotation = Quaternion.Euler(0, 45, 0);
                        break;
                }

                rotationState++;

                if (rotationState > 3)
                    rotationState = 0;
            }

            if (detectCollision.Colliding())
            {
                render.material = matRed;
            }
            else 
            {
                render.material = matGreen;

                if (Input.GetKeyDown(KeyCode.Mouse0) && inventory.equipment[currentItem] > 0)
                {
                    Instantiate(prefab, obj.transform.position, obj.transform.rotation);
                    inventory.RemoveItem(currentItem, 1);
                } 
                else if (inventory.equipment[currentItem] == 0)
                    render.material = matRed; 
            }
        }
        else
        {
            obj.gameObject.SetActive(false);
        }
    }
}