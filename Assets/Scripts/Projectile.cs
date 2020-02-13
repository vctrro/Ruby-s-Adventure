using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Сила снаряда")]
    [SerializeField]
    private int cogForce = 300;
    private Rigidbody2D rb2d;
    private Vector2 startPosition;
    
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Vector2.Distance(startPosition, transform.position) > 8.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction)
    {
        rb2d.AddForce(direction * cogForce);
    }

    private void OnCollisionEnter2D(Collision2D other) {

        rb2d.drag = 0.6f;       
        rb2d.AddForce(other.relativeVelocity * 10);
        Debug.Log($"Velocity {rb2d.velocity}");
        Debug.Log($"RelativeVelocity {other.relativeVelocity}");
        EnemyController enemy = other.collider.GetComponent<EnemyController>();
        if (enemy != null) 
        {
            enemy.Fix();
            Destroy(gameObject);
        }
        //Debug.Log($"Снаряд столкнулся с {other.gameObject}");
        
    }
}
