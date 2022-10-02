using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_AbilityPanel : MonoBehaviour {

    public GameObject buttonPrefab;

    TMP_Player player;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<TMP_Player>();
        UpdateAbilityUI();
    }

    
    public void OnUpdateAbilities(System.Object src, EventArgs e) {
        UpdateAbilityUI();
    }


    void UpdateAbilityUI() {
        while (transform.childCount > 1) {
            GameObject.Destroy(transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < player.abilities.Count; i++) {
            if (player.abilities[i].unlocked) {
                GameObject go = GameObject.Instantiate(buttonPrefab, this.transform);
                go.transform.SetSiblingIndex(go.transform.GetSiblingIndex() - 1); //move 1 up the hierarchys
                Button b = go.GetComponent<Button>();
                b.GetComponentInChildren<Text>().text = player.abilities[i].name;
            }
        }
    }

}
