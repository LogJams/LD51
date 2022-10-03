using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource disableBoos;

    // Start is called before the first frame update
    void Start()
    {
        OverworldManager.instance.OnBossDisable += PlayDisableSound;
    }

    public void PlayDisableSound(System.Object src, System.EventArgs e)
    {
        disableBoos.Play();
    }
}
