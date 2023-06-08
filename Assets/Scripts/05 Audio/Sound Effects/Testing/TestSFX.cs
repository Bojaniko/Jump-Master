using UnityEngine;

namespace Studio28.Audio.SFX.Testing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    public class TestSFX : MonoBehaviour
    {

        public void PlaySound()
        {
            gameObject.GetComponent<AudioSource>().Play();
        }

        public void StopSound()
        {
            gameObject.GetComponent<AudioSource>().Stop();
        }
    }
}