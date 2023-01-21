using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ActionMenuAPI;
using Il2CppSystem.Collections;
using MelonLoader;
using MunchenClient.Config;
using MunchenClient.Core;
using MunchenClient.Menu.Player;
using MunchenClient.Menu.Protections;
using MunchenClient.Misc;
using MunchenClient.ModuleSystem.Modules;
using MunchenClient.Utils;
using MunchenClient.Wrappers;
using UnhollowerBaseLib;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using IEnumerator = System.Collections.IEnumerator;

namespace MunchenClient.Menu.Others
{
    internal class ActionWheelMenu
    {
        #region Statics

        internal static CustomActionMenu.ActionMenuPage clientPage;

        internal static CustomActionMenu.ActionMenuButton flightButton;

        internal static CustomActionMenu.ActionMenuButton serializationButton;

        internal static CustomActionMenu.ActionMenuPage worldPage;
        internal static CustomActionMenu.ActionMenuButton GoHomeButton;
        internal static CustomActionMenu.ActionMenuButton GoLastWorldButton;
        internal static CustomActionMenu.ActionMenuButton HideItemsButton;
        internal static bool HideItems;
        internal static CustomActionMenu.ActionMenuButton CopyInstanceIDButton;
        internal static CustomActionMenu.ActionMenuButton GotoCopiedIDButton;
        internal static CustomActionMenu.ActionMenuPage MurderPage;

        internal static CustomActionMenu.ActionMenuPage ESPPage;
        internal static CustomActionMenu.ActionMenuButton PlayerwallhackButton;
        internal static CustomActionMenu.ActionMenuButton ItemwallhackButton;
        internal static bool ItemWallHack;
        internal static CustomActionMenu.ActionMenuButton PlayerNameplateESP;
        internal static bool NameplateWallHack;
        internal static CustomActionMenu.ActionMenuButton TriggerWallhackButton;
        internal static bool TriggerWallHack;

        internal static CustomActionMenu.ActionMenuPage monkePage;
        internal static CustomActionMenu.ActionMenuButton earrapeButton;
        internal static CustomActionMenu.ActionMenuButton colliderHiderButton;
        internal static CustomActionMenu.ActionMenuButton itemPooferButton;
        internal static CustomActionMenu.ActionMenuButton freezeItemsButton;
        internal static bool ItemsFrozen;
        internal static CustomActionMenu.ActionMenuButton HideSelfButton;
        internal static bool Hiden;
        internal static CustomActionMenu.ActionMenuButton CrashWithID;


        internal static CustomActionMenu.ActionMenuPage otherPage;
        internal static CustomActionMenu.ActionMenuButton portalButton;
        internal static bool Nameplates;
        internal static CustomActionMenu.ActionMenuButton NameplateInfo;
        internal static CustomActionMenu.ActionMenuButton ClippingDistance;
        internal static bool Clipping;
        internal static CustomActionMenu.ActionMenuButton AvatarDownloadLogs;
        internal static bool AviLog;

        internal static CustomActionMenu.ActionMenuButton ExceptionLogs;
        internal static bool ExcLog;


        internal static CustomActionMenu.ActionMenuPage nasaPage;

        internal static CustomActionMenu.ActionMenuPage MediaPage;
        internal static CustomActionMenu.ActionMenuButton mediaPlay;
        internal static CustomActionMenu.ActionMenuButton mediaNext;
        internal static CustomActionMenu.ActionMenuButton mediaLast;
        internal static CustomActionMenu.ActionMenuButton mediaMute;
        internal static CustomActionMenu.ActionMenuButton mediaVolUp;
        internal static CustomActionMenu.ActionMenuButton mediaVolDown;

        internal static CustomActionMenu.ActionMenuButton TeleportToRaycastButton;

        #endregion

        public static class Freeze
        {
            public static IEnumerator FreezeItems()
            {
                VRC_Pickup[] array = Resources.FindObjectsOfTypeAll<VRC_Pickup>();
                while (true)
                {
                    if (ItemsFrozen)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            Networking.SetOwner(PlayerWrappers.GetCurrentPlayer().prop_VRCPlayerApi_0, array[i].gameObject);
                        }

                        yield return new WaitForSeconds(.5f);
                    }

