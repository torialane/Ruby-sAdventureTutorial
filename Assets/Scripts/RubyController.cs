using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public int cogs = 5;
    public Text cogstext;

    public GameObject projectilePrefab;

    public Text losstext;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public AudioSource musicSource;

    public float speeduptime = 10.0f;
    float timerdisplay;

    public AudioClip LossSound;
    public AudioClip ammo;

    public GameObject HealthEffectPrefab;
    public GameObject DamageEffectPrefab;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    private bool gameOver = (false);

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cogstext.text = cogs.ToString();
        currentHealth = maxHealth;
        timerdisplay = -1.0f;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            Vector2 move = new Vector2(horizontal, vertical);

            if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            animator.SetFloat("Look X", lookDirection.x);
            animator.SetFloat("Look Y", lookDirection.y);
            animator.SetFloat("Speed", move.magnitude);

            if (isInvincible)
            {
                invincibleTimer -= Time.deltaTime;
                if (invincibleTimer < 0)
                    isInvincible = false;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {

                if (cogs > 0)
                {
                    Launch();
                    cogs -= 1;
                    cogstext.text = cogs.ToString();
                }
            }

            if (Input.GetKeyDown("escape"))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
                if (hit.collider != null)
                {
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.R))

            {

                if (gameOver == true)

                {

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                }
            }
        }
        if (timerdisplay >= 0)
        {
            timerdisplay -= Time.deltaTime;
            if (timerdisplay < 0)
            {
                speed = 3;
            }
        }

    }
    void OnCollisionEnter2D(Collision2D other)
    {
    
        if (other.collider.tag == "CogPickup")
        {
            PlaySound(ammo);
            cogs = cogs + 3;
            Destroy(other.collider.gameObject);
            cogstext.text = cogs.ToString();
        }
        
        if (other.collider.tag == "Banan")
        {
            speed = speed + 2;
            Destroy(other.collider.gameObject);
            timerdisplay = speeduptime;
        }

    }



    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject projectileObject = Instantiate(DamageEffectPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);

        }

        if (amount > 0)
        {
            GameObject projectileObject = Instantiate(HealthEffectPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth <= 0)
        {
            losstext.enabled = true;
            gameOver = true;
            speed = 0;
            musicSource.Stop();
            musicSource.clip = LossSound;
            musicSource.loop = (false);
            musicSource.Play();
        }



        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}