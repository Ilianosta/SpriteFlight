using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] GameObject bounceEffectPrefab;
    [SerializeField] private float maxScale, minScale = 1;
    [SerializeField] private float minSpeed, maxSpeed = 5;
    [SerializeField] private float maxSpinSpeed = 10;
    Rigidbody2D rb;
    AudioSource audioSource;
    private float GetRandomScale() => Random.Range(minScale, maxScale);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Size
        float randomSize = GetRandomScale();
        transform.localScale = new Vector3(randomSize, randomSize, randomSize);

        // Speed & direction
        Vector2 randomDirection = Random.insideUnitCircle;
        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        rb.AddForce(randomDirection * randomSpeed);

        // Rotation
        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);

        // Destroy the effect after 1 second
        Destroy(bounceEffect, 1f);

        audioSource.pitch = Random.Range(.5f, 1.5f);
        audioSource.Play();
    }
}
