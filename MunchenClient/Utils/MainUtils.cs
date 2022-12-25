using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using MunchenClient.Config;
using MunchenClient.ModuleSystem;
using MunchenClient.Patching;
using UnityEngine;

namespace MunchenClient.Utils
{
	internal static class MainUtils
	{
		internal static readonly List<object> antiGCList = new List<object>();

		internal static readonly ConcurrentQueue<Action> mainThreadQueue = new ConcurrentQueue<Action>();

		internal static void RestartGame()
		{
			ModuleManager.ShutdownModules();
			if (Configuration.GetGeneralConfig().AutoClearCache)
			{
				ClearCache();
			}
			try
			{
				Process.Start(Environment.CurrentDirectory + "/VRChat.exe", Environment.CommandLine.ToString());
			}
			catch (Exception e)
			{
				ConsoleUtils.Exception("MainUtils", "RestartGame - ProcessStart", e, "RestartGame", 41);
			}
			try
			{
				GeneralUtils.GetMainProcess().Kill();
			}
			catch (Exception e2)
			{
				ConsoleUtils.Exception("MainUtils", "RestartGame - ProcessKill", e2, "RestartGame", 50);
			}
		}

		internal static void ClearCache()
		{
			string[] directories;
			try
			{
				directories = Directory.GetDirectories(AssetBundleDownloadManager.prop_AssetBundleDownloadManager_0.field_Private_Cache_0.path);
			}
			catch (Exception e)
			{
				ConsoleUtils.Exception("Cache", "ClearCache - Folder Preparation", e, "ClearCache", 65);
				return;
			}
			string[] files;
			try
			{
				files = Directory.GetFiles(AssetBundleDownloadManager.prop_AssetBundleDownloadManager_0.field_Private_Cache_0.path);
			}
			catch (Exception e2)
			{
				ConsoleUtils.Exception("Cache", "ClearCache - File Preparation", e2, "ClearCache", 78);
				return;
			}
			ConsoleUtils.Info("Cache", $"Total Cache Entries: {directories.Length + files.Length}", ConsoleColor.Gray, "ClearCache", 83);
			for (int i = 0; i < files.Length; i++)
			{
				try
				{
					File.Delete(files[i]);
				}
				catch (Exception)
				{
				}
			}
			for (int j = 0; j < directories.Length; j++)
			{
				try
				{
					Directory.Delete(directories[j], recursive: true);
				}
				catch (Exception)
				{
				}
			}
			ConsoleUtils.Info("Cache", "Cache has been automatically cleared", ConsoleColor.Gray, "ClearCache", 104);
		}

		internal static Texture2D CreateTexture(string base64)
		{
			return CreateTexture(Convert.FromBase64String(base64));
		}

		internal static Texture2D CreateTexture(byte[] data)
		{
			Texture2D texture2D = new Texture2D(2, 2);
			ImageConversion.LoadImage(texture2D, data);
			texture2D.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			return texture2D;
		}

		internal static Sprite CreateSprite(string base64)
		{
			Texture2D texture2D = CreateTexture(base64);
			Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			Vector4 border = Vector4.zero;
			Sprite sprite = Sprite.CreateSprite_Injected(texture2D, ref rect, ref pivot, 100f, 0u, SpriteMeshType.Tight, ref border, generateFallbackPhysicsShape: false);
			sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			return sprite;
		}

		internal static Sprite CreateSprite(Texture2D texture)
		{
			Rect rect = new Rect(0f, 0f, texture.width, texture.height);
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			Vector4 border = Vector4.zero;
			Sprite sprite = Sprite.CreateSprite_Injected(texture, ref rect, ref pivot, 100f, 0u, SpriteMeshType.Tight, ref border, generateFallbackPhysicsShape: false);
			sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			return sprite;
		}

		internal static void ShutdownClient()
		{
			Patcher.UnpatchAllMethods();
			ModuleManager.DeinitializeModules();
		}

