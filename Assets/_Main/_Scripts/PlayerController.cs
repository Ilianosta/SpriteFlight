using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] UIDocument uiDoc;
    [SerializeField] GameObject boosterFlame;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float thrustForce = 4;
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float scoreMultiplier = 1;
    [SerializeField] MyAudioClip[] audioClips;
    float elapsedTime = 0;
    float score = 0;
    Rigidbody2D rb;
    Label scoreText, highScoreText;
    Button restartButton;
    AudioSource audioSource;
    const string HIGHSCOREKEY = "HighScore";

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        scoreText = uiDoc.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDoc.rootVisualElement.Q<Label>("HighScoreLabel");
        restartButton = uiDoc.rootVisualElement.Q<Button>("RestartButton");

        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        highScoreText.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        MovePlayer();
    }

    void UpdateScore()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;
    }

    void MovePlayer()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            PlayAudioClip("Thrust");
        }
        else
        {
            StopAudioClip("Thrust");
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
        }
    }

    void UpdateHighScore()
    {
        if (score > PlayerPrefs.GetInt(HIGHSCOREKEY) || !PlayerPrefs.HasKey(HIGHSCOREKEY))
        {
            PlayerPrefs.SetInt(HIGHSCOREKEY, Mathf.FloorToInt(score));
        }
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt(HIGHSCOREKEY, 0);
        highScoreText.style.display = DisplayStyle.Flex;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        restartButton.style.display = DisplayStyle.Flex;
        Destroy(gameObject);

        StopAllAudio();
        AudioManager.Instance.PlaySFX(audioClips.FirstOrDefault(c => c.name == "Explosion")?.audioClip);

        UpdateHighScore();
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void PlayAudioClip(string name)
    {
        if (audioSource.isPlaying && audioSource.clip == audioClips.FirstOrDefault(c => c.name == name)?.audioClip) return;

        foreach (var clip in audioClips)
        {
            if (clip.name == name)
            {
                audioSource.clip = clip.audioClip;
                audioSource.Play();
                return;
            }
        }
        Debug.LogWarning("Audio clip not found: " + name);
    }

    void StopAudioClip(string name)
    {
        foreach (var clip in audioClips)
        {
            if (clip.name == name)
            {
                audioSource.Stop();
                return;
            }
        }
        Debug.LogWarning("Audio clip not found: " + name);
    }

    void StopAllAudio()
    {
        audioSource.Stop();
    }
}
[System.Serializable]
public class MyAudioClip
{
    public string name;
    public AudioClip audioClip;
}

