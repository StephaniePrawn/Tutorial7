using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Provider;

public class PlayerControllerTask7 : MonoBehaviour

   
{
    [SerializeField] GameObject projectilePrefab;

    public int lives = 3;
    public int score = 0;

    Rigidbody2D physicsBody;
    [SerializeField] float Speed = 20;
    AudioSource speaker;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip deathSound;

    [SerializeField] GameObject reticle;
    [SerializeField] AudioClip scoreSound;
    [SerializeField] AudioClip shootSound;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float ProjectileSpeed = 500;
    Vector2 projectileDirection = new Vector2(1, 0);


    private void OnGUI()
    {
        Texture livesTexture = GetComponent<SpriteRenderer>().sprite.texture;
         int xStart = 70;
            int spacing = 40;
            int yPosition = 10;
            int pixels = 40;
            
        

        for (int livesDrawn = 0; livesDrawn < lives; livesDrawn++)
        {
           

            Rect drawLocation3 = new Rect(
                x: xStart + spacing * livesDrawn,
                y: yPosition,
                width: pixels,
                height: pixels);
            GUI.DrawTexture(drawLocation3, livesTexture);
            print(livesDrawn);
        }    

        Texture scoreSprite = projectilePrefab.GetComponent<SpriteRenderer>().sprite.texture;
        
        for (int scoreDrawn = 0; scoreDrawn < score; scoreDrawn++) 
        {
            int xPosition = xStart + scoreDrawn * spacing;
      
            int xSize = 20;
            int ySize = 20;
            Rect drawLocation = new Rect(xPosition, 50, xSize, ySize);
            GUI.DrawTexture(drawLocation, scoreSprite);
        }

    }

    void Start()
    {
        physicsBody = GetComponent<Rigidbody2D>();
        speaker = GetComponent<AudioSource>();

    }

    void Update()
    {
        HandleMovement();
        CheckWinAndLoseStates();
        if (physicsBody == null) return;
       
        HandleMovement();
        CheckWinAndLoseStates();
        HandleFiring();
       


    }

    private void HandleFiring()
    {
        if (physicsBody.linearVelocity != Vector2.zero)
        {
            projectileDirection = physicsBody.linearVelocity.normalized;
        }
        
        reticle.transform.localPosition = projectileDirection; 

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Vector2 spawnLocation = transform.position;
            Quaternion spawnRotation = projectilePrefab.transform.rotation;
            GameObject spawnedProjectile = Instantiate(projectilePrefab, spawnLocation, spawnRotation);
            spawnedProjectile.GetComponent<Rigidbody2D>().AddForce(projectileDirection * ProjectileSpeed);
            speaker.PlayOneShot(shootSound); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyControllerTask7>() != null)
        {
            lives--;
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        }
    }

    private void HandleMovement()
    {
        float xInput = Keyboard.current.dKey.value - Keyboard.current.aKey.value;
        float yInput = Keyboard.current.wKey.value - Keyboard.current.sKey.value;
        Vector2 force = new Vector2(xInput, yInput) * Speed;
        physicsBody.AddForce(force);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7, 7),
            Mathf.Clamp(transform.position.y, -4, 4), 0);
    }

    private void CheckWinAndLoseStates()
    {
        if (score >= 10)
        {
            GameObject enemyObject = GameObject.Find("Enemy");
            Destroy(enemyObject);
            speaker.PlayOneShot(winSound);
            Destroy(physicsBody);
        }
        if (lives <= 0)
        {
            speaker.PlayOneShot(deathSound);
            Destroy(physicsBody);
            GetComponent<SpriteRenderer>().color = Color.red;
            Invoke(nameof(RemoveCorpse), 1.5f);
        }
    }

    private void RemoveCorpse()
    {
        Destroy(gameObject);
    }

    public void AddPoints(int points)
    {
        score += points;
        speaker.PlayOneShot(scoreSound);

    }
}
