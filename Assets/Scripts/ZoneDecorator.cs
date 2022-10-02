using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static HexagonHelpers;

public class ZoneDecorator : MonoBehaviour {

    public List<GameObject> spawnRequired;

    public List<GameObject> bossRequired;

    public List<GameObject> enemyRequired;

    public List<GameObject> friendlyRequired;

    List<int> bossRoomScenes = new List<int> {3, 4, 5 };

    public bool RemoveBossScene(int idx) {
        if (bossRoomScenes.Contains(idx)) {
            bossRoomScenes.Remove(idx);
            return true;
        }
        return false;
    }

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
        GameObject go = Instantiate(spawnRequired[0], this.transform );
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
        GameObject go = Instantiate(bossRequired[0], this.transform);
        SetPosition(go, (int)x0, 1, (int)y0);
        zone.decorations.Add(go);
        
        Teleport tele = go.GetComponent<Teleport>();
        if (tele != null) {
            tele.sceneIndex = bossRoomScenes[Random.Range(0, bossRoomScenes.Count)]; // boss rooms start at index 3 in the room loader order
        }
    }

    void DecorateEnemy(Zone zone) {
        float x0 = zone.x + (float)zone.w / 2;
        float y0 = zone.y + (float)zone.h / 2;

        //spawn objects
        GameObject go = Instantiate(enemyRequired[0], this.transform);
        SetPosition(go, (int)x0, 1, (int)y0);
        zone.decorations.Add(go);
    }

    void DecorateFriendly(Zone zone) {
        float x0 = zone.x + (float)zone.w / 2;
        float y0 = zone.y + (float)zone.h / 2;

        //spawn objects
        GameObject go = Instantiate(friendlyRequired[0], this.transform);
        SetPosition(go, (int)x0, 1, (int)y0);
        zone.decorations.Add(go);
    }


}
