using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using MelonLoader;
using MunchenClient.Config;
using MunchenClient.Core;
using MunchenClient.Core.Compatibility;
using MunchenClient.ModuleSystem;
using MunchenClient.ModuleSystem.Modules;
using MunchenClient.Utils;
using MunchenClient.Wrappers;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;

namespace MunchenClient.Patching.Patches
{
	internal class NetworkManagerPatch : PatchComponent
	{
		private static bool isConnectedToInstance = true;

		protected override string patchName => "NetworkManagerPatch";

		internal override void OnInitializeOnStart()
		{
			InitializeLocalPatchHandler(typeof(NetworkManagerPatch));
			PatchMethod((from mb in typeof(NetworkManager).GetMethods()
				where mb.Name.StartsWith("Method_Public_Void_Player_") && CheckMethod(mb, "OnPlayerJoined")
				select mb).First(), null, GetLocalPatch("OnPlayerJoinPatch"));
			PatchMethod((from mb in typeof(NetworkManager).GetMethods()
				where mb.Name.StartsWith("Method_Public_Void_Player_") && CheckMethod(mb, "OnPlayerLeft")
				select mb).Last(), null, GetLocalPatch("OnPlayerLeavePatch"));
			///PatchMethod(typeof(NetworkManager).GetMethod("Method_Internal_Void_PDM_0"), null, GetLocalPatch("OnJoinedRoomPatch")); //fix for on joined world :)
			PatchMethod(typeof(NetworkManager).GetMethod("Method_Public_Void_PDM_0"), null, GetLocalPatch("OnJoinedRoomPatch")); //fix for the peeps Method_Internal_Void_PDM_0 wont work
			//PatchMethod(typeof(NetworkManager).GetMethod("OnJoinedRoom"), null, GetLocalPatch("OnJoinedRoomPatch")); //broken and replaced
			PatchMethod(typeof(NetworkManager).GetMethod("OnLeftRoom"), null, GetLocalPatch("OnLeftRoomPatch")); //works
			PatchMethod(typeof(NetworkManager).GetMethod("OnMasterClientSwitched"), null, GetLocalPatch("OnMasterClientSwitchedPatch"));
		}

		internal static void OnUIManagerInit()
		{
			foreach (VRC.Player item in UnityEngine.Object.FindObjectsOfType<VRC.Player>())
			{
				VRC.Player __ = item;
				OnPlayerJoinPatch(ref __);
			}
		}

		private static void OnJoinedRoomPatch()
		{
			isConnectedToInstance = true;
			ModuleManager.OnRoomJoined();
		}

		private static void OnLeftRoomPatch()
		{
			isConnectedToInstance = false;
			AssetManagementPatch.OnRoomLeft();
			ModuleManager.OnRoomLeft();
		}

		private static void OnMasterClientSwitchedPatch(ref Photon.Realtime.Player __0)
		{
			if (__0 != null)
			{
				PlayerInformation playerInformation = PlayerWrappers.GetPlayerInformation(__0.field_Public_Player_0);
				if (playerInformation != null)
				{
					ModuleManager.OnRoomMasterChanged(playerInformation);
				}
			}
		}

		#region Nameplates
		public static Transform GetBasePlate(VRC.Player PlayerData, NameplateManager[] Nameplates)
        {
			foreach (NameplateManager NameplateContainer in Nameplates)
			{
				string PUID = NameplateContainer.field_Public_VRCPlayer_0._player.field_Private_APIUser_0.id;
				if (PUID == PlayerData.prop_APIUser_0.id) { return NameplateContainer.gameObject.transform; }
            }
			MelonLogger.Msg("Nameplate search returned null");
			return null; 
        }

