using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource disableBoss;
    public AudioSource pitfallSound;
    public AudioSource mainBackgroundMusic;
    public AudioSource bossOneBackgroundMusic;
    public AudioSource bossTwoBackgroundMusic;

    private GameObject player;
    private PitfallManager pitfallManager;
    private PlayerManager playerManager;

    private int bossFight = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pitfallManager = player.GetComponent<PitfallManager>();
        playerManager = player.GetComponent<PlayerManager>();

        disableBoss.volume = SoundConstants.VOLUME;
        pitfallSound.volume = SoundConstants.VOLUME;
        mainBackgroundMusic.volume = SoundConstants.VOLUME;
        bossOneBackgroundMusic.volume = SoundConstants.VOLUME;
        bossTwoBackgroundMusic.volume = SoundConstants.VOLUME;
    }

    // Start is called before the first frame update
    void Start()
    {
        OverworldManager.instance.OnBossDisable += PlayDisableSound;
        BattleTimeManager.instance.OnBattleStart += PlayBossFightMusic;
        BattleTimeManager.instance.OnBattleEnd += PlayBackgroundMusic;
        pitfallManager.OnPitfallStateChange += PlayPickUpSound;
        playerManager.OnUnlockPitfall += PlayPickUpSound;
    }

    public void PlayDisableSound(System.Object src, System.EventArgs e)
    {
        disableBoss.Play();
    }

    public void PlayPickUpSound(System.Object src, System.EventArgs e)
    {
        pitfallSound.Play();
    }

    public void PlayBossFightMusic(System.Object src, System.EventArgs e)
    {
        if (bossFight == 0)
        {
            mainBackgroundMusic.Stop();
            bossOneBackgroundMusic.Play();
        } else
        {
            mainBackgroundMusic.Stop();
            bossTwoBackgroundMusic.Play();
        }

        bossFight += 1;
    }


    public void PlayBackgroundMusic(System.Object src, System.EventArgs e)
    {
        bossOneBackgroundMusic.Stop();
        bossTwoBackgroundMusic.Stop();
        mainBackgroundMusic.Play();
    }
}
