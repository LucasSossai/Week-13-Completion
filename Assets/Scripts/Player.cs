using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float horizontalScreenSize = 11.5f;
    private float verticalScreenSize = 7.5f;
    private float speed;
    private int lives;
    private int shooting;
    private bool hasShield;

    public GameManager gameManager;
    public GameObject bullet;
    public GameObject explosion;
    public GameObject thruster;
    public GameObject coin;

    public GameObject shieldEffect;  // Reference to the shield bubble GameObject

    // Start is called before the first frame update
    void Start()
    {
        speed = 6f;
        lives = 3;
        shooting = 1;
        hasShield = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        FindObjectOfType<GameManager>().SetLives(lives);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Shooting();
    }

    void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * Time.deltaTime * speed);
        if (transform.position.x > horizontalScreenSize || transform.position.x <= -horizontalScreenSize)
        {
            transform.position = new Vector3(transform.position.x * -1, transform.position.y, 0);
        }
        if (transform.position.y > verticalScreenSize || transform.position.y < -verticalScreenSize)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y * -1, 0);
        }
    }

    void Shooting()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (shooting)
            {
                case 1:
                    Instantiate(bullet, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                    break;
                case 2:
                    Instantiate(bullet, transform.position + new Vector3(-0.5f, 1, 0), Quaternion.identity);
                    Instantiate(bullet, transform.position + new Vector3(0.5f, 1, 0), Quaternion.identity);
                    break;
                case 3:
                    Instantiate(bullet, transform.position + new Vector3(-0.5f, 1, 0), Quaternion.Euler(0, 0, 30f));
                    Instantiate(bullet, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                    Instantiate(bullet, transform.position + new Vector3(0.5f, 1, 0), Quaternion.Euler(0, 0, -30f));
                    break;
            }
        }
    }

    public void SetLives(int Lives)
    {
        FindObjectOfType<GameManager>().livesText.text = "Lives: " + lives;
    }

    public void LoseALife()
    {
        if (hasShield)
        {
            // Lose the shield
            hasShield = false;
            gameManager.PlayPowerDown();  // Play the powerdown sound when the shield is lost
            if (shieldEffect != null)
            {
                shieldEffect.SetActive(false); // Deactivate the shield bubble
            }
        }
        else
        {
            // No shield, lose a life
            lives--;
        }

        FindObjectOfType<GameManager>().SetLives(lives);

        if (lives == 0)
        {
            gameManager.GameOver();
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    IEnumerator SpeedPowerDown()
    {
        yield return new WaitForSeconds(3f);
        speed = 6f;
        thruster.gameObject.SetActive(false);
        gameManager.UpdatePowerupText("");
        gameManager.PlayPowerDown();  // Play powerdown sound when speed powerup ends
    }

    IEnumerator ShootingPowerDown()
    {
        yield return new WaitForSeconds(3f);
        shooting = 1;
        gameManager.UpdatePowerupText("");
        gameManager.PlayPowerDown();  // Play powerdown sound when shooting powerup ends
    }

    private void OnTriggerEnter2D(Collider2D whatIHit)
    {
        if (whatIHit.tag == "Powerup")
        {
            gameManager.PlayPowerUp();  // Play powerup sound when a powerup is picked up
            int powerupType = Random.Range(1, 5); // This can be 1, 2, 3, or 4
            switch (powerupType)
            {
                case 1:
                    // Speed powerup
                    speed = 9f;
                    gameManager.UpdatePowerupText("Picked up Speed!");
                    thruster.gameObject.SetActive(true);
                    StartCoroutine(SpeedPowerDown());
                    break;
                case 2:
                    // Double shot
                    shooting = 2;
                    gameManager.UpdatePowerupText("Picked up Double Shot!");
                    StartCoroutine(ShootingPowerDown());
                    break;
                case 3:
                    // Triple shot
                    shooting = 3;
                    gameManager.UpdatePowerupText("Picked up Triple Shot!");
                    StartCoroutine(ShootingPowerDown());
                    break;
                case 4:
                    // Shield powerup
                    gameManager.UpdatePowerupText("Picked up Shield!");
                    hasShield = true;
                    if (shieldEffect != null)
                    {
                        shieldEffect.SetActive(true); // Activate the shield bubble
                    }
                    break;
            }
            Destroy(whatIHit.gameObject);
        }
    }
}