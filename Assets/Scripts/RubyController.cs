﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]      //Требует наличия двух компонентов, при отсутствии создает их
public class RubyController : MonoBehaviour
{
    [Header("Максимальное здоровье")]
    [Range(5, 10)]
    [SerializeField] int maxHealth = 5;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] public GameObject projectilePrefab;
    [Header("Индикатор здоровья")]
    [SerializeField] public TMP_Text _text;
    [Header("События при изменении здоровья")]
    [SerializeField] public OnHealthEvent OnHealthChange;      //Событие при изменении здоровья
    [SerializeField] public UnityEvent OnBigBoom;

    int _health; // подумать
    int Health { get { return _health; } set { _health = value; OnHealthChange.Invoke(_health); } }
    float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    bool buttonPressed;

    Rigidbody2D rb2d;
    Animator animator;
    Vector2 move, moveDirection;
    FloatingJoystick moveJoystick;
    JButton Button;

    private void Awake() {
        Health = maxHealth;
    }
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Button = FindObjectOfType<JButton>();
        moveJoystick = FindObjectOfType<FloatingJoystick>();
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 24;
        print("Starting " + Time.time);
        StartCoroutine(WaitAndPrint(2.0F));
        print($"<color=green>Before WaitAndPrint Finishes {Time.time}</color>");
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        move.Set(horizontal, vertical);
        move += moveJoystick.Direction;
        move.Normalize();

        if (!Mathf.Approximately(move.x, 0f) || !Mathf.Approximately(move.y, 0.0f))
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

        if (Input.GetKeyDown(KeyCode.B))        //для теста 
        {
            OnBigBoom.Invoke();
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

    IEnumerator WaitAndPrint(float waitTime)
    {
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
            //Debug.Log($"<color=red>{name} теряет {-amount} здоровья, осталось {Health}</color>");
            if (Health == 0)
            {
                gameObject.SetActive(false);
                Debug.Log($"{name} умерла");
                //Application.Quit();       // --- Меню (!) ---
            }
            return true;
        }
        else
        {
            if (Health == maxHealth)
                return false;

            Health = Mathf.Clamp(Health + amount, 0, maxHealth);
            //Debug.Log($"<color=green>{name} восстанавливает {amount} здоровья, всего {Health}</color>");
            return true;
        }
    }
    public void SetHealth(int health)
    {
        _text.CrossFadeAlpha(0f, 0f, false);
        /* if (health < 2)
        {
            _text.color = Color.red;
        }
        else
        {
            _text.color = new Color(1f, 0.6f, 0f);
        } */
        _text.text = health.ToString();
        _text.CrossFadeAlpha(1f, 0.8f, false);
    }

    void LaunchCog()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection);
        animator.SetTrigger("Launch");
    }
}

[System.Serializable]
public class OnHealthEvent : UnityEvent<int>
{

}