using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private float horizontal;
    [SerializeField] private float speed = 8f;
    private bool isFacingRignt = true;

    public Rigidbody2D rb;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private bool isMoving = false;


    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactionAction;
    private bool canInteract = false;
    private GameObject interactableObject;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        //moveAction = playerInput.actions["Move"]; // Ensure "Move" is the name of your action

        playerInput = GetComponent<PlayerInput>();
        interactionAction = playerInput.actions["Interact"];


    }

    private void OnEnable()
    {
        Debug.Log("Interact Action Enabled!");
        interactionAction.Enable();
        interactionAction.performed += OnInteract;

    }
    private void OnDisable()
    {
        interactionAction.performed -= OnInteract;
        interactionAction.Disable();
    }

    void Update()
    {
        //horizontal = moveAction.ReadValue<Vector2>().x; // Get horizontal input
        //point and click movement
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 screen = Mouse.current.position.ReadValue();
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(screen);

            //Does click overlap an NPC Collider?
            //Collider2D hit = Physics2D.OverlapPoint(clickPos);
            //RaycastHit2D hit = Physics2D.Raycast(clickPos, Camera.main.transform.forward, 100f);
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(screen));

            if (hit)
            {
                if (hit.transform.CompareTag("NPC"))
                {

                    Debug.Log("Clicked on NPC: " + hit.transform.name);
                    DialogueManager.instance.StartDialogue();
                    return; //dont start a walk this frame.
                }
                if (hit.transform.CompareTag("Ground"))
                {
                    targetPosition = hit.point;
                    isMoving = true;    
                }
            }
            //otherwise walk horizontally towards clicked x
            //targetPosition = new Vector2(clickPos.x, rb.position.y);
            //isMoving = true;
        }


        Flip();
    }

    private void FixedUpdate()
    {
        //rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        if (isMoving)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (Vector2.Distance(rb.position, targetPosition) < 0.05f)
            {
                isMoving = false;
            }
        }
    }

    private void Flip()
    {
        Vector2 direction = targetPosition - rb.position;
        if ((isFacingRignt && direction.x < 0) || (!isFacingRignt && direction.x > 0))
        {
            isFacingRignt = !isFacingRignt;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            canInteract = true;
            interactableObject = other.gameObject;
            Debug.Log("Press left mouse button to interact");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            canInteract = false;
            interactableObject = null;
            Debug.Log("Left interaction zone.");
        }
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("OnInteract function called!"); // Debug step 1
        if (canInteract && interactableObject != null)
        {

            Debug.Log("Interacting with" + interactableObject.name);
        }
        else
        {
            Debug.Log("Cannot interact right now!"); // Debug step 3
        }
    }



}
