using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Il2CppSystem.Collections.Generic;
using MunchenClient.Config;
using MunchenClient.Core;
using MunchenClient.Menu.Others;
using MunchenClient.Menu.World;
using MunchenClient.Misc;
using MunchenClient.ModuleSystem.Modules;
using MunchenClient.Utils;
using MunchenClient.Wrappers;
using UnchainedButtonAPI;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace MunchenClient.Menu
{
	internal class WorldMenu : QuickMenuNestedMenu
	{
		internal static QuickMenuToggleButton ItemWallhackButton;
		internal static QuickMenuToggleButton TriggerWallhackButton;
        internal WorldMenu(QuickMenuButtonRow parent)
			: base(parent, LanguageManager.GetUsedLanguage().WorldMenuName, LanguageManager.GetUsedLanguage().WorldMenuDescription)
		{
			QuickMenuButtonRow quickMenuButtonRow = new QuickMenuButtonRow(this);
			new QuickMenuSpacers(this);
			QuickMenuButtonRow parentRow = new QuickMenuButtonRow(this);
			new InstanceHistoryMenu(quickMenuButtonRow);
			new WorldTweaksMenu(quickMenuButtonRow);

			new QuickMenuSingleButton(quickMenuButtonRow, LanguageManager.GetUsedLanguage().JoinByID, delegate
			{
				//GeneralWrappers.ShowInputPopup(LanguageManager.GetUsedLanguage().JoinByID, string.Empty, InputField.InputType.Standard, isNumeric: false, LanguageManager.GetUsedLanguage().ConfirmText, WorldUtils.GoToWorld);
				WorldUtils.GoToWorld(Clipboard.GetText());

			}, LanguageManager.GetUsedLanguage().JoinByIDDescription);

			new QuickMenuSingleButton(quickMenuButtonRow, LanguageManager.GetUsedLanguage().CopyInstanceID, delegate
			{
				Clipboard.SetText(WorldUtils.GetCurrentWorld().id + ":" + WorldUtils.GetCurrentInstance().instanceId);
                UserInterface.WriteHudMessage(LanguageManager.GetUsedLanguage().CopyInstanceIDClicked);
			}, LanguageManager.GetUsedLanguage().CopyInstanceIDDescription);

			//new QuickMenuSingleButton(parentRow, "Download World File", delegate { GeneralUtils.DownloadFileToPath(WorldUtils.GetCurrentWorld().assetUrl, "Worlds", WorldUtils.GetCurrentWorld().name + "-" + WorldUtils.GetCurrentWorld().id, "vrcw", OnWorldDownloadFinished); QuickMenuUtils.ShowAlert("Started downloading world"); }, "Downloads the world file onto your disk");

			new QuickMenuSingleButton(parentRow, "Drop Portal", delegate
			{
				GeneralWrappers.ShowInputPopup("Drop portal to instance", string.Empty, InputField.InputType.Standard, isNumeric: false, LanguageManager.GetUsedLanguage().ConfirmText, DropPortalToInstance, null, "Enter instance id...");
			}, "Drop a portal to a specific instance");

            ItemWallhackButton = new QuickMenuToggleButton(parentRow, LanguageManager.GetUsedLanguage().ItemWallhack, Configuration.GetGeneralConfig().ItemWallhack, delegate
			{
				Configuration.GetGeneralConfig().ItemWallhack = true;
				Configuration.SaveGeneralConfig();
                ActionWheelMenu.ItemwallhackButton.SetButtonText("Item Wallhack: <color=green>On");
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
			}, LanguageManager.GetUsedLanguage().ItemWallhackDescription, delegate
			{
				Configuration.GetGeneralConfig().ItemWallhack = false;
				Configuration.SaveGeneralConfig();
                ActionWheelMenu.ItemwallhackButton.SetButtonText("Item Wallhack: <color=red>Off");
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
			}, LanguageManager.GetUsedLanguage().ItemWallhackDescription);
            TriggerWallhackButton = new QuickMenuToggleButton(parentRow, LanguageManager.GetUsedLanguage().TriggerWallhack, Configuration.GetGeneralConfig().TriggerWallhack, delegate
			{
				Configuration.GetGeneralConfig().TriggerWallhack = true;
				Configuration.SaveGeneralConfig();
                ActionWheelMenu.TriggerWallhackButton.SetButtonText("Trigger Wallhack: <color=green>On");
                VRC_Trigger[] array2 = Resources.FindObjectsOfTypeAll<VRC_Trigger>();
				for (int j = 0; j < array2.Length; j++)
				{
					System.Collections.Generic.List<Renderer> list2 = MiscUtils.FindAllComponentsInGameObject<Renderer>(array2[j].gameObject, includeInactive: false, searchParent: false);
					if (list2.Count != 0 && !(list2[0] == null))
					{
						GeneralWrappers.EnableOutline(list2[0], state: true);
					}
				}
			}, LanguageManager.GetUsedLanguage().TriggerWallhackDescription, delegate
			{
				Configuration.GetGeneralConfig().TriggerWallhack = false;
				Configuration.SaveGeneralConfig();
                ActionWheelMenu.TriggerWallhackButton.SetButtonText("Trigger Wallhack: <color=red>Off");
                VRC_Trigger[] array = Resources.FindObjectsOfTypeAll<VRC_Trigger>();
				for (int i = 0; i < array.Length; i++)
				{
					System.Collections.Generic.List<Renderer> list = MiscUtils.FindAllComponentsInGameObject<Renderer>(array[i].gameObject, includeInactive: false, searchParent: false);
					if (list.Count != 0 && !(list[0] == null))
					{
						GeneralWrappers.EnableOutline(list[0], state: false);
					}
				}
			}, LanguageManager.GetUsedLanguage().TriggerWallhackDescription);
		}

		private static void OnWorldDownloadFinished(bool success)
		{
			if (success)
			{
				ConsoleUtils.Info("Worlds", "Success downloading world", ConsoleColor.Gray, "OnWorldDownloadFinished", 156);
				QuickMenuUtils.ShowAlert("Success downloading world");
			}
			else
			{
				ConsoleUtils.Info("Worlds", "Failed downloading world", ConsoleColor.Gray, "OnWorldDownloadFinished", 161);
				QuickMenuUtils.ShowAlert("Failed downloading world");
			}
		}

		private void DropPortalToInstance(string worldId, Il2CppSystem.Collections.Generic.List<KeyCode> pressedKeys, Text text)
		{
			string[] array = worldId.Split(':');
			WorldUtils.DropPortal(array[0], array[1]);
		}
	}
}
