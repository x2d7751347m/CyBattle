using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using TMPro;
using UnityEngine.Serialization;

public class LevelLoader : NetworkBehaviour
{
    public string Scene;
    public TMP_Text PlayerNickName;

    public override void OnStartServer()
    {
        base.OnStartServer();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        InstanceFinder.ClientManager.StartConnection();
        
        var conn = Owner;
            

        var lookupData = new SceneLookupData(Scene);
        /* Create a lookup handle using this objects scene.
         * This is one of many ways FishNet knows what scene to load
         * for the clients. */
        
        var sld = new SceneLoadData(lookupData)
        {
            /* Set automatically unload to false
             * so the server does not unload this scene when
             * there are no more connections in it. */
            Options = new LoadOptions
            {
                AutomaticallyUnload = false
            },
            //Load scenes as additive.
            ReplaceScenes = ReplaceOption.None,
            //Set the preferred active scene so the client changes active scenes.
            PreferredActiveScene = lookupData
        };
        SceneManager.LoadConnectionScenes(conn, sld);
        PlayerNickName.text = InstanceFinder.NetworkManager.GetComponent<GameManager>().PlayerNickName;
    }
}
