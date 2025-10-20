using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private bool gameStarted = false;
    public GameObject startButton;
    public GameObject menuTextObject;

    // Rigidbody of the player.
    private Rigidbody rb; 

    private int count;
    private int goalColletables = 10;

    // Movement along X and Y axes.
    private float movementX;
    private float movementY;

    // Speed at which the player moves.
    public float speed = 10f; 

    // Jump settings
    public float jumpForce = 5f;
    private bool isGrounded;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private bool isPoweredUp = false;
    public GameObject powerCapsule;
    public Material poweredUpPlayerMat;
    public GameObject poweredUpTextObject;
    public GameObject specialWall;
    private bool specialWallUsed = false;
    private bool powerCapsuleSpawned = false;

    // Start is called before the first frame update.
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        count = 0; 
        SetCountText();

        menuTextObject.SetActive(true);
        winTextObject.SetActive(false);
        powerCapsule.SetActive(false);
        poweredUpTextObject.SetActive(false);

        // Freeze player before game starts
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;   // stops movement from AddForce
    }

    public void StartGame()
    {
        gameStarted = true;

        rb.useGravity = true;
        rb.isKinematic = false;
        
        startButton.SetActive(false);
        menuTextObject.SetActive(false);
    }

 
    // Movement input
    void OnMove(InputValue movementValue)
    {
        if (!gameStarted) return;
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

    // Jump input
    void OnJump(InputValue jumpValue)
    {
        if (!gameStarted) return;
        if (isGrounded && jumpValue.isPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void FixedUpdate() 
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed); 
    }

    // Detect pickups
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("PickItUp")) 
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
        if (other.gameObject.CompareTag("Finish")) 
        {
            other.gameObject.SetActive(false);
            WinGame();
        }
        if (other.gameObject.CompareTag("PowerCap")) 
        {
            other.gameObject.SetActive(false);
            SetPowerUp();
        }
    }

    // Detect ground collisions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;

        if (isPoweredUp && !specialWallUsed && collision.collider.CompareTag("SpecialWall"))
        {
            specialWallUsed = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 knockbackDir = collision.contacts[0].normal;
            rb.AddForce(knockbackDir * 2f, ForceMode.Impulse);

            collision.collider.gameObject.SetActive(false);
        }
    }



    void SetCountText() 
    {
        countText.text =  "Count: " + count.ToString();
        
        if (!powerCapsuleSpawned && count >= goalColletables)
        {
            powerCapsule.SetActive(true);
            powerCapsuleSpawned = true; // or destory power capsule
        }
    }

    void WinGame() 
    {
        // poweredUpTextObject.SetActive(false);
        winTextObject.SetActive(true);
        gameStarted = false;       
        rb.linearVelocity = Vector3.zero; 
        rb.isKinematic = true;      
    }

    void SetPowerUp()
    {
        poweredUpTextObject.SetActive(true);

        Renderer playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            playerRenderer.material = poweredUpPlayerMat;
        }

        if (specialWall != null)
        {
            Collider wallCollider = specialWall.GetComponent<Collider>();
            if (wallCollider != null)
            {
                wallCollider.enabled = true; 
            }
        }

        speed = 20f; 
        isPoweredUp = true;   
    }

}
