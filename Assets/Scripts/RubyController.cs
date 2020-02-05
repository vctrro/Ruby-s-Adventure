using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D), typeof(Animator))]
public class RubyController : MonoBehaviour
{
    [Header("Максимальное здоровье")]
    [SerializeField]
    protected int maxHealth = 5;
    public float maxSpeed = 3f;
    public int Health { get; private set; }

    public GameObject projectilePrefab;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    bool buttonPressed;

    Rigidbody2D rb2d;
    Animator animator;
    Vector2 move, moveDirection;
    protected FloatingJoystick Joystick;
    protected JButton Button;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Health = maxHealth;
        Joystick = FindObjectOfType<FloatingJoystick>();
        Button = FindObjectOfType<JButton>();
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 24;
        print("Starting " + Time.time);
        StartCoroutine(WaitAndPrint(2.0F));
        print($"<color=green>Before WaitAndPrint Finishes {Time.time}</color>");
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        move.Set(horizontal, vertical);
        move += Joystick.Direction;
        move.Normalize();

        if(!Mathf.Approximately(move.x, 0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        rb2d.position += move * maxSpeed * Time.deltaTime;
        
        if (isInvincible) 
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            LaunchCog();
        }
        if (!buttonPressed && Button.Pressed) 
        {
            buttonPressed = true;
            LaunchCog();
        }
        buttonPressed = Button.Pressed;
    }

    IEnumerator WaitAndPrint(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Debug.Log($"<color=red>WaitAndPrint {Time.time}</color>");
    }

    public bool ChangeHealth(int amount) 
    {
        if (amount < 0)
        {
            if (isInvincible)
            return false;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            Health = Mathf.Clamp(Health + amount, 0, maxHealth);

            animator.SetTrigger("Hit");
            Debug.Log($"<color=red>{name} теряет {-amount} здоровья, осталось {Health}</color>");
            if (Health == 0) 
            {
                Destroy(gameObject);
                Debug.Log($"{name} умерла");
                //Application.Quit();
            }
            return true;
        }
        else 
        {
            if (Health == maxHealth)
            return false;

            Health = Mathf.Clamp(Health + amount, 0, maxHealth);
            Debug.Log($"<color=green>{name} восстанавливает {amount} здоровья, всего {Health}</color>");
            return true;
        }
    }

    void LaunchCog()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection);
        animator.SetTrigger("Launch");
    }
}