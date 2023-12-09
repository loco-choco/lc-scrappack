using System.IO;
using BepInEx;
using UnityEngine;
using LethalLib.Modules;

namespace ScrapPack
{
    [BepInPlugin("locochoco.scrappack", "ScrapPack", "0.1.0")] // (GUID, mod name, mod version)
    public class ScrapPackMod : BaseUnityPlugin
    {
        public void OnEnable(){
	    Logger.LogInfo("Scrap Pack mod Enabled!");
	    transform.position = Vector3.zero;
	    var scrappackBundle = AssetBundle.LoadFromFile(Path.Combine(Info.Location, "scrappack")); 
	    var scrappackPrefab = scrappackBundle.LoadAsset<GameObject>("scrappack");
	    Utils.AddNetworkPrefab(scrappackPrefab);

            Items.RegisterShopItem(scrappackPrefab.GetComponent<Item>(), 0);
	}
    }
}
