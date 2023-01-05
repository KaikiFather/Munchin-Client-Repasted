using System;
using System.Security.Cryptography;
using MelonLoader;
using MunchenClient.Config;
using MunchenClient.ModuleSystem.Modules;
using MunchenClient.Utils;
using UnityEngine;
using VRC.Core;

namespace MunchenClient.Patching.Patches
{
	internal class VRCPlayerPatch : PatchComponent
	{
		protected override string patchName => "VRCPlayerPatch";

		internal override void OnInitializeOnStart()
		{
			InitializeLocalPatchHandler(typeof(VRCPlayerPatch));
			if (true/*!ApplicationBotHandler.IsBot()*/) //these patches seem to no longer work, going to hard code with objects 
			{
				//PatchMethod(typeof(VRCPlayer).GetMethod("Method_Public_Static_String_APIUser_0"), GetLocalPatch("GetFriendlyDetailedNameForSocialRankPatch"), null);
				//PatchMethod(typeof(VRCPlayer).GetMethod("Method_Public_Static_String_APIUser_1"), GetLocalPatch("GetFriendlyDetailedNameForSocialRankPatch"), null);
				//PatchMethod(typeof(VRCPlayer).GetMethod("Method_Public_Static_Color_APIUser_0"), GetLocalPatch("GetColorForSocialRankPatch"), null);
			}
			else
			{
				PatchMethod(typeof(VRCPlayer).GetMethod("Method_Public_Virtual_Final_New_Void_Single_Single_0"), GetLocalPatch("UnknownUpdatePatch"), null);
			}
		}

		private static bool UnknownUpdatePatch()
		{
			return false;
		}

		private static bool GetFriendlyDetailedNameForSocialRankPatch(APIUser __0, ref string __result)
		{
			if (__0 == null)
			{
				return true;
			}

			string[] LocalRanks = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "//MünchenClient//Config//LocalRanks.json");
			foreach (string Ranks in LocalRanks)
            {
				MelonLogger.Msg("Ranking " + __0.displayName + $"ID: {__0.id}");
                string[] Rank = Ranks.Split(':');
				string ID = Rank[0];
                string RankName = Rank[1];
                string RankColor = Rank[2];
				if (__0.id == ID)
                {
					__result = RankName;
                }
            }

			//if (Configuration.GetGeneralConfig().RanksCustomRanks && PlayerUtils.playerCustomRank.ContainsKey(__0.id) && PlayerUtils.playerCustomRank[__0.id].customRankEnabled)
			//{
			//	__result = PlayerUtils.playerCustomRank[__0.id].customRank;
			//	return false;
			//}
			return true;
		}
        public static Color ColorIdentify(string ColorString)
        {
            switch (ColorString)
            {
                case "white": return Color.white;
                case "black": return Color.black;
                case "blue": return Color.blue;
                case "clear": return Color.clear;
                case "cyan": return Color.cyan;
                case "gray": return Color.gray;
                case "green": return Color.green;
                case "grey": return Color.grey;
                case "magenta": return Color.magenta;
                case "red": return Color.red;
                case "yellow": return Color.yellow;
                default: return Color.white;
            }
        }
		private static bool GetColorForSocialRankPatch(APIUser __0, ref Color __result)
		{
			if (__0 == null)
			{
				return true;
			}
            string[] LocalRanks = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "//MünchenClient//Config//LocalRanks.json");
            foreach (string Ranks in LocalRanks)
            {
                MelonLogger.Msg("Coloring " + __0.displayName + $"ID: {__0.id}");
                string[] Rank = Ranks.Split(':');
                string ID = Rank[0];
                string RankName = Rank[1];
                string RankColor = Rank[2];
                if (__0.id == ID)
                {
                    __result = ColorIdentify(RankColor); 
                }
            }

			//if (Configuration.GetGeneralConfig().RanksCustomRanks && PlayerUtils.playerCustomRank.ContainsKey(__0.id) && PlayerUtils.playerCustomRank[__0.id].customRankColorEnabled)
			//{
			//	__result = PlayerUtils.playerCustomRank[__0.id].customRankColor;
			//	return false;
			//}
			return true;
		}
	}
}
