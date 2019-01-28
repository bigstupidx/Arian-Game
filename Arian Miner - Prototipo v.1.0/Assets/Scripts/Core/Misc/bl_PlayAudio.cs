using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class bl_PlayAudio : MonoBehaviour {

	[SerializeField]private AudioClip[] Clips;


    public void Play(int id)
    {
        AudioSource.PlayClipAtPoint(Clips[id], transform.position);
    }
}