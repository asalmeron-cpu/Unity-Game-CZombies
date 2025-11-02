using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementFPS : MonoBehaviour
{
    // --- MOVIMIENTO ---
    public float speed = 5f;          // velocidad normal
    public float sprintSpeed = 8f;    // velocidad corriendo
    public float gravity = -20f;      // gravedad
    public float jumpHeight = 2f;     // altura salto
    public float normalSpeed = 5f;    // velocidad base

    // --- POSTURA ---
    public float standHeight = 2f;    // altura de pie
    public float crouchHeight = 1.2f; // altura agachado
    public float proneHeight = 0.6f;  // altura prone
    public float heightSmooth = 5f;   // suavizado altura
    public Transform playerCamera;    
    public float cameraStandY = 1.6f;
    public float cameraCrouchY = 1f;
    public float cameraProneY = 0.5f;

    // --- RATON ---
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool forcedProne = false; // si obligo a estar prone

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standHeight;
        Cursor.lockState = CursorLockMode.Locked; // bloqueo cursor
        Cursor.visible = false;
    }

    void Update()
    {
        Mover();   // movimiento
        Mirar();   // mirar con raton
        Postura(); // cambio de postura
    }

    void Mover()
    {
        bool isGrounded = controller.isGrounded; 
        if(isGrounded && velocity.y < 0) velocity.y = -2f; // pego al suelo

        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right*x + transform.forward*z; // direccion de movimiento

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;
        controller.Move(move * currentSpeed * Time.deltaTime); // muevo jugador

        float tol = 0.05f; // tolerancia altura para saltar
        if(Input.GetButtonDown("Jump") && isGrounded && Mathf.Abs(controller.height-standHeight)<tol)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // salto
        }

        velocity.y += gravity * Time.deltaTime; // aplico gravedad
        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    void Mirar()
    {
        float mouseX = Input.GetAxis("Mouse X")*mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y")*mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // limite de camara
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX); // giro jugador
    }

    void Postura()
    {
        float targetHeight = standHeight;
        float targetCamY = cameraStandY;

        if(forcedProne)
        {
            targetHeight = proneHeight;
            targetCamY = cameraProneY;
        }
        else if(Input.GetKey(KeyCode.LeftControl))
        {
            targetHeight = crouchHeight;
            targetCamY = cameraCrouchY;
        }
        else if(Input.GetKey(KeyCode.Z))
        {
            targetHeight = proneHeight;
            targetCamY = cameraProneY;
        }

        // suavizo cambio
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime*heightSmooth);

        // suavizo camara
        Vector3 camPos = playerCamera.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime*heightSmooth);
        playerCamera.localPosition = camPos;
    }

    public void ForceProneState()
    {
        forcedProne = true; // obligo a prone
    }

    public void ReleaseForcedState()
    {
        forcedProne = false; // quito forced
    }
}
