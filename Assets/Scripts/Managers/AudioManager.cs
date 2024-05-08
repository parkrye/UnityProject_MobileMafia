using UnityEngine.Audio;

public class AudioManager : BaseManager
{
    private AudioMixer _audioMixer;

    private void Awake()
    {
        _audioMixer = GameManager.Resource.Load<AudioMixer>("Audio/AudioMixer");
    }

    public float MasterVolume
    {
        get
        {
            float volume;
            _audioMixer.GetFloat("Master", out volume);
            return volume;
        }
        set
        {
            if (value == 0f)
            {
                _audioMixer.SetFloat("Master", -80f);
            }
            else
            {
                _audioMixer.SetFloat("Master", -40f + value * 40f);
            }
        }
    }
    public float BGMVolume
    {
        get
        {
            float volume;
            _audioMixer.GetFloat("BGM", out volume);
            return volume;
        }
        set
        {
            if (value == 0f)
            {
                _audioMixer.SetFloat("BGM", -80f);
            }
            else
            {
                _audioMixer.SetFloat("BGM", -40f + value * 40f);
            }
        }
    }
    public float SFXVolume
    {
        get
        {
            float volume;
            _audioMixer.GetFloat("SFX", out volume);
            return volume;
        }
        set
        {
            if (value == 0f)
            {
                _audioMixer.SetFloat("SFX", -80f);
            }
            else
            {
                _audioMixer.SetFloat("SFX", -40f + value * 40f);
            }
        }
    }
}