using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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
