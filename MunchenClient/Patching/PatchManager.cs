using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MunchenClient.Core.Compatibility;
using MunchenClient.ModuleSystem.Modules;
using MunchenClient.Patching.Patches;
using MunchenClient.Utils;
using UnhollowerRuntimeLib.XrefScans;

namespace MunchenClient.Patching
{
	internal static class PatchManager
	{
		private static readonly List<PatchComponent> patches = new List<PatchComponent>();

		internal static bool CheckNonGlobalMethod(MethodBase methodBase, string methodName, int maxMethodNameLength = 0)
		{
			try
			{
				foreach (XrefInstance item in XrefScanner.XrefScan(methodBase))
				{
					if (item.Type == XrefType.Method)
					{
						MethodBase methodBase2 = item.TryResolve();
						if (!(methodBase2 == null) && methodBase2.Name.Contains(methodName) && (maxMethodNameLength <= 0 || (maxMethodNameLength > 0 && methodBase2.Name.Length <= maxMethodNameLength)))
						{
							return true;
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}

		internal static bool CheckMethod(MethodBase methodBase, string match)
		{
			try
			{
				foreach (XrefInstance item in XrefScanner.XrefScan(methodBase))
				{
					if (item.Type != 0 || !item.ReadAsObject().ToString().Contains(match))
					{
						continue;
					}
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		internal static bool CheckUsed(MethodBase methodBase, string methodName, int maxMethodNameLength = 0)
		{
			try
			{
				foreach (XrefInstance item in XrefScanner.UsedBy(methodBase))
				{
					MethodBase methodBase2 = item.TryResolve();
					if (methodBase2 == null || !methodBase2.Name.Contains(methodName) || (maxMethodNameLength > 0 && (maxMethodNameLength <= 0 || methodBase2.Name.Length > maxMethodNameLength)))
					{
						continue;
					}
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		internal static bool CheckUsing(MethodInfo methodBase, string match, Type type)
		{
			foreach (XrefInstance item in XrefScanner.XrefScan(methodBase))
			{
				if (item.Type == XrefType.Method)
				{
					MethodBase methodBase2 = item.TryResolve();
					if (!(methodBase2 == null) && methodBase2.DeclaringType == type && methodBase2.Name.Contains(match))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static HarmonyMethod GetLocalPatch(Type patchType, string name)
		{
			if (patchType == null)
			{
				ConsoleUtils.Info("PatchManager", "Cannot use GetLocalPatch as LocalPatchHandler hasn't been called yet", ConsoleColor.Gray, "GetLocalPatch", 126);
				return null;
			}
			MethodInfo method = patchType.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
			if (method == null)
			{
				ConsoleUtils.Info("PatchManager", "Failed to find valid method named: " + name, ConsoleColor.Gray, "GetLocalPatch", 134);
				return null;
			}
			return new HarmonyMethod(method);
		}

		internal static HarmonyMethod GetLocalPatch(string name)
		{
			return GetLocalPatch(typeof(PatchManager), name);
		}

		internal static void AnalyzeFunction(MethodInfo methodBase)
		{
			ConsoleUtils.Info("AnalyzeFunction", "-------------------------------------------", ConsoleColor.Gray, "AnalyzeFunction", 149);
			ConsoleUtils.Info("AnalyzeFunction", "Starting analyze of function: " + methodBase.Name, ConsoleColor.Gray, "AnalyzeFunction", 150);
			try
			{
				int num = 0;
				foreach (XrefInstance item in XrefScanner.XrefScan(methodBase))
				{
					if (item.Type == XrefType.Global)
					{
						ConsoleUtils.Info("AnalyzeFunction", "XrefScan Global: " + item.ReadAsObject().ToString(), ConsoleColor.Gray, "AnalyzeFunction", 159);
					}
					else
					{
						MethodBase methodBase2 = item.TryResolve();
						if (methodBase2 == null)
						{
							continue;
						}
						ConsoleUtils.Info("AnalyzeFunction", "XrefScan Method: " + methodBase2.Name, ConsoleColor.Gray, "AnalyzeFunction", 169);
					}
					num++;
				}
				ConsoleUtils.Info("AnalyzeFunction", $"XrefScan Instances: {num}", ConsoleColor.Gray, "AnalyzeFunction", 174);
				int num2 = 0;
				foreach (XrefInstance item2 in XrefScanner.UsedBy(methodBase))
				{
					MethodBase methodBase3 = item2.TryResolve();
					if (!(methodBase3 == null))
					{
						ConsoleUtils.Info("AnalyzeFunction", "UsedBy: " + methodBase3.Name, ConsoleColor.Gray, "AnalyzeFunction", 185);
						num2++;
					}
				}
				ConsoleUtils.Info("AnalyzeFunction", $"UsedBy Instances: {num2}", ConsoleColor.Gray, "AnalyzeFunction", 188);
			}
			catch (Exception e)
			{
				ConsoleUtils.Exception("AnalyzeFunction", "Function Analyzer", e, "AnalyzeFunction", 192);
			}
			ConsoleUtils.Info("AnalyzeFunction", "Analyze Complete!", ConsoleColor.Gray, "AnalyzeFunction", 195);
			ConsoleUtils.Info("AnalyzeFunction", "-------------------------------------------", ConsoleColor.Gray, "AnalyzeFunction", 196);
		}

		internal static void ConstructPatchesOnStart()
		{
			patches.Add(new UnityEnginePatch());
			patches.Add(new SteamworksPatch());
			//patches.Add(new PhotonPatch()); //broken and detected
			patches.Add(new NetworkManagerPatch());
			//patches.Add(new DownloaderPatch()); //broken
			//patches.Add(new EventHandlerPatch()); 
			//patches.Add(new VRCNetworkingClientPatch()); //Broken
			patches.Add(new VRCPlayerPatch()); //fine
			if (true/*!ApplicationBotHandler.IsBot()*/)
			{
				patches.Add(new WebSocketPatch()); //fine
				//patches.Add(new AntiCrashPatch()); //many broken components 
                patches.Add(new AvatarPerformancePatch()); //fine
				patches.Add(new VRCAvatarManagerPatch()); //fine
				patches.Add(new ImageDownloaderPatch()); //fine
				patches.Add(new VideoPlayerPatch()); //fine
				patches.Add(new FinalIKPatch()); //fine
				patches.Add(new NotificationPatch()); //fine
				patches.Add(new AssetManagementPatch()); //fine
				patches.Add(new ActionMenuPatch()); //fine
				patches.Add(new QuickMenuPatch()); //fine
				patches.Add(new UdonPatch()); //fine
				//patches.Add(new PortalsPatch()); //broken
				patches.Add(new VRCUIManagerPatch()); //fine
				if (!CompatibilityLayer.IsRunningOculus()) { patches.Add(new VRCTrackingSteamPatch()); }
			}
			else
			{
				patches.Add(new BatchModePatch()); //bot related patching, todo remove
				patches.Add(new CursorPatch());
			}
		}

		internal static void InitializePatchesOnStart()
		{
			for (int i = 0; i < patches.Count; i++)
			{
				try
				{
					patches[i].OnInitializeOnStart();
				}
				catch (Exception e)
				{
					ConsoleUtils.Exception("PatchManager", "OnInitializeOnStart (Patch Name: " + patches[i].GetPatchName() + ")", e, "InitializePatchesOnStart", 260);
				}
			}
		}

		internal static void InitializePatchesOnUIInit()
		{
			for (int i = 0; i < patches.Count; i++)
			{
				try
				{
					patches[i].OnInitializeOnUIInit();
				}
				catch (Exception e)
				{
					ConsoleUtils.Exception("PatchManager", "OnInitializeOnUIInit (Patch Name: " + patches[i].GetPatchName() + ")", e, "InitializePatchesOnUIInit", 276);
				}
			}
		}
	}
}
