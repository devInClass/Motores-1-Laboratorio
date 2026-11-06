using UnityEngine;

namespace SlugLab.Audio
{
    /// <summary>
    /// Reproduce efectos de sonido con un pequeño pool de voces para que varios
    /// sonidos puedan sonar a la vez sin cortarse.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;

        [SerializeField] private int voiceCount = 8;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 0.8f;

        private AudioSource[] _voices;
        private int _nextVoice;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _voices = new AudioSource[Mathf.Max(1, voiceCount)];

            for (int i = 0; i < _voices.Length; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                _voices[i] = source;
            }
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        /// <summary>Atajo estático: AudioManager.PlaySfx(clip).</summary>
        public static void PlaySfx(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;

            if (_instance == null) _instance = FindFirstObjectByType<AudioManager>();
            if (_instance == null) return;

            _instance.Play(clip, volume);
        }

        public void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null || _voices == null) return;

            AudioSource voice = _voices[_nextVoice];
            _nextVoice = (_nextVoice + 1) % _voices.Length;
            voice.PlayOneShot(clip, Mathf.Clamp01(volume * masterVolume));
        }
    }
}
