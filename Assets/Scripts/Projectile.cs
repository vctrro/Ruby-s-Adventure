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
    
    // Start is called before the first frame update
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    // Update is called once per frame
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

        EnemyController enemy = other.collider.GetComponent<EnemyController>();
        if (enemy != null) 
        {
            enemy.Fix();
            Destroy(gameObject);
        }
        rb2d.AddForce(rb2d.velocity * -100);
        //Debug.Log($"Снаряд столкнулся с {other.gameObject}");
        
    }
}
