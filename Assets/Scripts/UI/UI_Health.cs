using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Health : MonoBehaviour
{

    PlayerManager player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        player.OnHealthChange += UpdateHealth;
    }

    void UpdateHealth(System.Object src, System.EventArgs e) {
        int hp = player.Health;

        //child 0 is text
        for (int i = 1; i <= 5; i++) {
            transform.GetChild(i).GetComponent<Image>().enabled = (i <= hp);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
