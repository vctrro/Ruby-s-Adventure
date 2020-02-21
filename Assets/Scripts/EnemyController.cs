using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] public float maxSpeed = 2.5f;
    private float fixTime = 2.5f;
    private bool isFixed, ifAtHome = true, left;
    private Rigidbody2D rb2d;
    private Animator animator;
    private Vector2 startPosition, moveDirection, destination, detour;
    [SerializeField] public ParticleSystem ps_Explosion;

    private void Start()
    {
        // ruby.OnBigBoom.AddListener(()=>{Fix();});
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        transform.Find("BotTrigger").GetComponent<EnemyTrigger>().OnFindRuby.AddListener(FindRuby);
        transform.Find("BotTrigger").GetComponent<EnemyTrigger>().OnLostRuby.AddListener(LostRuby);
        destination = startPosition = rb2d.position;
    }

    private void Update()
    {
        if (ifAtHome || isFixed) return;       // Если дома или заблокирован ничего не делает

        if (rb2d.position.Equals(startPosition)) ifAtHome = true; // Пришел домой
        
        MoveTo();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.name == "Ruby" && !isFixed)
        {
            other.collider.GetComponent<RubyController>().ChangeHealth(-1);
        }
    }

    private void FindRuby(Vector2 other)
    {
        ifAtHome = false;
        destination = other;
    }

    private void LostRuby()
    {
        destination = startPosition;
    }

    private void MoveTo()
    {
        destination = LookAround(destination);
        moveDirection = destination - rb2d.position;
        // moveDirection.Normalize();
        animator.SetFloat("Move X", moveDirection.x);
        animator.SetFloat("Move Y", moveDirection.y);
        // rb2d.position += moveDirection * maxSpeed * Time.deltaTime;
        rb2d.position = Vector2.MoveTowards(rb2d.position, destination, Time.deltaTime * maxSpeed);
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

    private void LookAround1()
    {
        LayerMask mask = LayerMask.GetMask("Obstacles", "Water");
        RaycastHit2D hit = Physics2D.Linecast(rb2d.position, destination, mask);
        Debug.DrawLine(rb2d.position, destination, Color.green);
        Debug.Log($"<color=green>Move {rb2d.position} to {destination}</color>");
        if (hit.collider != null)        
        {
            detour = hit.collider.ClosestPoint(hit.point);
            hit = Physics2D.Linecast(rb2d.position, detour, mask);
            Debug.DrawLine(rb2d.position, detour, Color.red);
            Debug.Log($"<color=red>Move {rb2d.position} to {detour}</color>");
        }
    }
    private Vector2 LookAround(Vector2 destination)
    {
        LayerMask mask = LayerMask.GetMask("Obstacles", "Water");
        RaycastHit2D hit = Physics2D.Linecast(rb2d.position, destination, mask);
        Debug.DrawLine(rb2d.position, destination, Color.green);
        // Debug.Log($"<color=green>Move {rb2d.position} to {destination}</color>");
        if (hit.collider != null)        
        {
            // if (((Vector2)hit.collider.bounds.center - rb2d.position).x - (hit.point - rb2d.position).x <= 0)
            if (Vector2.SignedAngle((Vector2)hit.collider.bounds.center - rb2d.position, hit.point - rb2d.position) <= 0)
            {
                Debug.DrawLine(rb2d.position, (Vector2)hit.collider.bounds.center, Color.red);
                if (left)
                Debug.Log("<color=green>Go to Right</color>");
                left = false;
            }
            else
            {
                if (!left)
                Debug.Log("<color=red>Go to Left</color>");
                left = true;
            }

            /* if (hit.point.Equals(Vector2.Min(hit.point, hit.centroid)))
            {
                if (hit.point.Equals(Vector2.Min(hit.point, hit.centroid)))
                {

                }
                detour = hit.collider.bounds.min;
                detour.Set(detour.x, detour.y - 0.6f);
                hit = Physics2D.Linecast(rb2d.position, detour, mask);
                Debug.DrawLine(rb2d.position, detour, Color.red);
                Debug.Log($"<color=red>Centroid {hit.centroid} to {detour}</color>");
                return detour;
            } */
            // Debug.Log($"<color=white>Move {detour} to {hit.collider.bounds.min}</color>");
        }
        return destination;
    }

    private IEnumerator Detour()
    {
        while (true)
        {

            yield return null;
        }
        yield return new WaitForSeconds(1);
    }
}
