using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_AbilityPanel : MonoBehaviour {

    public Image PitfallIcon;
    public GameObject pitfallText;
    PlayerManager player;
    PitfallManager pm;

    public Sprite DownSprite;
    public Sprite UpSprite;
    public Sprite SettingUpSprite;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        pm = player.GetComponent<PitfallManager>();
        player.OnUnlockPitfall += UnlockAbility;

        //set pitfall icon to no ability
        // PitfallIcon
        pitfallText.SetActive(false);
    }


    public void UnlockAbility(System.Object src, EventArgs e) {
        PitfallIcon.gameObject.SetActive(true);
        pitfallText.gameObject.SetActive(true);
        //play a sound or something!

        pm.OnPitfallStateChange += OnPitfallStateChange;
        PitfallIcon.sprite = DownSprite;
    }


    public void OnPitfallStateChange(System.Object src, EventArgs e) {
        switch (pm.pitfallState) {
            default:
            case PitfallManager.PitfallStates.down:
                PitfallIcon.sprite = DownSprite;
                break;
            case PitfallManager.PitfallStates.setUp:
                PitfallIcon.sprite = UpSprite;
                break;
            case PitfallManager.PitfallStates.settingUp:
                PitfallIcon.sprite = SettingUpSprite;
                break;
        }
    }

}
