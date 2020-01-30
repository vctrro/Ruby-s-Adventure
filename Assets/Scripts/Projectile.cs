using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb2d;
    Vector2 startPosition;
    public int cogForce = 300;
    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
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