		public static string RetreiveRank(VRC.Player PlayerData)
        {
            string[] LocalRanks = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "//MünchenClient//Config//LocalRanks.json");
            foreach (string Ranks in LocalRanks)
            {
                string[] Rank = Ranks.Split(':');
                string ID = Rank[0];
                string RankName = Rank[1];
                if (PlayerData.prop_APIUser_0.id == ID)
                {
                    return RankName;
                }
            }
			return null;
        }
        public static Color RetreiveRankColor(VRC.Player PlayerData)
        {
            string[] LocalRanks = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "//MünchenClient//Config//LocalRanks.json");
            foreach (string Ranks in LocalRanks)
            {
                string[] Rank = Ranks.Split(':');
                string ID = Rank[0];
                string RankColor = Rank[2];
                if (PlayerData.prop_APIUser_0.id == ID)
                {
					return VRCPlayerPatch.ColorIdentify(RankColor);
                }
            }
            return Color.white;
        }
		private static void OnPlayerJoinPatch(ref VRC.Player __0)
		{
			if (!isConnectedToInstance || __0 == null || PlayerWrappers.GetPlayerInformation(__0) != null) { return; }
			bool flag = __0.prop_APIUser_0.id == APIUser.CurrentUser.id;
			GameObject canvas = null;
			ImageThreeSlice nameplateBackground = null;
			Image nameplateIconBackground = null;
			GameObject CreatedPlate = null;
			RectTransform rectTransform = null;
			TextMeshProUGUI textMeshProUGUI = null;
			TextMeshProUGUI trustText = null;
			TextMeshProUGUI NameText = null;
            try //total hours wasted debugging this code: 8
            { //im never touching this code again
				NameplateManager[] PlateManager = UnityEngine.Object.FindObjectsOfType<NameplateManager>();
                canvas = GetBasePlate(__0, PlateManager).Find("PlayerNameplate/Canvas").gameObject; //returning the transform and dropping the static variable was the solution
				nameplateBackground = canvas.transform.Find("NameplateGroup/Nameplate/Contents/Main/Background").GetComponent<ImageThreeSlice>();
				nameplateIconBackground = canvas.transform.Find("NameplateGroup/Nameplate/Contents/Icon/Background").GetComponent<Image>();
                trustText = canvas.transform.Find("NameplateGroup/Nameplate/Contents/Quick Stats/Trust Text").GetComponent<TextMeshProUGUI>();

                if (!flag)
				{
                    Transform transform = canvas.transform.Find("NameplateGroup/Nameplate/Contents");
					GameObject gameObject3 = transform.transform.Find("Quick Stats").gameObject;

                    CreatedPlate = UnityEngine.Object.Instantiate(gameObject3, transform);
					rectTransform = CreatedPlate.GetComponent<RectTransform>();
					rectTransform.localPosition = MiscUtils.GetNameplateOffset(GeneralUtils.isQuickMenuOpen);
                    foreach (RectTransform componentsInChild in CreatedPlate.GetComponentsInChildren<RectTransform>())
					{
                        if (componentsInChild.name != "Trust Text")
						{
							componentsInChild.gameObject.SetActive(value: false);
							continue;
						}
						textMeshProUGUI = componentsInChild.GetComponent<TextMeshProUGUI>();
						textMeshProUGUI.text = "MünchenClient Nameplate";
					}
					if (Configuration.GetGeneralConfig().NameplateMoreInfo)
					{
                        CreatedPlate.SetActive(value: true);
					}
					else
					{
                        CreatedPlate.SetActive(value: false);
					}
				}
                if (Configuration.GetGeneralConfig().NameplateWallhack && CameraFeaturesHandler.GetCameraSetup() == 0)
				{
                    canvas.layer = 19;
				}
			}
			catch (Exception e)
			{
				ConsoleUtils.Exception("PatchManager", "OnPlayerJoinPatch - Nameplate", e, "OnPlayerJoinPatch", 146);
			}
			PlayerInformation playerInformation;
			try
			{
				playerInformation = new PlayerInformation
				{
					actorId = ((VRCNetworkBehaviour)__0.prop_VRCPlayer_0).prop_Int32_1,
					actorIdData = ((VRCNetworkBehaviour)__0.prop_VRCPlayer_0).prop_Int32_0 * PhotonNetwork.field_Public_Static_Int32_0 + 1,
					actorIdDataOther = ((VRCNetworkBehaviour)__0.prop_VRCPlayer_0).prop_Int32_0 * PhotonNetwork.field_Public_Static_Int32_0 + 3,
					id = __0.prop_APIUser_0.id,
					displayName = __0.prop_APIUser_0.displayName,
					isLocalPlayer = flag,
					isInstanceMaster = __0.prop_VRCPlayerApi_0.isMaster,
					isVRChatStaff = false,
					isVRUser = __0.prop_VRCPlayerApi_0.IsUserInVR(),
					isQuestUser = (__0.prop_APIUser_0.last_platform != "standalonewindows"),
					isClientUser = false,
					blockedLocalPlayer = false,
					rankStatus = PlayerRankStatus.Unknown,
					player = __0,
					playerApi = __0.prop_VRCPlayerApi_0,
					vrcPlayer = __0.prop_VRCPlayer_0,
					apiUser = __0.prop_APIUser_0,
					networkBehaviour = __0.prop_VRCPlayer_0,
					uSpeaker = __0.prop_VRCPlayer_0.prop_USpeaker_0,
					input = __0.prop_VRCPlayer_0.GetComponent<GamelikeInputController>(),
					airstuckDetections = 0,
					lastNetworkedUpdatePacketNumber = __0.prop_PlayerNet_0.field_Private_Int32_0,
					lastNetworkedUpdateTime = Time.realtimeSinceStartup,
					lastNetworkedVoicePacket = 0f,
					lagBarrier = 0,
					nameplateCanvas = canvas,
					nameplateBackground = nameplateBackground,
					nameplateIconBackground = nameplateIconBackground,
					customNameplateObject = CreatedPlate,
					customNameplateTransform = rectTransform,
					customNameplateText = textMeshProUGUI,
					RankText = RetreiveRank(__0),
					RankColor = RetreiveRankColor(__0),
					RankMesh = trustText,
                    RankIcon = canvas.transform.Find("NameplateGroup/Nameplate/Contents/Quick Stats/Trust Icon").GetComponent<Image>(),
                    PlateName = canvas.transform.Find("NameplateGroup/Nameplate/Contents/Main/Text Container/Name").GetComponent<TextMeshProUGUI>()
                };
			}
			catch (Exception e2)
			{
				ConsoleUtils.Exception("PatchManager", "OnPlayerJoinPatch - Pre Process", e2, "OnPlayerJoinPatch", 195);
				return;
			}
			try
			{
				if (!PlayerUtils.playerColorCache.ContainsKey(__0.prop_APIUser_0.displayName))
				{
					PlayerUtils.playerColorCache.Add(__0.prop_APIUser_0.displayName, VRCPlayer.Method_Public_Static_Color_APIUser_0(__0.prop_APIUser_0));
				}
				else
				{
					PlayerUtils.playerColorCache[__0.prop_APIUser_0.displayName] = VRCPlayer.Method_Public_Static_Color_APIUser_0(__0.prop_APIUser_0);
				}
				PlayerUtils.playerCachingList.Add(playerInformation.displayName, playerInformation);
			}
			catch (Exception e3)
			{
				ConsoleUtils.Exception("PatchManager", "OnPlayerJoinPatch - Post Process", e3, "OnPlayerJoinPatch", 215);
				return;
			}
			ModuleManager.OnPlayerJoin(playerInformation);
			try
			{
				if (__0.prop_APIUser_0.tags.Contains("admin_moderator") || __0.prop_APIUser_0.developerType == APIUser.DeveloperType.Internal || __0.prop_APIUser_0.developerType == APIUser.DeveloperType.Moderator)
				{
					playerInformation.isVRChatStaff = true;
					if (Configuration.GetModerationsConfig().LogModerationsWarnAboutModerators)
					{
						string text = "<color=red>WARNING: <color=white>VRChat Staff joined: <color=purple>" + playerInformation.displayName;
						GeneralUtils.InformHudText(LanguageManager.GetUsedLanguage().ProtectionsMenuName, text);
						ConsoleUtils.Info(LanguageManager.GetUsedLanguage().ProtectionsMenuName, text, ConsoleColor.Red, "OnPlayerJoinPatch", 235);
					}
				}
			}
			catch (Exception e4)
			{
				ConsoleUtils.Exception("PatchManager", "OnPlayerJoinPatch - Staffcheck", e4, "OnPlayerJoinPatch", 241);
			}
		}
		#endregion

		private static void OnPlayerLeavePatch(ref VRC.Player __0)
		{
			if (__0 == null)
			{
				return;
			}
			PlayerInformation playerInformation = PlayerWrappers.GetPlayerInformation(__0);
			if (playerInformation != null && PlayerUtils.playerCachingList.ContainsKey(playerInformation.displayName))
			{
				if (GeneralUtils.notificationTracker.ContainsKey(__0.prop_APIUser_0.displayName))
				{
					GeneralUtils.notificationTracker.Remove(__0.prop_APIUser_0.displayName);
				}
				ModuleManager.OnPlayerLeft(playerInformation);
				PlayerUtils.playerCachingList.Remove(playerInformation.displayName);
			}
		}
	}
}
