using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDie
{
    public Rigidbody rb;

    public Vector3 movementDirection;

    public float movementSpeed;
    public float speedMultiplier;
    public float drag;
    //public float jumpSpeed;
    //public float jumpSpeedMultiplier;

    public new Camera camera;
    public Vector3 mouseWorldPosition;
    public Vector3 mousePosition;

    public bool pickUp;
    public float MaxPickupDistance;
    public GameObject holdPosition;
    public GameObject heldObject;
    public LayerMask layer;

    public LayerMask InteractLayer;
    public GameObject currentInteract;
    public FlowerObject flower;

    public KeyCode[] keys;
    public ActionType[] actions;
    [SerializeField]
    private int CurrentAction;

    private bool isGrounded;
    private bool isHolding;

    [Space(12)]
    [Header("Third Person")]
    public bool thirdPerson;
    public float mouseSensitivity;
    public bool invertMouse;
    public Transform thirdPersonPos;
    public float RotationsOffset;
    public float RotationPointYOffset;


    // Start is called before the first frame update
    void Awake()
    {
        camera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }
    private bool resetMouse;
    // Update is called once per frame
    void Update()
    {
        movementDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        //pickUp = Input.GetMouseButton(1);

        // moving
        if (movementDirection.magnitude > 0)
        {
            rb.drag = 0;
        }
        else
        {
            rb.drag = drag;
        }

        if (rb.velocity.magnitude < movementSpeed * speedMultiplier)
            rb.velocity = movementSpeed * speedMultiplier * movementDirection;

        // Rotation
        if (Input.GetMouseButton(1))
        {
            resetMouse = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y") * ((invertMouse) ? -1 : 1);

            // x rotation
            transform.Rotate(new Vector3(0f, mouseX, 0f), Space.World);

            // y rotation
            Quaternion oldRotation = thirdPersonPos.rotation;
            Vector3 oldPos = thirdPersonPos.position;
            thirdPersonPos.RotateAround(holdPosition.transform.position - ((holdPosition.transform.position - transform.position) * RotationsOffset) + new Vector3(0, RotationPointYOffset, 0f), thirdPersonPos.transform.right, mouseY);
            if (thirdPersonPos.forward.y < -0.9f)
            {
                thirdPersonPos.SetPositionAndRotation(oldPos, oldRotation);
            }
        }
        else
        {
            if (resetMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        /*mousePosition = Input.mousePosition;
        mousePosition.z += 1;
        mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);

        transform.LookAt(Common.PythagHyp(camera.transform.position, mouseWorldPosition, transform.position.y), Vector3.up);*/



        // equiped action
        for (int i = 0; i < keys.Length && i < actions.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                CurrentAction = i;
            }
        }

        /*rb.MovePosition(transform.position + 
            (movementSpeed * speedMultiplier * Time.deltaTime * movementDirection)
            );*/

        // interaction
        /* TODO
         * - verify that the object can do that action
         */

        if (Input.GetMouseButtonDown(0))
        {
            if (currentInteract != null)
            {
                currentInteract.GetComponent<IInteract>().Interact(actions[CurrentAction], flower);
            }
        }
    }



    private void FixedUpdate()
    {

        // pickup
        RaycastHit hit;
        if (pickUp)
        {
            if (!isHolding)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, MaxPickupDistance, layer))
                {
                    GameObject gameObject = hit.collider.gameObject;

                    SetHoldingObject(gameObject);
                }


            }
        }
        else if (isHolding)
        {
            DropObject();
        }

        // interaction
        //Ray interactObjectRay = new(camera.transform.position, mouseWorldPosition);
        if (Physics.Raycast(camera.transform.position, (mouseWorldPosition - camera.transform.position), out hit, 50f, InteractLayer))
        {
            if (hit.transform.gameObject != currentInteract)
            {
                //Debug.Log("ItemObject found");
                if (currentInteract != null)
                {
                    //Destroy(currentInteract.GetComponent<Outline>());
                }
                currentInteract = hit.transform.gameObject;
                //currentInteract.AddComponent<Outline>();
            }
        }
        else if (currentInteract != null)
        {
            //Destroy(currentInteract.GetComponent<Outline>());
            currentInteract = null;
        }

    }

    // sets the object being carried by player
    private void SetHoldingObject(GameObject gameObject)
    {
        // set held object
        heldObject = gameObject;
        // set velocity to 0
        heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // disabling gravity
        heldObject.GetComponent<Rigidbody>().useGravity = false;
        // disable coliders
        heldObject.GetComponent<Collider>().enabled = false;
        //Add follow scriptand set target
        if (heldObject.TryGetComponent<FollowTarget>(out FollowTarget component))
            component.enabled = true;
        else
            heldObject.AddComponent<FollowTarget>().SetTarget(holdPosition.transform);

        isHolding = true;
    }

    public void DropObject()
    {
        // enable collider
        heldObject.GetComponent<Collider>().enabled = true;
        // disabling gravity
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        // terminate follow controls
        heldObject.GetComponent<FollowTarget>().enabled = false;
        // 
        heldObject = null;
        isHolding = false;
    }

    public void Die()
    {
        Debug.Log("Player dead");
    }
}
