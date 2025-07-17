using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public enum AudioType
    {
        MasterVolume,
        MusicVolume,
        SFXVolume
    }
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        // Asegurar que el AudioManager sea un singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
    }

    #region Volumen
    public float GetVolumePercentage(AudioType audioType)
    {
        audioMixer.GetFloat(audioType.ToString(), out float volume);
        float linearValue = DbToLinear(volume);
        return linearValue;
    }
    public void SetMusicVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat(AudioType.MusicVolume.ToString(), -80);
            return;
        }
        audioMixer.SetFloat(AudioType.MusicVolume.ToString(), Mathf.Log10(volume) * 20); // Convierte a escala logarítmica
    }

    public void SetSFXVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat(AudioType.SFXVolume.ToString(), -80);
            return;
        }
        audioMixer.SetFloat(AudioType.SFXVolume.ToString(), Mathf.Log10(volume) * 20); // Convierte a escala logarítmica
    }
    #endregion

    #region Reproducción de Sonido
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    #endregion

    // Convierte de dB a escala lineal (0 a 1)
    private float DbToLinear(float db)
    {
        return Mathf.Pow(10, db / 20);
    }

    // Convierte de escala lineal (0 a 1) a dB
    private float LinearToDb(float linear)
    {
        return linear > 0 ? 20 * Mathf.Log10(linear) : -80f; // -80 dB es el mínimo estándar en Unity
    }
}
