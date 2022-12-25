using System;
using System.Reflection;
using HarmonyLib;
using MunchenClient.Utils;
using UnhollowerRuntimeLib;
using VRC.UI.Elements;

namespace UnchainedButtonAPI
{
	internal class QuickMenuPatches
	{
		private static HarmonyLib.Harmony harmonyInstance;

		private static bool initializedQuickMenuChanges;

		internal static void InitializeButtonPatches(string clientName)
		{
			try
			{
				harmonyInstance = new HarmonyLib.Harmony("QuickMenuPatches" + clientName); //creates harmony instance
				harmonyInstance.Patch(typeof(VRC.UI.Elements.QuickMenu).GetMethod("OnEnable"), null, new HarmonyMethod(typeof(QuickMenuPatches).GetMethod("QuickMenuOnEnablePatch", BindingFlags.Static | BindingFlags.NonPublic)));
				ClassInjector.RegisterTypeInIl2Cpp<QuickMenuMunchenPage>();
			}
			catch (Exception ex)
			{
				ConsoleUtils.FlushToConsole("UnchainedMenuAPI", "Failed patching QuickMenu function: " + ex.Message, ConsoleColor.Gray, "InitializeButtonPatches", 27);
			}
		}

		private static void QuickMenuOnEnablePatch() //runs from the harmony patch, i believe this works
		{
			if (!initializedQuickMenuChanges)
			{
				initializedQuickMenuChanges = true;
				QuickMenuUtils.OnMenuInitialized(); //runs menu init
			}
		}

		internal static bool HasMenuChangesBeenInitialized()
		{
			return initializedQuickMenuChanges;
		}
	}
}
