using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using HarmonyLib;

namespace ScrapPack
{
    public static class Utils
    {
	//Network prefab -----------
	private static List<GameObject> networkPrefabsToAdd = new();

        public static void AddNetworkPrefab(GameObject prefab) => networkPrefabsToAdd.Add(prefab);

	[HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        public static void GameNetworkManagerStartPostfix(GameNetworkManager __instance){
	    var networkManager =  __instance.GetComponent<NetworkManager>();
	    foreach(var g in networkPrefabsToAdd) networkManager.AddNetworkPrefab(g); 
	    
	}
    }
}
