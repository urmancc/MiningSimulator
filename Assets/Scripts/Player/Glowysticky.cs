using UnityEngine;
using System.Collections;

public class GlowStick2D : MonoBehaviour
{
    [Header("Shaking & Light")]
    private int targetShakes;
    private int currentShakes = 0;
    public float shakeDistance = 0.5f;
    public Light stickLight;

    [Header("Throwing")]
    public LineRenderer line;
    public Transform handPoint; 
    public float throwPower = 10f;
    private bool isThrown = false;

    void Start()
    {
        targetShakes = Random.Range(5, 11);
        if (stickLight) stickLight.enabled = false;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (isThrown) return;

        // 1. Follow Player Hand
        if (handPoint != null)
        {
            transform.position = handPoint.position;
        }

        // 2. Shake Logic (F Key)
        if (Input.GetKeyDown(KeyCode.F))
        {
            currentShakes++;
            StartCoroutine(VisualBounce());

            if (currentShakes >= targetShakes && stickLight)
            {
                stickLight.enabled = true;
            }
        }

        // 3. Trajectory & Throw (T Key)
        if (Input.GetKey(KeyCode.T))
        {
            if (line != null) 
            {
                line.enabled = true;
                DrawTrajectory2D();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (line != null) line.enabled = false;
            Throw2D();
        }
    }

    IEnumerator VisualBounce()
    {
        transform.localPosition += new Vector3(0, shakeDistance, 0);
        yield return new WaitForSeconds(0.05f);
        transform.localPosition -= new Vector3(0, shakeDistance, 0);
    }

    void DrawTrajectory2D()
    {
        int points = 15;
        line.positionCount = points;
        Vector2 startPos = transform.position;
        
        Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mPos - startPos).normalized;
        Vector2 velocity = direction * throwPower;

        for (int i = 0; i < points; i++)
        {
            float t = i * 0.1f;
            Vector2 point = startPos + (velocity * t) + 0.5f * Physics2D.gravity * (t * t);
            line.SetPosition(i, point);
        }
    }

    void Throw2D()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            isThrown = true;
            transform.SetParent(null);
            rb.bodyType = RigidbodyType2D.Dynamic;

            Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mPos - (Vector2)transform.position).normalized;
            rb.AddForce(dir * throwPower, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isThrown)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.bodyType = RigidbodyType2D.Kinematic;
                transform.SetParent(collision.transform);
            }
        }
    }
}
