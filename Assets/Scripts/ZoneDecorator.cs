using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static HexagonHelpers;

public class ZoneDecorator : MonoBehaviour {

    public GameObject signpost;
    public GameObject treasure;
    public GameObject door;
    public GameObject simpleEnemy;


    public GameObject turretBoss;
    public GameObject hunterBoss;

    int bossSceneIndex = 0;

    public void DecorateZone(Zone zone) {
        zone.decorations = new List<GameObject>();
        switch (zone.type) {
            default:
            case ZONE_TYPES.SPAWN:
                DecorateSpawn(zone);
                break;
            case ZONE_TYPES.BOSS:
                DecorateBoss(zone);
                break;

            case ZONE_TYPES.ENEMY:
                DecorateEnemy(zone);
                break;

            case ZONE_TYPES.FRIENDLY:
                DecorateFriendly(zone);
                break;
        }
    }


    void DecorateSpawn(Zone zone) {
        float x0 = zone.x + (float)zone.w / 2;
        float y0 = zone.y + (float)zone.h / 2;

        //spawn objects
        GameObject go = Instantiate(signpost, this.transform );
        SetPosition(go, (int)x0, 1, (int)y0);
        zone.decorations.Add(go);

        //move the player into the domain
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerManager playerManager = player.GetComponent<PlayerManager>();
        playerManager.SetCurrentTilePosition(new Vector3(x0, 0, y0 - 2));

        SetPosition(player, (int)x0, 2.5f, (int)y0 - 2);
    }

    void DecorateBoss(Zone zone) {
        float x0 = zone.x + (float)zone.w / 2;
        float y0 = zone.y + (float)zone.h / 2;

        //spawn objects
        GameObject go = Instantiate(treasure, this.transform);
        SetPosition(go, (int)x0, 1, (int)y0);
        zone.decorations.Add(go);





        //spawn the boss
        if (bossSceneIndex == 0) {
            GameObject boss = Instantiate(turretBoss, this.transform);
            SetPosition(boss, (int)(x0 + 3), 0, (int)y0);
            zone.decorations.Add(go);
            BattleTimeManager.instance.SetBoss1(boss.GetComponent<TurretBoss>());
        }
        if (bossSceneIndex == 1) {
            GameObject boss = Instantiate(hunterBoss, this.transform);
            SetPosition(boss, (int)(x0 + 3), 0, (int)y0);
            zone.decorations.Add(go);
            BattleTimeManager.instance.SetBoss2(boss.GetComponent<HuntingBoss>());
        }
        bossSceneIndex++;
        zone.boss_index = bossSceneIndex; // 1 or 2 for boss1 and boss2
    }

    void DecorateEnemy(Zone zone) {
        float x0 = zone.x + (float)zone.w / 2;
        float y0 = zone.y + (float)zone.h / 2;

        //spawn objects
        GameObject go = Instantiate(simpleEnemy, this.transform);
        SetPosition(go, (int)x0, 1, (int)y0);
        zone.decorations.Add(go);

        go.GetComponent<SimpleEnemy>().parentZone = zone;

    }

    void DecorateFriendly(Zone zone) {
        float x0 = zone.x + (float)zone.w / 2;
        float y0 = zone.y + (float)zone.h / 2;

        //spawn objects
        GameObject go = Instantiate(door, this.transform);
        SetPosition(go, (int)x0, 1, (int)y0);
        zone.decorations.Add(go);
    }


}
