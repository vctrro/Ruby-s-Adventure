using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public float maxSpeed = 2.5f;
    private bool isFixed, persecution, obstacle;
    private float fixTime = 2.5f;
    private Rigidbody2D rb2d;
    private Animator animator;
    private Vector2 startPosition, moveDirection, destination;
    public ParticleSystem ps_Explosion;

    private void Start()
    {
        // ruby.OnBigBoom.AddListener(()=>{Fix();});
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = rb2d.position;
    }

    private void Update()
    {
        if (isFixed) return;       //Если заблокирован

        if (!persecution && !obstacle)       //Если не преследует игрока и не обходит препятствие - идёт домой
        {
            if ((Vector2.Distance(startPosition, rb2d.position) > 0.2f)) //!rb2d.Equals(startPosition))
            {
                destination = startPosition;
            }
            else
            {
                destination = rb2d.position;
            }
        }

        MoveTo(destination);
    }

    private void LateUpdate() 
    {
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Ruby")
        {
            destination = other.transform.position;
            persecution = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Ruby")
            persecution = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"<color=blue>Collide with {other.gameObject.name}</color>");

        foreach (ContactPoint2D contact in other.contacts)
            {
                // Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
                Debug.Log($"Contact point{contact.point}");
            }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (isFixed) return;
        if (other.gameObject.tag == "Robot") return;

        if (other.gameObject.name == "Ruby")
        {
            other.collider.GetComponent<RubyController>().ChangeHealth(-1);
        }
        else 
        {
            //обходить препятствия
            if (obstacle) return;
            obstacle = true;
            Detour(other.collider.bounds.center, other.collider.bounds.extents, other.otherCollider.bounds.extents);
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if ((other.gameObject.tag == "Robot") || (other.gameObject.name == "Ruby")) return;
        obstacle = false;
    }

    private void MoveTo(Vector2 direction)
    {
        moveDirection = direction - rb2d.position;
        moveDirection.Normalize();
        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);
        rb2d.position += moveDirection * maxSpeed * Time.deltaTime;
    }

    public void Fix()
    {
        ps_Explosion.Play();
        StartCoroutine(Fixed());
    }

    private IEnumerator Fixed()
    {
        isFixed = true;
        animator.SetBool("isFixed", true);
        yield return new WaitForSeconds(fixTime);
        isFixed = false;
        animator.SetBool("isFixed", false);
    }

    private void Detour(Vector2 center, Vector2 size, Vector2 robotSize)
    {
        Vector2 detour;

        if (rb2d.position.x >= center.x)
        {
            // Debug.Log($"<color=green>X IF {rb2d.position.x} >= {center.x}</color>");
            detour.x = center.x + size.x + robotSize.x +0.1f;
        }
        else
        {
            // Debug.Log($"<color=red>X ELSE {rb2d.position.x} <= {center.x}</color>");
            detour.x = center.x - size.x - robotSize.x -0.1f;
        }
        if (rb2d.position.y >= center.y)
        {
            // Debug.Log($"<color=green>Y IF {rb2d.position.y} >= {center.y}</color>");
            detour.y = center.y + size.y + robotSize.y +0.1f;
        }
        else
        {
            // Debug.Log($"<color=red>Y ELSE {rb2d.position.y} <= {center.y}</color>");
            detour.y = center.y - size.y - robotSize.y -0.1f;
        }
        destination = detour;
        Debug.Log($"<color=white>Move {rb2d.position} to {detour}</color>");
        // MoveTo(detour);
    }
}
