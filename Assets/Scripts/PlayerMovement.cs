using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    private float horizontal;
    private float speed = 8f;
    private bool isFacingRignt = true;

    public Rigidbody2D rb;


    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactionAction;
    private bool canInteract = false;
    private GameObject interactableObject;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"]; // Ensure "Move" is the name of your action
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
        horizontal = moveAction.ReadValue<Vector2>().x; // Get horizontal input
        Flip();

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Lvl01_St01");
        }

        if (interactionAction.WasPressedThisFrame())
        {
            Debug.Log("E key detected");
        }

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        if ((isFacingRignt && horizontal < 0) || (!isFacingRignt && horizontal > 0))
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
            Debug.Log("Press E to interact");
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
