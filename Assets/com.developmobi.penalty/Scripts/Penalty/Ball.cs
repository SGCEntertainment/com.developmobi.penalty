using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    private bool IsFirstContact { get; set; }
    private bool IsGoal { get; set; }
    private Vector2 InitScale { get; set; } = Vector3.one;
    private Vector2 TargetScale { get; set; } = Vector3.one * 0.4176936f;

    private Transform Center { get; set; }

    public float totalDistance = 4.0f;
    private const float force = 25;

    private AudioSource Source { get; set; }

    private static Rigidbody2D Rigidbody { get; set; }
    private static Vector2 Velocity { get; set; }

    private bool EndTravel { get; set; }
    public static Action<bool> OnTravelled { get; set; }
    public static Action<Transform> OnPressed { get; set; }

    private void OnMouseDown()
    {
        if(Source.isPlaying)
        {
            Source.Stop();
        }

        Source.Play();

        GameObject[] targets = GameObject.FindGameObjectsWithTag("target");
        Rigidbody.WakeUp();

        Transform target = targets[Random.Range(0, targets.Length)].transform;
        Vector2 direction = target.transform.position - transform.position;

        Rigidbody.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        Rigidbody.AddTorque(100);
        Invoke(nameof(ResetMe), 1.5f);

        OnPressed?.Invoke(target);
    }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ResetMe();
    }

    private void Update()
    {
        if(!Center)
        {
            Center = GameObject.Find("center").transform;
        }

        float distanceToGoal = Mathf.Abs(Center.position.y - transform.position.y);
        if(distanceToGoal <= 1 && !EndTravel && IsFirstContact)
        {
            EndTravel = true;
            OnTravelled?.Invoke(IsGoal);
        }

        transform.localScale = Vector2.Lerp(TargetScale, InitScale, distanceToGoal / totalDistance);
    }

    private void ResetMe()
    {
        EndTravel = false;

        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0;

        transform.position = new Vector2(0, -4.27f);
        Rigidbody.Sleep();

        IsFirstContact = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(IsFirstContact)
        {
            return;
        }

        if(collision.transform.name != "ground")
        {
            IsFirstContact = true;
            IsGoal = string.Equals(collision.transform.name, "goal");
        }
    }
}