		internal static string GetLaunchParameter(string name)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i].StartsWith(name))
				{
					if (commandLineArgs[i].Contains("="))
					{
						return commandLineArgs[i].Substring(commandLineArgs[i].LastIndexOf("=") + 1);
					}
					return commandLineArgs[i];
				}
			}
			return string.Empty;
		}

		internal static void ProcessMainThreadQueue()
		{
			try
			{
				if (mainThreadQueue.Count > 0 && mainThreadQueue.TryDequeue(out var result))
				{
					result();
				}
			}
			catch (Exception e)
			{
				ConsoleUtils.Exception("Main", "ProcessMainThreadQueue", e, "ProcessMainThreadQueue", 189);
			}
		}

		internal static string GetTextFromClipboard()
		{
			try
			{
				return Clipboard.GetText();
			}
			catch (Exception ex)
			{
				ConsoleUtils.Info("Utils", "Clipboard Exception: " + ex.Message, ConsoleColor.Gray, "GetTextFromClipboard", 203);
				return "Clipboard Exception (" + ex.Message + ")";
			}
		}

		internal static string GetActualHardwareIdentifier()
		{
			long num = 2173162573851L;
			long num2 = num ^ -1822076715 ^ 0xCCA000000000000L ^ 0x22A7DA944CL ^ -7968319096167071744L ^ -3816365552469803008L ^ 0x48E877F1F3000000L ^ -3805541685128069120L ^ -6026324156323004416L ^ 0x1C2942AACD390000L ^ -2340264320232849408L ^ 0x4230000000000000L ^ 0x3D9CF2DDDA149800L ^ 0x316991CC0L ^ 0 ^ -2145479602233933824L ^ 0x6BBF276000000000L ^ 0 ^ 0x2B98CF5627BC0000L ^ 0x1D2E9628A000000L ^ 0x2FEED00000000000L ^ 0x537F000000000000L ^ -6972949015494792192L ^ 0x6A00000000000000L ^ -279495115381760L ^ 0 ^ 0 ^ 0x269D9 ^ 0 ^ 0 ^ 0x29C1BC8F49000000L ^ 0x26420C4E67000000L ^ 0 ^ 0xBF9CC200000000L ^ 0x50575A0000000000L ^ 0x5EF87EAFC7079000L ^ 0x67909B0000000000L ^ -2319916758049226752L ^ 0x490685E592FC6400L ^ 0xFDBF8 ^ 0 ^ 0x2000000000000000L ^ 0xC0000000u ^ 0x820000 ^ 0 ^ -115975173177344L ^ 0x4922390000000000L ^ 0x578E7ABDF9000000L ^ 0x259AD71700000000L ^ 0x63BB4A0000000L ^ 0x6100000000000000L ^ -5188146770730811392L ^ 0x57FD3E0000000000L ^ -4276730796141707264L ^ -6786001544839371008L ^ 0 ^ 0x5277C5E0D600000L ^ 0 ^ -5941371953334321152L ^ -7540573888590118912L ^ 0x116302D900000000L ^ 0 ^ 0x5400000000000000L ^ -3575858104132173824L ^ 0x436191A723B70000L ^ 0x6891E861A200000L ^ -4611686018427387904L ^ 0x1F2B028600000000L ^ -8745643619089645568L ^ 0 ^ 0x7E0F430000000000L ^ 0x4924000000000L ^ 0x46B7F4BF28D26D00L ^ 0 ^ 0 ^ 0 ^ 0xD894000000L ^ 0x4A4A44376C2A0000L ^ 0 ^ 0x2A52138F90000000L ^ 0 ^ 0x52BC0 ^ 0x39682E22000000L ^ 0xAF7DC00000000L ^ 0x120BCA6F0000L ^ 0x4F034386D1BC0000L ^ 0x160840000000000L ^ 0x789278100D000000L ^ 0x5C054D0E930L ^ -17592186044416L ^ -2830512365802356736L ^ 0x26657C8418F6EFL ^ 0 ^ 0x4C00000000000000L ^ 0x46B0D08388000000L ^ -1209379031510679552L ^ 0 ^ 0x53421C1372000000L ^ 0x355E000000000000L ^ 0 ^ 0x37D128F300000000L ^ 0x30000 ^ 0x14E20C1D62C42800L ^ 0 ^ -8515242394525958144L ^ 0x63286B0000000000L ^ 0x45F9710000000000L ^ 0x40B5B5E9A1380000L ^ 0 ^ 0 ^ 0x46E1980000000000L ^ 0x552E178000000000L ^ 0x7100000000000000L ^ 0 ^ 0 ^ 0x756A490948B0EA00L ^ -4858255253784494080L ^ 0x6800000000000000L ^ 0x1040000000000000L ^ 0x38A2279F00000000L ^ 0 ^ 0 ^ 0x6C1D620000000000L ^ 0x475FF9633600L ^ 0 ^ -977125538244067328L ^ -82522213862559L ^ 0 ^ 0x65C2000000000000L ^ 0 ^ -1326310090260611072L ^ long.MinValue ^ 0x4901000000000000L ^ -8134912768680656896L ^ 0x32119EEDDL ^ 0 ^ 0x1F200000000000L ^ 0 ^ 0 ^ long.MinValue ^ 0x7056860000000000L ^ -2132450025513418752L ^ 0x295ACC538000000L ^ 0 ^ 0x72BFEAFD6C800000L ^ 0x3673E879F51A0000L ^ 0x6C4D000000000000L ^ -1785995011112828928L ^ 0x557E698D8AA28000L ^ 0 ^ 0 ^ 0 ^ 0x2481BAB400000000L ^ 0 ^ 0x229F347CCL ^ -1 ^ 0x2100000000000000L ^ 0x5E23C7EE93780000L ^ 0x43C5F8DE88F89400L ^ -3668643526L ^ 0x4F7D959CAFA20000L ^ 0x1000000000000000L ^ 0x2DC9F0CB8FC60000L ^ 0x5360000000000L ^ 0x39CEA36000000000L ^ 0 ^ -6708111644968353792L ^ 0 ^ 0x3CB438B898220000L ^ 0 ^ 0x530CDCA00EF12100L ^ 0 ^ 0x1AFC460000000000L ^ -8556839292003942400L ^ -8523625244752084992L ^ 0 ^ 0 ^ 0 ^ 0 ^ -102346541645774848L ^ 0x6C00000000000L ^ 0 ^ 0 ^ 0x3A00000000000000L ^ 0x7A40000000000000L ^ 0 ^ 0x150A9A0000L ^ 0x90EBB9BBu ^ -8796093022208L ^ 0x21B39D0000000000L ^ 0 ^ 0x5ADD2ECF5D000000L ^ 0 ^ 0 ^ 0x1D11CC1A4000000L ^ -2864289363007635456L ^ 0x1439FCC7C0000000L ^ 0x887C00000000L ^ 0 ^ -8546673436703850496L ^ 0 ^ 0x55653E3DAF000000L ^ 0x1C1DCB20FEFA5200L ^ 0x3626000000000000L ^ 0x6900000000000000L ^ 0x2A00000000000000L ^ -1637459926619565056L ^ 0x64297DB3 ^ -34468790272L ^ -35184372088832L ^ 0x6D00000000000000L ^ -4611686018427387904L ^ 0x3A8074AC00000000L ^ -215993974035316736L ^ 0x61839C0000000000L ^ 0x35D21362DE1D0000L ^ -6812843387394195456L ^ 0x41A5000000000000L ^ 0x18669D6BFC296200L ^ -1191817827951050752L ^ 0x44BD2F0000000000L ^ 0x1C5D000000000000L ^ 0 ^ 0 ^ 0x169C369637000000L ^ 0 ^ 0x5410975660600000L ^ 0 ^ -1879048192 ^ 0x6100000000000000L ^ 0x188211800000L ^ 0x1537B10000000000L ^ 0 ^ long.MinValue ^ 0xCF193E135000000L ^ 0x3A64A65800000000L ^ 0x775C1B5379000000L ^ 0 ^ 0x24E8000000000000L ^ long.MinValue ^ 0x1A4000000000L ^ 0 ^ -9119789245425254400L ^ 0x3900000000000000L ^ 0 ^ -1029424358575046656L ^ -8285429028383358976L ^ -218296301002752L ^ 0x3F69013CC790000L ^ 0x57FD57248D000000L ^ 0x1A66CAA7640CEA00L ^ 0 ^ 0x2A62DD0000000000L ^ -63350767616L ^ -3381370963363233792L ^ 0x3BB4FD8A00000L ^ -506889355919360000L ^ -4098128227564781568L ^ 0x7720D516CD000000L ^ 0 ^ -204509162766336L ^ 0x38D2CAC662060000L ^ 0x312F2F2800000000L ^ 0 ^ 0xC27EFE ^ -2933944628392493056L ^ 0 ^ -1015596900344135680L ^ 0x7100000000000000L ^ -2483813344364L ^ 0x165CEB0000000000L ^ -8918899510332751872L ^ 0 ^ -1662046838755164160L ^ -6854478632857894912L ^ -1886209865282486272L ^ 0x227074C2C7E40000L ^ 0 ^ -2480561153L ^ 0x236665289F545100L ^ 0x22FD000000000000L ^ -7208292678583189504L ^ 0 ^ 0 ^ 0x8E0000000L ^ -2515566150608224256L ^ long.MinValue ^ 0x6D4B15EC00000000L ^ 0 ^ 0x5C ^ 0x340A8949C3C0000L ^ 0 ^ 0 ^ 0 ^ -6500890165223021312L ^ 0 ^ 0x98F000000000000L ^ 0x596420EC00000000L ^ -103691894492L ^ 0x296C025682D60000L ^ 0x6C1919D2CC000000L ^ -6869221376L ^ -6946316783670263808L ^ 0x5FA2C5C57L ^ -7250736839250100992L ^ 0x2EA767F00000000L ^ -9695232 ^ 0x27E4000000000000L ^ -5856632329836953600L ^ 0x4000000000000000L ^ 0x7C35D88248EE0000L ^ 0 ^ 0x448046F25E300000L ^ 0x87CCB44C000000L ^ -54980231168L ^ 0x3DA9000000000000L ^ 0x3C30000000000000L ^ -5619446959279439872L ^ -806879232 ^ -4375209654595092480L ^ -6649990085934579712L ^ 0x375D22D3B4BF0000L ^ 0x3528F41ECDE80000L ^ 0 ^ 0xB98F27648000000L ^ 0x5D088A0858000000L ^ -4942815240896118784L ^ 0x5F33650000000000L ^ long.MinValue ^ -525805904864L ^ 0x27DCB80000000000L ^ 0 ^ 0x11E8570498000000L ^ 0x747A000000000000L ^ -2377900603251621888L ^ 0x39F0000000L ^ -2738188573441261568L ^ 0x2701194F2CD00000L ^ 0 ^ 0x2CD384C7CB230000L ^ 0xA3FECE5527D5800L ^ -2738188573441261568L ^ 0x1D5C832A0C97CB00L ^ 0x4BA5E72EA5A02L ^ 0x4EC4DE60189B0000L ^ 0x164A6E88000L ^ 0x6A00000000L ^ 0x47EF3FE000000000L ^ 0x1F49000000000000L ^ 0x25FB040000000000L ^ 0x35E1F86C ^ 0x6B63AF0C6A9C0000L ^ 0x68CDA10000000000L ^ -4250999496747515904L ^ -7133701809754865664L ^ 0x4865130000000000L ^ 0x2FFEEB2A0000000L ^ -5764607523034234880L ^ 0x8F1AD523B000000L ^ 0x87560000000L ^ 0x1E94000000000000L ^ -239286650941734912L ^ -5024187596796854272L ^ 0x794EF20000000000L ^ 0x566C154F862E0000L ^ 0x4B9A019BDF98E100L ^ 0 ^ 0x3378586140L ^ -9066553134481932288L ^ 0x1E46BCE300000000L ^ 0x98F536900000000L;
			num = num2;
			try
			{
				string name = "SOFTWARE\\Microsoft\\Cryptography";
				string name2 = "MachineGuid";
				using RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
				using RegistryKey registryKey2 = registryKey.OpenSubKey(name);
				if (registryKey2 == null)
				{
					ConsoleUtils.Info("Main", "Unable to find hardware identifier key", ConsoleColor.Gray, "GetActualHardwareIdentifier", 226);
					return string.Empty;
				}
				object value = registryKey2.GetValue(name2);
				if (value == null)
				{
					ConsoleUtils.Info("Main", "Unable to find hardware identifier index", ConsoleColor.Gray, "GetActualHardwareIdentifier", 235);
					return string.Empty;
				}
				return value.ToString();
			}
			catch (Exception)
			{
				ConsoleUtils.Info("Main", "Failed getting HWID - falling back to legacy HWID", ConsoleColor.Gray, "GetActualHardwareIdentifier", 245);
				return SystemInfo.GetDeviceUniqueIdentifier();
			}
		}
	}
}
