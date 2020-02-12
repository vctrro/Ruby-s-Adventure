using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public float maxSpeed = 2.5f;
    private bool isFixed, persecution, obstacle;
    private float fixTime;
    private Rigidbody2D rb2d;
    private Animator animator;
    private Vector2 startPosition, moveDirection;
    public ParticleSystem PS;

    private void Start()
    {
        // ruby.OnBigBoom.AddListener(()=>{Fix();});
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = rb2d.position;
    }

    private void Update()
    {
        if (isFixed)        //Если заблокирован
        {
            fixTime -= Time.deltaTime;
            if (fixTime > 0) return;

            isFixed = false;
            animator.SetBool("isFixed", false);
        }

        if (!persecution && !obstacle)       //Если не преследует игрока идёт домой, если препятствие обходит
        {
            if ((Vector2.Distance(startPosition, rb2d.position) > 0.2f)) //!rb2d.Equals(startPosition))
            {
                MoveTo(startPosition);
            }
            else
            {
                moveDirection.Set(0, 0);
                animator.SetFloat("Move X", moveDirection.x);
                animator.SetFloat("Move Y", moveDirection.y);
            }
        }
    }

    private void MoveTo(Vector2 direction)
    {
        if (isFixed) return;
        moveDirection = direction - rb2d.position;
        moveDirection.Normalize();
        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);
        rb2d.position += moveDirection * maxSpeed * Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
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
            Debug.Log($"<color=red>Collide with {other.gameObject.name}</color>");
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if ((other.gameObject.tag == "Robot") || (other.gameObject.name == "Ruby")) return;
        obstacle = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Ruby")
        {
            MoveTo(other.transform.position);
            persecution = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Ruby")
            persecution = false;
    }

    public void Fix()
    {
        isFixed = true;
        PS.Play();
        animator.SetBool("isFixed", true);
        fixTime = 2.5f;
        //rb2d.simulated = false;
    }

    private void Detour(Vector2 center, Vector2 size, Vector2 robotSize)
    {
        Vector2 detour;

        if (rb2d.position.x >= center.x)
        {
            Debug.Log($"<color=green>X IF {rb2d.position.x} >= {center.x}</color>");
            detour.x = center.x + size.x + robotSize.x +0.1f;
        }
        else
        {
            Debug.Log($"<color=red>X ELSE {rb2d.position.x} <= {center.x}</color>");
            detour.x = center.x - size.x - robotSize.x -0.1f;
        }
        if (rb2d.position.y >= center.y)
        {
            Debug.Log($"<color=green>Y IF {rb2d.position.y} >= {center.y}</color>");
            detour.y = center.y + size.y + robotSize.y +0.1f;
        }
        else
        {
            Debug.Log($"<color=red>Y ELSE {rb2d.position.y} <= {center.y}</color>");
            detour.y = center.y - size.y - robotSize.y -0.1f;
        }
        Debug.Log($"<color=white>Move {rb2d.position} to {detour}</color>");
        MoveTo(detour);
    }
}
