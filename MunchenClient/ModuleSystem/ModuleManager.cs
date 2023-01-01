using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Il2CppSystem;
using MelonLoader;
using MunchenClient.Config;
using MunchenClient.ModuleSystem.Modules;
using MunchenClient.ModuleSystem.Modules.AntiAssetBundleCrasher;
using MunchenClient.Utils;
using Photon.Realtime;
using UnityEngine;

namespace MunchenClient.ModuleSystem
{
	internal class ModuleManager
	{
		private static readonly List<ModuleComponent> modules = new List<ModuleComponent>();

		internal static void ConstructModules()
		{
			try
			{
				//modules.Add(new ApplicationBotHandler()); //appbots removed
				//modules.Add(new AntiAssetBundleCrashHandler()); //loads acab, untested
				modules.Add(new GeneralHandler()); //broken on room left
				//modules.Add(new AntiInstanceLockHandler()); //broken

				if (true/*appbots removed !ApplicationBotHandler.IsBot()*/)
				{
					modules.Add(new PlayerEventsHandler());
					modules.Add(new PlayerHandler());
					//modules.Add(new PlayerTargetHandler()); //broken on room left
					modules.Add(new KeybindHandler()); //broken on update
					//modules.Add(new FlashlightHandler()); //causes so many errors lmfao
					modules.Add(new ProtectionsHandler()); 
					//modules.Add(new GlobalDynamicBonesHandler()); //depreciated, OSC update
					//modules.Add(new GlobalAvatarDatabaseHandler()); //depreciated, big menu update
					//modules.Add(new MenuColorHandler()); //depreciated, big menu update
					//modules.Add(new AntiShaderCrashHandler()); //antishadercrashmodule.dll can probably not load because of eac
					modules.Add(new AntiPortalHandler());
					modules.Add(new CameraFeaturesHandler()); //broken (changecameraactualzoomstate)
					//modules.Add(new AvatarFavoritesHandler()); //depreciated big menu update
					modules.Add(new InstanceHistoryHandler());
					//modules.Add(new AvatarHistoryHandler()); 
					modules.Add(new ModerationHandler());
					//modules.Add(new MimicHandler()); //depreciated, useless
                    //modules.Add(new LovenseHandler()); //depreciated, gross
				}
			}
			catch (System.Exception e) { ConsoleUtils.Exception("ModuleManager", "ConstructModules", e, "ConstructModules", 62); }
		}
		#region boilerplate
		internal static void InitializeModules()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnApplicationStart();
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "InitializeModules (Module Name: " + modules[i].GetModuleName() + ")", e, "InitializeModules", 76);
				}
			}
		}

		internal static void DeinitializeModules()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnApplicationQuit();
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "DeinitializeModules (Module Name: " + modules[i].GetModuleName() + ")", e, "DeinitializeModules", 91);
				}
			}
			modules.Clear();
		}

		internal static void OnUIManagerLoaded()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnUIManagerLoaded();
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnUIManagerLoaded (Module Name: " + modules[i].GetModuleName() + ")", e, "OnUIManagerLoaded", 108);
				}
			}
		}

		internal static void ShutdownModules()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnApplicationQuit();
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "ShutdownModules (Module Name: " + modules[i].GetModuleName() + ")", e, "ShutdownModules", 124);
				}
			}
		}

		internal static void OnLevelWasLoaded(int level)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnLevelWasLoaded(level);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnLevelWasLoaded (Module Name: " + modules[i].GetModuleName() + ")", e, "OnLevelWasLoaded", 139);
				}
			}
		}

		internal static void OnLevelWasInitialized(int levelIndex)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnLevelWasInitialized(levelIndex);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnLevelWasInitialized (Module Name: " + modules[i].GetModuleName() + ")", e, "OnLevelWasInitialized", 154);
				}
			}
		}

		internal static void OnLevelWasUnloaded(int levelIndex)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnLevelWasUnloaded(levelIndex);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnLevelWasUnloaded (Module Name: " + modules[i].GetModuleName() + ")", e, "OnLevelWasUnloaded", 169);
				}
			}
		}

		internal static void OnUpdate()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnUpdate();
				}
				catch (System.Exception e)
				{
					//ConsoleUtils.Exception("ModuleManager", "OnUpdate (Module Name: " + modules[i].GetModuleName() + ")", e, "OnUpdate", 184); //stacks really hard while waiting for menu so i disable for now
				}
			}
		}

		internal static void OnLateUpdate()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnLateUpdate();
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnLateUpdate (Module Name: " + modules[i].GetModuleName() + ")", e, "OnLateUpdate", 199);
				}
			}
		}

		internal static void OnFixedUpdate()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnFixedUpdate();
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnFixedUpdate (Module Name: " + modules[i].GetModuleName() + ")", e, "OnFixedUpdate", 214);
				}
			}
		}

		internal static void OnPlayerJoin(PlayerInformation playerInfo)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnPlayerJoin(playerInfo);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnPlayerJoin (Module Name: " + modules[i].GetModuleName() + ")", e, "OnPlayerJoin", 231);
				}
			}
		}

		internal static void OnPlayerLeft(PlayerInformation playerInfo)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnPlayerLeft(playerInfo);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnPlayerLeft (Module Name: " + modules[i].GetModuleName() + ")", e, "OnPlayerLeft", 246);
				}
			}
		}

		internal static void OnRoomJoined()
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnRoomJoined();
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnRoomJoined (Module Name: " + modules[i].GetModuleName() + ")", e, "OnRoomJoined", 261);
				}
			}
		}

		internal static void OnRoomLeft() //broken broken broken
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnRoomLeft();
				}
				catch (System.Exception e)
				{
					//ConsoleUtils.Exception("ModuleManager", "OnRoomLeft (Module Name: " + modules[i].GetModuleName() + ")", e, "OnRoomLeft", 276);
				}
			}
		}

		internal static void OnRoomMasterChanged(PlayerInformation newMaster)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnRoomMasterChanged(newMaster);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnRoomMasterChanged (Module Name: " + modules[i].GetModuleName() + ")", e, "OnRoomMasterChanged", 291);
				}
			}
		}

		internal static void OnAvatarLoaded(string playerId, string playerName, ref GameObject avatar)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnAvatarLoaded(playerId, playerName, ref avatar);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnAvatarLoaded (Module Name: " + modules[i].GetModuleName() + ")", e, "OnAvatarLoaded", 306);
				}
			}
		}

		internal static bool OnPortalEntered(ref PortalInternal portal)
		{
			bool result = true;
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					if (!modules[i].OnPortalEntered(ref portal))
					{
						result = false;
					}
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnPortalEntered (Module Name: " + modules[i].GetModuleName() + ")", e, "OnPortalEntered", 326);
				}
			}
			return result;
		}

		internal static void OnPortalCreated(ref PortalInternal portal, string worldId, string instanceId, int playerCount)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnPortalCreated(ref portal, worldId, instanceId, playerCount);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnPortalCreated (Module Name: " + modules[i].GetModuleName() + ")", e, "OnPortalCreated", 343);
				}
			}
		}

		internal static void OnPortalDestroyed(ref PortalInternal portal)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnPortalDestroyed(ref portal);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnPortalDestroyed (Module Name: " + modules[i].GetModuleName() + ")", e, "OnPortalDestroyed", 358);
				}
			}
		}

		internal static void OnPortalSetTimer(ref PortalInternal portal, float timer)
		{
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					modules[i].OnPortalSetTimer(ref portal, timer);
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnPortalSetTimer (Module Name: " + modules[i].GetModuleName() + ")", e, "OnPortalSetTimer", 373);
				}
			}
		}

		internal static bool OnEventReceived(ref EventData eventData)
		{
			bool result = true;
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					if (!modules[i].OnEventReceived(ref eventData))
					{
						result = false;
					}
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnEventReceived (Module Name: " + modules[i].GetModuleName() + ")", e, "OnEventReceived", 393);
				}
			}
			return result;
		}

		internal static bool OnEventSent(byte eventCode, ref Il2CppSystem.Object eventData, ref RaiseEventOptions raiseEventOptions)
		{
			bool result = true;
			for (int i = 0; i < modules.Count; i++)
			{
				try
				{
					if (!modules[i].OnEventSent(eventCode, ref eventData, ref raiseEventOptions))
					{
						result = false;
					}
				}
				catch (System.Exception e)
				{
					ConsoleUtils.Exception("ModuleManager", "OnEventSent (Module Name: " + modules[i].GetModuleName() + ")", e, "OnEventSent", 415);
				}
			}
			return result;
		}
		#endregion
	}
}
