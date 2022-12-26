using System.Linq;
using System.Reflection;
using ActionMenuAPI;
using UnhollowerRuntimeLib.XrefScans;
using HarmonyLib;
using static MunchenClient.Core.MunchenClientLocal;
using MunchenClient.Core;

namespace MunchenClient.Patching.Patches
{
	internal class ActionMenuPatch : PatchComponent
	{
		protected override string patchName => "ActionMenuPatch";

		internal override void OnInitializeOnStart()
		{
            _Harmony.Patch(typeof(PedalOption).GetMethod(nameof(PedalOption.Start)),
                postfix: new HarmonyMethod(typeof(ActionMenuPatch), nameof(ActionMenuPatch.OpenMainPage)));
        }

		private static void OpenMainPage(PedalOption __instance)
		{
			CustomActionMenu.OpenMainPage(__instance.field_Public_ActionMenu_0);
		}
	}
}
