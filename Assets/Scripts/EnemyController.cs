using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public float maxSpeed = 2.5f;
    private float fixTime = 2.5f;
    private bool isFixed, ifAtHome, obstacle;
    private Rigidbody2D rb2d;
    private Animator animator;
    private Vector2 startPosition, moveDirection, destination;
    [SerializeField] public ParticleSystem ps_Explosion;

    private void Start()
    {
        // ruby.OnBigBoom.AddListener(()=>{Fix();});
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        destination = startPosition = rb2d.position;
    }

    private void Update()
    {
        if (ifAtHome || isFixed) return;       // Если дома или заблокирован ничего не делает

        if (rb2d.position.Equals(startPosition)) ifAtHome = true; // Пришел домой
        MoveTo(destination);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Ruby")
        {
            ifAtHome = false;
            destination = other.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Ruby")
        {
            destination = startPosition;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"<color=blue>Collide with {other.gameObject.name}</color>");
        if (other.gameObject.name == "Ruby") return;

        //обходить препятствия
        if (other.collider.bounds.IntersectRay(new Ray(other.otherCollider.bounds.center, destination)))
        {
            //if (obstacle) return;
            obstacle = true;
            destination = other.collider.ClosestPoint(other.otherCollider.bounds.center);// + (Vector2) other.otherCollider.bounds.extents;
            //Detour(other.collider.bounds.center, other.collider.bounds.extents, other.otherCollider.bounds.extents);
            Debug.Log($"<color=white>Move {other.otherCollider.bounds.center} to {destination}</color>");
        }
        else
        {
            obstacle = false;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.name == "Ruby" && !isFixed)
        {
            other.collider.GetComponent<RubyController>().ChangeHealth(-1);
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if ((other.gameObject.tag == "Robot") || (other.gameObject.name == "Ruby")) return;
        // obstacle = false;
    }

    private void MoveTo(Vector2 direction)
    {
        moveDirection = direction - rb2d.position;
        moveDirection.Normalize();
        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);
        // rb2d.position += moveDirection * maxSpeed * Time.deltaTime;
        rb2d.position = Vector2.MoveTowards(rb2d.position, direction, Time.deltaTime * maxSpeed);
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