                    yield return new WaitForSeconds(.5f);
                }
            }
        }

        internal ActionWheelMenu()
        {
            clientPage = new CustomActionMenu.ActionMenuPage(CustomActionMenu.ActionMenuBaseMenu.MainMenu, "München", AssetLoader.LoadTexture("MunchenClientLogo"));

            flightButton = new CustomActionMenu.ActionMenuButton(clientPage, "Flight: <color=red>Off", delegate
            {
                GeneralUtils.ToggleFlight(!GeneralUtils.flight);
                MovementMenu.flightButton.SetToggleState(GeneralUtils.flight);
                flightButton.SetButtonText(GeneralUtils.flight ? "Flight: <color=green>On" : "Flight: <color=red>Off");
            }, AssetLoader.LoadTexture("FlightIcon"));

            #region ESP Page //Done

            ESPPage = new CustomActionMenu.ActionMenuPage(clientPage, "ESP Menu", AssetLoader.LoadTexture("WallhackIcon"));

            PlayerwallhackButton = new CustomActionMenu.ActionMenuButton(ESPPage, "Player Wallhack: <color=red>Off", delegate
            {
                Configuration.GetGeneralConfig().PlayerWallhack = !Configuration.GetGeneralConfig().PlayerWallhack;
                Configuration.SaveGeneralConfig();
                GeneralWrappers.ApplyAllPlayerWallhack(Configuration.GetGeneralConfig().PlayerWallhack);
                PlayerMenu.wallhackButton.SetToggleState(Configuration.GetGeneralConfig().PlayerWallhack);
                PlayerwallhackButton.SetButtonText(Configuration.GetGeneralConfig().PlayerWallhack ? "Player Wallhack: <color=green>On" : "Player Wallhack: <color=red>Off");
            }, AssetLoader.LoadTexture("WallhackIcon"));
            PlayerwallhackButton.SetButtonText(Configuration.GetGeneralConfig().PlayerWallhack ? "Player Wallhack: <color=green>On" : "Player Wallhack: <color=red>Off");

            ItemwallhackButton = new CustomActionMenu.ActionMenuButton(ESPPage, "Item Wallhack: <color=red>Off", delegate
            {
                Configuration.GetGeneralConfig().ItemWallhack = !Configuration.GetGeneralConfig().ItemWallhack;
                Configuration.SaveGeneralConfig();
                if (!ItemWallHack)
                {
                    ItemWallHack = true;
                    GeneralHandler.pickupRenderersToHighlight.Clear();
                    VRC_Pickup[] array4 = Resources.FindObjectsOfTypeAll<VRC_Pickup>();
                    for (int m = 0; m < array4.Length; m++)
                    {
                        if (!(array4[m].gameObject.name == "ViewFinder") && !(array4[m].gameObject.name == "PhotoCamera"))
                        {
                            System.Collections.Generic.List<Renderer> list4 = MiscUtils.FindAllComponentsInGameObject<Renderer>(array4[m].gameObject, includeInactive: false, searchParent: false);
                            for (int n = 0; n < list4.Count; n++)
                            {
                                if (!(list4[n] == null))
                                {
                                    GeneralWrappers.EnableOutline(list4[n], state: true);
                                    GeneralHandler.pickupRenderersToHighlight.Add(list4[n]);
                                }
                            }
                        }
                    }
                }
                else
                
                {
                    ItemWallHack = false;
                    GeneralHandler.pickupRenderersToHighlight.Clear();
                    VRC_Pickup[] array3 = Resources.FindObjectsOfTypeAll<VRC_Pickup>();
                    for (int k = 0; k < array3.Length; k++)
                    {
                        if (!(array3[k] == null) && !(array3[k].gameObject.name == "ViewFinder") && !(array3[k].gameObject.name == "PhotoCamera"))
                        {
                            System.Collections.Generic.List<Renderer> list3 = MiscUtils.FindAllComponentsInGameObject<Renderer>(array3[k].gameObject, includeInactive: false, searchParent: false);
                            for (int l = 0; l < list3.Count; l++)
                            {
                                if (!(list3[l] == null))
                                {
                                    GeneralWrappers.EnableOutline(list3[l], state: false);
                                }
                            }
                        }
                    }
                }

                WorldMenu.ItemWallhackButton.SetToggleState(Configuration.GetGeneralConfig().ItemWallhack);
                ItemwallhackButton.SetButtonText(Configuration.GetGeneralConfig().ItemWallhack ? "Item Wallhack: <color=green>On" : "Item Wallhack: <color=red>Off");
            }, AssetLoader.LoadTexture("WallhackIcon"));
            ItemwallhackButton.SetButtonText(Configuration.GetGeneralConfig().ItemWallhack ? "Item Wallhack: <color=green>On" : "Item Wallhack: <color=red>Off");

            PlayerNameplateESP = new CustomActionMenu.ActionMenuButton(ESPPage, "Item Wallhack: <color=red>Off", delegate
            {
                Configuration.GetGeneralConfig().NameplateWallhack = !Configuration.GetGeneralConfig().NameplateWallhack;
                Configuration.SaveGeneralConfig();
                if (!NameplateWallHack)
                {
                    NameplateWallHack = true;
                    GeneralUtils.SetNameplateWallhack(state: true);
                }
                else
                {
                    NameplateWallHack = false;
                    GeneralUtils.SetNameplateWallhack(state: false);
                }

                NameplateMenu.nameplatewallhackButton.SetToggleState(Configuration.GetGeneralConfig().NameplateWallhack);
                PlayerNameplateESP.SetButtonText(Configuration.GetGeneralConfig().NameplateWallhack ? "Nameplate ESP: <color=green>On" : "Nameplate ESP: <color=red>Off");
            }, AssetLoader.LoadTexture("WallhackIcon"));
            PlayerNameplateESP.SetButtonText(Configuration.GetGeneralConfig().NameplateWallhack ? "Nameplate ESP: <color=green>On" : "Nameplate ESP: <color=red>Off");

            TriggerWallhackButton = new CustomActionMenu.ActionMenuButton(ESPPage, "Item Wallhack: <color=red>Off", delegate
            {
                Configuration.GetGeneralConfig().TriggerWallhack = !Configuration.GetGeneralConfig().TriggerWallhack;
                Configuration.SaveGeneralConfig();
                if (!TriggerWallHack)
                {
                    TriggerWallHack = true;
                    VRC_Trigger[] array2 = Resources.FindObjectsOfTypeAll<VRC_Trigger>();
                    for (int j = 0; j < array2.Length; j++)
                    {
                        System.Collections.Generic.List<Renderer> list2 = MiscUtils.FindAllComponentsInGameObject<Renderer>(array2[j].gameObject, includeInactive: false, searchParent: false);
                        if (list2.Count != 0 && !(list2[0] == null))
                        {
                            GeneralWrappers.EnableOutline(list2[0], state: true);
                        }
                    }
                }
                else
                {
                    TriggerWallHack = false;
                    VRC_Trigger[] array = Resources.FindObjectsOfTypeAll<VRC_Trigger>();
                    for (int i = 0; i < array.Length; i++)
                    {
                        System.Collections.Generic.List<Renderer> list = MiscUtils.FindAllComponentsInGameObject<Renderer>(array[i].gameObject, includeInactive: false, searchParent: false);
                        if (list.Count != 0 && !(list[0] == null))
                        {
                            GeneralWrappers.EnableOutline(list[0], state: false);
                        }
                    }
                }

                WorldMenu.TriggerWallhackButton.SetToggleState(Configuration.GetGeneralConfig().TriggerWallhack);
                TriggerWallhackButton.SetButtonText(Configuration.GetGeneralConfig().TriggerWallhack ? "Trigger Wallhack: <color=green>On" : "Trigger Wallhack: <color=red>Off");
            }, AssetLoader.LoadTexture("WallhackIcon"));
            TriggerWallhackButton.SetButtonText(Configuration.GetGeneralConfig().TriggerWallhack ? "Trigger Wallhack: <color=green>On" : "Trigger Wallhack: <color=red>Off");

            #endregion

            #region Monke Page

            monkePage = new CustomActionMenu.ActionMenuPage(clientPage, "Monkey Menu", AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Monkey.png").texture);

            earrapeButton = new CustomActionMenu.ActionMenuButton(monkePage, "Earrape Mic: <color=red>Off", delegate
            {
                if (PlayerHandler.GetPlayerVolume() > 1.70141173E+38f)
                {
                    PlayerHandler.SetPlayerVolume(1f);
                    earrapeButton.SetButtonText("Earrape Mic: <color=red>Off");
                }
                else
                {
                    PlayerHandler.SetPlayerVolume(float.MaxValue);
                    earrapeButton.SetButtonText("Earrape Mic: <color=green>On");
                }
            }, AssetLoader.LoadTexture("EarrapeIcon"));

            colliderHiderButton = new CustomActionMenu.ActionMenuButton(monkePage, "Collider Hider: <color=red>Off", delegate
            {
                GeneralUtils.capsuleHider = !GeneralUtils.capsuleHider;
                PlayerUtils.ChangeCapsuleState(GeneralUtils.capsuleHider);
                FunMenu.capsuleHiderButton.SetToggleState(GeneralUtils.capsuleHider);
                colliderHiderButton.SetButtonText(GeneralUtils.capsuleHider ? "Collider Hider: <color=green>On" : "Collider Hider: <color=red>Off");
            }, AssetLoader.LoadTexture("ColliderHider"));

            itemPooferButton = new CustomActionMenu.ActionMenuButton(monkePage, "Respawn Items", delegate
            {
                try
                {
                    Il2CppArrayBase<VRC_Pickup> il2CppArrayBase = Resources.FindObjectsOfTypeAll<VRC_Pickup>();
                    foreach (VRC_Pickup vrc_Pickup in il2CppArrayBase)
                    {
                        bool Object = !(vrc_Pickup == null) && !(vrc_Pickup.gameObject == null) && vrc_Pickup.gameObject.active && vrc_Pickup.enabled && vrc_Pickup.pickupable && !vrc_Pickup.name.Contains("ViewFinder") && !(HighlightsFX.prop_HighlightsFX_0 == null);
                        if (Object)
                        {
                            vrc_Pickup.transform.position = new Vector3(0f, -50f, 0f);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Pancake.png").texture);

            freezeItemsButton = new CustomActionMenu.ActionMenuButton(monkePage, "Freeze Items: <color=red>Off", delegate
            {
                if (!ItemsFrozen)
                {
                    try
                    {
                        MelonCoroutines.Start(Freeze.FreezeItems());
                    }
                    catch
                    {
                    }

                    ItemsFrozen = true;
                }
                else
                {
                    ItemsFrozen = false;
                }

                freezeItemsButton.SetButtonText(ItemsFrozen ? "Freeze Items: <color=green>On" : "Freeze Items: <color=red>Off");
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Ice.png").texture);

            HideSelfButton = new CustomActionMenu.ActionMenuButton(monkePage, "Hide Self: <color=red>Off", delegate
            {
                if (!GeneralUtils.hideSelf)
                    GeneralUtils.ChangeHideSelfState(state: true);
                else
                {
                    GeneralUtils.ChangeHideSelfState(state: false);
                }

                HideSelfButton.SetButtonText(GeneralUtils.hideSelf ? "Hide Self: <color=green>On" : "Hide Self: <color=red>Off");
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\HideSelf.png").texture);

            CrashWithID = new CustomActionMenu.ActionMenuButton(monkePage, "Crash world with ID", delegate
            {
                MelonCoroutines.Start(GeneralUtils.GameCloseExploitEnumerator(false, null));
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Crash.png").texture);


            #region TODO MONKE

            //free cam
            //serialization
            /*
			portalButton = new CustomActionMenu.ActionMenuButton(clientPage, "Delete Portals", delegate //portals need fixed :3
			{
				PortalsMenu.DeleteAllPortals(informHud: true);
			}, AssetLoader.LoadTexture("PortalIcon"));
			*/
            /* 
			 serializationButton = new CustomActionMenu.ActionMenuButton(clientPage, "Serialization: <color=red>Off", delegate
			{
				GeneralUtils.serialization = !GeneralUtils.serialization;
				serializationButton.SetButtonText(GeneralUtils.serialization ? "Serialization: <color=green>On" : "Serialization: <color=red>Off");
				if (GeneralUtils.serialization)
				{
					GeneralUtils.fakelag = false;
					PhotonExploitsMenu.fakeLagButton.SetToggleState(state: false);
				}
			}, AssetLoader.LoadTexture("SerializationIcon"));
			*/

            #endregion

            #endregion

            TeleportToRaycastButton = new CustomActionMenu.ActionMenuButton(clientPage, "Teleport to raycast", delegate
            {
                Transform transform = Camera.main.transform;
                Transform transform2 = transform.transform;
                Ray ray = new Ray(transform2.position, transform2.forward);
                RaycastHit[] array = Physics.RaycastAll(ray);
                bool flag3 = array.Length != 0;
                if (flag3)
                {
                    RaycastHit raycastHit = array[0];
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = raycastHit.point;
                }
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Target.png").texture);

            #region World Page

            worldPage = new CustomActionMenu.ActionMenuPage(clientPage, "World Menu", AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\World.png").texture);

            #region Murder Page

            //MurderPage = new CustomActionMenu.ActionMenuPage(worldPage, "Murder Menu", AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Knife.png").texture);

            //force end  
            /*
             foreach (UdonSync item3 in Resources.FindObjectsOfTypeAll<UdonSync>())
				{
					if (item3.gameObject.name == "Game Logic")
					{
						Networking.RPC(RPC.Destination.All, item3.gameObject, "UdonSyncRunProgramAsRPC", GeneralUtils.fastRandom.NextBool() ? syncVictoryBystanderParameters : syncVictoryMurdererParameters);
						break;
					}
				}
             */

            //Block Udon
            /*
            Configuration.GetGeneralConfig().BlockAllUdonEvents = true;
            Configuration.GetGeneralConfig().BlockAllUdonEvents = false;
            */

            //udon Godmode
            /*
			Configuration.GetGeneralConfig().UdonGodmode = true;
			Configuration.GetGeneralConfig().UdonGodmode = false;
             */

            //blind all
            /*
             UdonExploitManager.udonsend("OnLocalPlayerFlashbanged", "everyone");
            UdonExploitManager.udonsend("OnLocalPlayerBlinded", "everyone");
             */

            //kill all
            /*
             currentSkippableSyncKills = 0;
				foreach (UdonSync item in Resources.FindObjectsOfTypeAll<UdonSync>())
				{
					if (item.gameObject.name.Contains("Player Node"))
					{
						currentSkippableSyncKills++;
						Networking.RPC(RPC.Destination.All, item.gameObject, "UdonSyncRunProgramAsRPC", syncKillParameters);
					}
				}
            */
            //All weapons pickupable
            /*
            Configuration.GetGeneralConfig().MurderForceWeaponPickupable = true;
			Configuration.GetGeneralConfig().MurderForceWeaponPickupable = false;
        
             */

            #region murderTODO

            //kill murderer 
            //kill detective

            #endregion

            #endregion

            GoHomeButton = new CustomActionMenu.ActionMenuButton(worldPage, "Go Home", delegate { WorldUtils.GoToWorld(GeneralHandler.HomeWorldID); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Home.png").texture);
            HideItemsButton = new CustomActionMenu.ActionMenuButton(worldPage, "Hide Items <color=red> off", delegate
            {
                if (HideItems)
                {
                    HideItems = false;
                }
                else
                {
                    HideItems = true;
                }

                HideItemsButton.SetButtonText(HideItems ? "Hide Items: <color=green>On" : "Hide Items: <color=red>Off");
                VRC_Pickup[] D = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
                for (int i = 0; i < D.Length; i++)
                {
                    bool L = D[i].gameObject.layer == 13;
                    if (L)
                    {
                        D[i].gameObject.SetActive(HideItems);
                    }
                }

                VRC_Pickup[] Y = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
                for (int j = 0; j < Y.Length; j++)
                {
                    bool E = Y[j].gameObject.layer == 13;
                    if (E)
                    {
                        Y[j].gameObject.SetActive(HideItems);
                    }
                }

                VRCPickup[] C = Resources.FindObjectsOfTypeAll<VRCPickup>().ToArray<VRCPickup>();
                for (int k = 0; k < C.Length; k++)
                {
                    bool G = C[k].gameObject.layer == 13;
                    if (G)
                    {
                        C[k].gameObject.SetActive(HideItems);
                    }
                }
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Pancake.png").texture);
            GoLastWorldButton = new CustomActionMenu.ActionMenuButton(worldPage, "Go to last world", delegate
            {
                string lastworld = null;
                int index = Configuration.GetInstanceHistoryConfig().InstanceHistory.Count - 2;
                SavedInstance savedInstance = Configuration.GetInstanceHistoryConfig().InstanceHistory[index];
                WorldUtils.GoToWorld(savedInstance.ID + ":" + savedInstance.Tags);
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\LeftArrow.png").texture);
            CopyInstanceIDButton = new CustomActionMenu.ActionMenuButton(worldPage, "Copy WID", delegate { Clipboard.SetText(WorldUtils.GetCurrentWorld().id + ":" + WorldUtils.GetCurrentInstance().instanceId); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Clip.png").texture);
            GotoCopiedIDButton = new CustomActionMenu.ActionMenuButton(worldPage, "Go to Copied WID", delegate { WorldUtils.GoToWorld(Clipboard.GetText()); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\RightArrow.png").texture);

            #endregion

            #region Other Page

            otherPage = new CustomActionMenu.ActionMenuPage(clientPage, "Other Menu", AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Cat.png").texture);

            #region NasaTech Page

            nasaPage = new CustomActionMenu.ActionMenuPage(otherPage, "Nasa Technologies", AssetLoader.LoadTexture("Quack"));
            CustomActionMenu.ActionMenuButton nasa1 = new CustomActionMenu.ActionMenuButton(nasaPage, "Quack!", delegate
            {
                NetworkedEmotesMenu.totalQuacks++;
                GeneralUtils.InformHudText("Lmao", $"Quack! ({NetworkedEmotesMenu.totalQuacks})");
                AudioUtils.PlayAudioClip(AssetLoader.LoadAudio("Quack"));
            }, AssetLoader.LoadTexture("Quack"));
            CustomActionMenu.ActionMenuButton nasa2 = new CustomActionMenu.ActionMenuButton(nasaPage, "Sizzukie", delegate
            {
                NetworkedEmotesMenu.totalSizzs++;
                GeneralUtils.InformHudText("Lmao", $"Exception ({NetworkedEmotesMenu.totalSizzs})");
                AudioUtils.PlayAudioClip(AssetLoader.LoadAudio("Sizzukie"));
            }, AssetLoader.LoadTexture("Sizzukie"));
            CustomActionMenu.ActionMenuButton nasa3 = new CustomActionMenu.ActionMenuButton(nasaPage, "QgIsGay", delegate
            {
                NetworkedEmotesMenu.totalQgs++;
                GeneralUtils.InformHudText("Lmao", $"Qg Is Gay ({NetworkedEmotesMenu.totalQgs})");
                AudioUtils.PlayAudioClip(AssetLoader.LoadAudio("QgIsGay"));
            }, AssetLoader.LoadTexture("QgIsGay"));
            CustomActionMenu.ActionMenuButton nasa4 = new CustomActionMenu.ActionMenuButton(nasaPage, "Niggers", delegate
            {
                NetworkedEmotesMenu.totalCowboys++;
                GeneralUtils.InformHudText("Lmao", $"Cowboy KenKen ({NetworkedEmotesMenu.totalCowboys})");
                AudioUtils.PlayAudioClip(AssetLoader.LoadAudio("Niggers"));
            }, AssetLoader.LoadTexture("Niggers"));
            CustomActionMenu.ActionMenuButton nasa5 = new CustomActionMenu.ActionMenuButton(nasaPage, "Allah", delegate
            {
                NetworkedEmotesMenu.totalAllahs++;
                GeneralUtils.InformHudText("Lmao", $"Allah Cat ({NetworkedEmotesMenu.totalAllahs})");
                AudioUtils.PlayAudioClip(AssetLoader.LoadAudio("Allah"));
            }, AssetLoader.LoadTexture("Allah"));
            CustomActionMenu.ActionMenuButton nasa6 = new CustomActionMenu.ActionMenuButton(nasaPage, "Gay", delegate
            {
                NetworkedEmotesMenu.totalGays++;
                GeneralUtils.InformHudText("Lmao", $"Gay ({NetworkedEmotesMenu.totalGays})");
                AudioUtils.PlayAudioClip(AssetLoader.LoadAudio("GayEcho"));
            }, AssetLoader.LoadTexture("OkayChamp"));

            #endregion

            CustomActionMenu.ActionMenuButton ClearVRAM = new CustomActionMenu.ActionMenuButton(otherPage, "Clear VRAM", delegate
            {
                PerformanceProfiler.StartProfiling("ClearVRAM");
                GeneralUtils.ClearVRAM();
                PerformanceProfiler.EndProfiling("ClearVRAM");
                string text = LanguageManager.GetUsedLanguage().ClearVRAMClicked.Replace("{TimeToClear}", string.Format("{0:F2}", PerformanceProfiler.GetProfiling("ClearVRAM")));
                ConsoleUtils.Info("Performance", text, ConsoleColor.Gray, ".ctor", 60);
                UserInterface.WriteHudMessage(text);
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\VRAM.png").texture);
            NameplateInfo = new CustomActionMenu.ActionMenuButton(otherPage, "More Nameplate Info", delegate
            {
                if (!Nameplates)
                {
                    Configuration.GetGeneralConfig().NameplateMoreInfo = true;
                    Configuration.SaveGeneralConfig();
                    foreach (KeyValuePair<string, PlayerInformation> playerCaching in PlayerUtils.playerCachingList)
                    {
                        if (!playerCaching.Value.isLocalPlayer)
                        {
                            playerCaching.Value.customNameplateObject.SetActive(value: true);
                        }
                    }

                    Nameplates = true;
                }
                else
                {
                    Configuration.GetGeneralConfig().NameplateMoreInfo = false;
                    Configuration.SaveGeneralConfig();
                    foreach (KeyValuePair<string, PlayerInformation> playerCaching2 in PlayerUtils.playerCachingList)
                    {
                        if (!playerCaching2.Value.isLocalPlayer)
                        {
                            playerCaching2.Value.customNameplateObject.SetActive(value: false);
                        }
                    }

                    Nameplates = false;
                }

                NameplateInfo.SetButtonText(Nameplates ? "More Nameplate Info: <color=green>On" : "More Nameplate Info: <color=red>Off");
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Minus.png").texture);
            ClippingDistance = new CustomActionMenu.ActionMenuButton(otherPage, "Set Clipping Distance", delegate
            {
                if (!Clipping)
                {
                    Configuration.GetGeneralConfig().MinimumCameraClippingDistance = true;
                    Configuration.SaveGeneralConfig();
                    Clipping = true;
                }
                else
                {
                    Configuration.GetGeneralConfig().MinimumCameraClippingDistance = true;
                    Configuration.SaveGeneralConfig();
                    Clipping = false;
                }

                ClippingDistance.SetButtonText(Clipping ? "Near Clip: <color=green>On" : "Near Clip: <color=red>Off");
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\HideSelf.png").texture);
            AvatarDownloadLogs = new CustomActionMenu.ActionMenuButton(otherPage, "Avatar Logs", delegate
            {
                if (!AviLog)
                {
                    Configuration.GetGeneralConfig().AvatarDownloadLogging = true;
                    Configuration.SaveGeneralConfig();
                    AviLog = true;
                }
                else
                {
                    Configuration.GetGeneralConfig().AvatarDownloadLogging = false;
                    Configuration.SaveGeneralConfig();
                    AviLog = false;
                }

                AvatarDownloadLogs.SetButtonText(AviLog ? "Avatar Logging: <color=green>On" : "Avatar Logging: <color=red>Off");
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Log.png").texture);
            ExceptionLogs = new CustomActionMenu.ActionMenuButton(otherPage, "Exception Logs", delegate
            {
                if (!ExcLog)
                {
                    Configuration.GetGeneralConfig().ExceptionLogging = true;
                    Configuration.SaveGeneralConfig();
                    ExcLog = true;
                }
                else
                {
                    Configuration.GetGeneralConfig().ExceptionLogging = false;
                    Configuration.SaveGeneralConfig();
                    ExcLog = false;
                }

                ExceptionLogs.SetButtonText(ExcLog ? "Exception Logs: <color=green>On" : "Exception Logs: <color=red>Off");
            }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Log.png").texture);

            #region TODO

            //flashlight

            #endregion

            #endregion

            #region Media Page //Done

            MediaPage = new CustomActionMenu.ActionMenuPage(clientPage, "Media Controls", AssetLoader.LoadTexture("MediaIcon"));
            mediaVolUp = new CustomActionMenu.ActionMenuButton(MediaPage, "Volume Up", delegate { UnmanagedUtils.VolumeUp(); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Plus.png").texture);
            mediaNext = new CustomActionMenu.ActionMenuButton(MediaPage, "Next Song", delegate { UnmanagedUtils.Next(); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\RightArrow.png").texture);
            mediaVolDown = new CustomActionMenu.ActionMenuButton(MediaPage, "Volume Down", delegate { UnmanagedUtils.VolumeDown(); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Minus.png").texture);
            mediaMute = new CustomActionMenu.ActionMenuButton(MediaPage, "(Un)Mute Media", delegate { UnmanagedUtils.MuteOrUnmute(); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Mute.png").texture);
            mediaLast = new CustomActionMenu.ActionMenuButton(MediaPage, "Previous Song", delegate { UnmanagedUtils.Previous(); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\LeftArrow.png").texture);
            mediaPlay = new CustomActionMenu.ActionMenuButton(MediaPage, "(Un)Pause Media", delegate { UnmanagedUtils.PlayOrPause(); }, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\PlayPause.png").texture);

            #endregion
        }
    }
}