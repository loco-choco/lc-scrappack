using UnityEngine;

namespace ScrapPack
{
    public class ScrapPack : GrabbableObject
    {
	//public GameObject fireEffect;

	//public AudioClip startJetpackSFX;

	//public AudioClip jetpackWarningBeepSFX;

	//public ParticleSystem smokeTrailParticle;

	//private PlayerControllerB previousPlayerHeldBy;

	//private float noiseInterval;

	//private RaycastHit rayHit;

	public override void ItemActivate(bool used, bool buttonDown = true){
	    base.ItemActivate(used, buttonDown);
	    if (buttonDown) ActivateJetpack();
	}

	//private void AfterJetpackImpulse(){
	//    if (previousPlayerHeldBy.jetpackControls)
	//	previousPlayerHeldBy.disablingJetpackControls = true;
	//   smokeTrailParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
	//}

	private void ActivateJetpack()
	{
	    playerHeldBy.syncFullRotation = playerHeldBy.transform.eulerAngles;
	    playerHeldBy.externalForces += Vector3.up * 100;
	}
        /*
	public override void DiscardItem()
	{
		Debug.Log($"Owner of jetpack?: {base.IsOwner}");
		Debug.Log($"Is dead?: {playerHeldBy.isPlayerDead}");
		if (base.IsOwner && playerHeldBy.isPlayerDead && !jetpackBroken && playerHeldBy.jetpackControls)
		    ExplodeJetpackServerRpc();
		base.DiscardItem();
	}

	[ServerRpc(RequireOwnership = false)]
	public void ExplodeJetpackServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(3663112878u, serverRpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 3663112878u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				ExplodeJetpackClientRpc();
			}
		}
	}

	[ClientRpc]
	public void ExplodeJetpackClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(2295726646u, clientRpcParams, RpcDelivery.Reliable);
				__endSendClientRpc(ref bufferWriter, 2295726646u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !jetpackBroken)
			{
				jetpackBroken = true;
				itemUsedUp = true;
				Debug.Log("Spawning explosion");
				Landmine.SpawnExplosion(base.transform.position, spawnExplosionEffect: true, 5f, 7f);
			}
		}
	}

	public override void EquipItem()
	{
		base.EquipItem();
		previousPlayerHeldBy = playerHeldBy;
	}

	public override void Update()
	{
		base.Update();
		if (GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
		{
			return;
		}
		SetJetpackAudios();
		if (playerHeldBy == null || !base.IsOwner || playerHeldBy != GameNetworkManager.Instance.localPlayerController)
		{
			return;
		}
		if (jetpackActivated)
		{
			jetpackPower = Mathf.Clamp(jetpackPower + Time.deltaTime * 10f, 0f, 500f);
		}
		else
		{
			jetpackPower = Mathf.Clamp(jetpackPower - Time.deltaTime * 75f, 0f, 1000f);
			if (playerHeldBy.thisController.isGrounded)
			{
				jetpackPower = 0f;
			}
		}
		forces = Vector3.Lerp(forces, Vector3.ClampMagnitude(playerHeldBy.transform.up * jetpackPower, 400f), Time.deltaTime);
		if (!playerHeldBy.jetpackControls)
		{
			forces = Vector3.zero;
		}
		if (!playerHeldBy.isPlayerDead && Physics.Raycast(playerHeldBy.transform.position, forces, out rayHit, 25f, StartOfRound.Instance.allPlayersCollideWithMask) && forces.magnitude - rayHit.distance > 50f && rayHit.distance < 4f)
		{
			playerHeldBy.KillPlayer(forces, spawnBody: true, CauseOfDeath.Gravity);
		}
		playerHeldBy.externalForces += forces;
	}

	private void SetJetpackAudios()
	{
		if (jetpackActivated)
		{
			if (noiseInterval >= 0.5f)
			{
				noiseInterval = 0f;
				RoundManager.Instance.PlayAudibleNoise(base.transform.position, 25f, 0.85f, 0, playerHeldBy.isInHangarShipRoom && StartOfRound.Instance.hangarDoorsClosed, 41);
			}
			else
			{
				noiseInterval += Time.deltaTime;
			}
			if (insertedBattery.charge < 0.15f)
			{
				if (!jetpackPlayingLowBatteryBeep)
				{
					jetpackPlayingLowBatteryBeep = true;
					jetpackBeepsAudio.clip = jetpackLowBatteriesSFX;
					jetpackBeepsAudio.Play();
				}
			}
			else if (Physics.CheckSphere(base.transform.position, 6f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
			{
				if (!jetpackPlayingWarningBeep && !jetpackPlayingLowBatteryBeep)
				{
					jetpackPlayingWarningBeep = true;
					jetpackBeepsAudio.clip = jetpackWarningBeepSFX;
					jetpackBeepsAudio.Play();
				}
			}
			else
			{
				jetpackBeepsAudio.Stop();
			}
		}
		else
		{
			jetpackPlayingWarningBeep = false;
			jetpackPlayingLowBatteryBeep = false;
			jetpackBeepsAudio.Stop();
		}
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		if (playerHeldBy != null && isHeld)
		{
			backPart.position = playerHeldBy.lowerSpine.position;
			backPart.rotation = playerHeldBy.lowerSpine.rotation;
			base.transform.Rotate(backPartRotationOffset);
			backPart.position = playerHeldBy.lowerSpine.position;
			Vector3 vector = backPartPositionOffset;
			vector = playerHeldBy.lowerSpine.rotation * vector;
			backPart.position += vector;
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void InitializeRPCS_JetpackItem()
	{
		NetworkManager.__rpc_func_table.Add(3663112878u, __rpc_handler_3663112878);
		NetworkManager.__rpc_func_table.Add(2295726646u, __rpc_handler_2295726646);
	}

	private static void __rpc_handler_3663112878(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((JetpackItem)target).ExplodeJetpackServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2295726646(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((JetpackItem)target).ExplodeJetpackClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	protected internal override string __getTypeName()
	{
		return "JetpackItem";
	}*/
    }
}
