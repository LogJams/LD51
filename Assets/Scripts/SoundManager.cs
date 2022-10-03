using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource disableBoos;
    public AudioSource pitfallSound;

    private GameObject player;
    private PitfallManager pitfallManager;
    private PlayerManager playerManager;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pitfallManager = player.GetComponent<PitfallManager>();
        playerManager = player.GetComponent<PlayerManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        OverworldManager.instance.OnBossDisable += PlayDisableSound;
        pitfallManager.OnPitfallStateChange += PlayPickUpSound;
        playerManager.OnUnlockPitfall += PlayPickUpSound;
    }

    public void PlayDisableSound(System.Object src, System.EventArgs e)
    {
        disableBoos.Play();
    }

    public void PlayPickUpSound(System.Object src, System.EventArgs e)
    {
        pitfallSound.Play();
    }
}
