using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using MelonLoader;
using MunchenClient.Config;
using MunchenClient.Core.Compatibility;
using MunchenClient.Menu;
using MunchenClient.Menu.Others;
using MunchenClient.Menu.PlayerOptions;
using MunchenClient.Misc;
using MunchenClient.ModuleSystem;
using MunchenClient.Patching;
using MunchenClient.Patching.Patches;
using MunchenClient.Security;
using MunchenClient.Utils;
using MunchenClient.Wrappers;
using ServerAPI.Core;
using UnchainedButtonAPI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MunchenClient.Core
{
	public static class MunchenClient
	{
		private static readonly string assetPath = Configuration.GetClientFolderName() + "\\Dependencies\\ClientAssets\\ClientAssets.assetbundle";

		private static bool shouldRunClient = false;

		public static void OnApplicationStart(HttpClient httpClient, List<string> parameters, string baseUrl, bool debugMode)
		{
			PerformanceProfiler.StartProfiling("OnApplicationStart"); //runs performance profiling for OnApplication start
			try
			{
				ServicePointManager.Expect100Continue = true;																											// This sets the httpclient to only request to the server with request headers, and if the server responds with a 100-continue response it allows the client to send the data associated with the headers https://learn.microsoft.com/en-us/dotnet/api/system.net.servicepointmanager.expect100continue?view=net-7.0
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; //sets httpclient security protocol
				HttpClientCustom.SetHttpClient(httpClient);																											  //creates http client
				GeneralUtils.clientSpecialBenefits = parameters.Contains("specialBenefits");																	 //returns true if start params contain specialBenefits
				GeneralUtils.clientBetaBranch = parameters.Contains("betaBranch");																				//returns true if start params contrain Beta access
			}
			catch (Exception e) { ConsoleUtils.Exception("Main", "OnApplicationStart - Assign Parameters", e, "OnApplicationStart", 51); }

			#region fuck appbots
			/* appbots removed
			try
			{
				ApplicationBotHandler.PreCheckBotHandler();
			}
			catch (Exception e2)
			
			{
				ConsoleUtils.Exception("Main", "OnApplicationStart - PreCheckBotHandler", e2, "OnApplicationStart", 60);
			}
			*/
			#endregion

			#region Startup pt1 Antileak->Earlyconfig->Compatibility->ServerAPI->LoadConfigs->ApplyConfigs
            try
			{
				GeneralUtils.AssignMainProcess(Process.GetCurrentProcess()); //assigns vrc/munchin as Main process
			}
			catch (Exception e3) { ConsoleUtils.Exception("Main", "OnApplicationStart - Assign Process", e3, "OnApplicationStart", 70); }
			try
			{
				AntiLeak.InitializeAntiLeak(); //tries to hide itself from kernel and other leaking methods, modified to not send any http requests or scan active processes
			}
			catch (Exception e4) { ConsoleUtils.Exception("Main", "OnApplicationStart - AntiLeak", e4, "OnApplicationStart", 80); }
			try
			{
				Configuration.OnApplicationStart(); //Loads Munchin Configuration
			}
			catch (Exception e5) { ConsoleUtils.Exception("Main", "OnApplicationStart - Initialize", e5, "OnApplicationStart", 89); }
            try
            {
                CompatibilityLayer.CheckCompatiblity(); //Runs compatibility checks, mostly useless until people make more mods
            }
            catch (Exception e6) { ConsoleUtils.Exception("Main", "OnApplicationStart - CompatibilityLayer", e6, "OnApplicationStart", 102); }
            //try { ServerAPICore.InitializeInstance(baseUrl, debugMode); /*init's server instance*/ } catch (Exception e7) { ConsoleUtils.Exception("Main", "OnApplicationStart - ServerAPI", e7, "OnApplicationStart", 112); }
            try
            {
                Configuration.LoadAllConfigurations(); //loads configs
            }
            catch (Exception e8) { ConsoleUtils.Exception("Main", "OnApplicationStart - Config Loader", e8, "OnApplicationStart", 121); }
            try
            {
                Configuration.ApplyConfigSettings(); //applies configs
            }
            catch (Exception e9) { ConsoleUtils.Exception("Main", "OnApplicationStart - Config Processor", e9, "OnApplicationStart", 130); }
			#endregion

			ModuleManager.ConstructModules();           //Loads all modules
			ModuleManager.InitializeModules();		   //Runs all modules
            PatchManager.ConstructPatchesOnStart();   //Loads all patches, moved these here to prevent patches crashing the game from running prematurely
            PatchManager.InitializePatchesOnStart(); //Runs all patches

            SceneManager.add_sceneUnloaded((Action<Scene>)delegate(Scene scene) { OnLevelWasUnloaded(scene.buildIndex); });
			shouldRunClient = true; //allows client to run when init has completed
			MelonCoroutines.Start(WaitForUserInterfaceInitialization());
			PerformanceProfiler.EndProfiling("OnApplicationStart");
		}

		#region boilerplate 
		public static void OnApplicationQuit()
		{
			if (shouldRunClient)
			{
				ModuleManager.ShutdownModules();
				if (Configuration.GetGeneralConfig().AutoClearCache)
				{
					MainUtils.ClearCache();
				}
			}
		}

		public static void OnLevelWasLoaded(int level)
		{
			if (shouldRunClient)
			{
				PerformanceProfiler.StartProfiling("OnLevelWasLoaded");
				ModuleManager.OnLevelWasLoaded(level);
				PerformanceProfiler.EndProfiling("OnLevelWasLoaded");
			}
		}

		public static void OnLevelWasInitialized(int level)
		{
			if (shouldRunClient)
			{
				PerformanceProfiler.StartProfiling("OnLevelWasInitialized");
				ModuleManager.OnLevelWasInitialized(level);
				PerformanceProfiler.EndProfiling("OnLevelWasInitialized");
			}
		}

		public static void OnLevelWasUnloaded(int levelIndex)
		{
			if (shouldRunClient)
			{
				PerformanceProfiler.StartProfiling("OnLevelWasUnloaded");
				ModuleManager.OnLevelWasUnloaded(levelIndex);
				PerformanceProfiler.EndProfiling("OnLevelWasUnloaded");
			}
		}
		#endregion

		#region Updates
		public static void OnUpdate()
		{
			if (!shouldRunClient) { return; } //skips if shouldnt run
            //try { ServerAPICore.GetInstance().OnUpdate(); }catch (Exception e) { ConsoleUtils.Exception("Main", "OnUpdate - ServerAPICore", e, "OnUpdate", 238); }
            try
            {
                Configuration.OnUpdate(); //updates config with changed settings etc
            }
            catch (Exception e2) { ConsoleUtils.Exception("Main", "OnUpdate - Configuration", e2, "OnUpdate", 248); }
            try
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Backspace))
                {
                    MainUtils.RestartGame(); //restarts game with ctrl+alt+backspace
                }
            }
            catch (Exception e3) { ConsoleUtils.Exception("Main", "OnUpdate - Last Resort", e3, "OnUpdate", 261); }
			
			ModuleManager.OnUpdate();            //passes onupdate to the modules
			MainUtils.ProcessMainThreadQueue(); //logger queue
			ConsoleUtils.ProcessWriteQueue();  //logger write
		}

		public static void OnLateUpdate()
		{
			if (!shouldRunClient) { return; }
			ModuleManager.OnLateUpdate();
			try
			{
				AntiLeak.OnAntiLeakUpdate(); //updates anti leak
			}
			catch (Exception e) { ConsoleUtils.Exception("Main", "OnUpdate - AntiLeak", e, "OnLateUpdate", 287); }
		}

		public static void OnFixedUpdate()
		{
			if (shouldRunClient)
			{
				ModuleManager.OnFixedUpdate();
			}
		}
		#endregion

		private static IEnumerator WaitForUserInterfaceInitialization() //waits for vrcUI to be loaded
		{
			while (GeneralWrappers.GetVRCUiManager() == null) //doesnt seem to wait anymore
			{
				yield return new WaitForEndOfFrame();
			}
			OnUserInterfaceInitialized();
		}

		private static void OnUserInterfaceInitialized()
		{
            try
            {
                ConsoleUtils.SetTitle(ConsoleUtils.GetTitle() + " - " + GeneralUtils.GetClientName() + " " + GeneralUtils.GetClientVersion() + " - Developed by " + GeneralUtils.GetClientDevelopers()); //moved this later as ML would overwrite
            } catch { }

			if (!shouldRunClient) { ConsoleUtils.FlushToConsole("[Main]", "Client not running due to no initialization", ConsoleColor.Gray, "OnUserInterfaceInitialized", 317); return; }
            PatchManager.InitializePatchesOnUIInit(); //initializes on UI patching
            if (CompatibilityLayer.GetCurrentGameVersion() != 1194 && CompatibilityLayer.GetCurrentGameVersion() != 1194) { ConsoleUtils.Info("Main", $"GameVersion {CompatibilityLayer.GetCurrentGameVersion()} - Munchin is running post EAC baby les go", ConsoleColor.Gray, "OnUserInterfaceInitialized", 329); }
			MelonCoroutines.Start(WaitForMenu()); //implemented to wait until menu exists, as this source no longer does

			#region Unused
			//InitializeMenu(); //added this here, but removed again because we need to wait longer to load client assets
			//DependencyDownloader.DownloadDependency("ClientAssets", OnMenuDependencyFinished); //Attempts to download Client assets, rather lets load from local ones.
			//ConsoleUtils.Info("Main", "Downloading client assets", ConsoleColor.Gray, "OnUserInterfaceInitialized", 341); //for some reason this download fails, it shouldn't as these assets are still available @ https://shintostudios.net/download/dependencies/ClientAssets.zip
			#endregion
		}
		//private static void OnMenuDependencyFinished() { ConsoleUtils.Info("Main", "Client assets downloaded", ConsoleColor.Gray, "OnMenuDependencyFinished", 347); Configuration.GetGeneralConfig().ClientAssetsVersion = GeneralConfig.ClientAssetsVersionCurrent; InitializeMenu(); } //attempts to download client assets, it should work, but it dont.

        public static bool MenuExists { get; set; }
        public static IEnumerator WaitForMenu() //created to wait for menu to exist until making client menus, prevents gameobjects from being eaten.
        {
			while (!MenuExists)
            {
                GameObject menu = GameObject.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent"); //looks for QMParent
				if (menu != null)
                {
					MelonLogger.Msg("Menu Found: Initializing UI. . .");
					InitializeMenu();
                    MenuExists = true;
                }
				yield return new WaitForSeconds(5f);
            }
        }

		private static void InitializeMenu()
		{
            PerformanceProfiler.StartProfiling("OnUIManagerInit");
			try
			{
				GeneralWrappers.InitializeWrappers(); //loads menu wrappers
			}
			catch (Exception e) { ConsoleUtils.Exception("Main", "OnUIManagerInit - Wrapper Init", e, "InitializeMenu", 364); }
            try { LanguageManager.LoadLanguage(); } //loads language preferences
			catch (Exception e2) { ConsoleUtils.Exception("Main", "OnUIManagerInit - Language Manager", e2, "InitializeMenu", 377); }
			try
			{
				#region CreateMenus
				AssetLoader.LoadAssetBundle(assetPath); //loads menu assetbundle from local files
				QuickMenuUtils.InitializeButtonAPI(LanguageManager.GetUsedLanguage().ClientName, MainUtils.CreateSprite(AssetLoader.LoadTexture("ToggleIconOn")), MainUtils.CreateSprite(AssetLoader.LoadTexture("ToggleIconOff"))); //inits button api
				
			    new MainClientMenu();                                                                        //inits munchin main mod menu
				new QuickMenuHeader(QuickMenus.SelectedUser, LanguageManager.GetUsedLanguage().ClientName); //loads a QMheader into the selected user menu
				QuickMenuButtonRow quickMenuButtonRow = new QuickMenuButtonRow(QuickMenus.SelectedUser);   //adds button row to selected user menu
				new PlayerOptionsMenu(quickMenuButtonRow);                                                //loads player options menu in selected user tab
				                                                                                         //new PlayerApplicationBotMenu(quickMenuButtonRow); //appbots removed
				new QuickMenuSingleButton(quickMenuButtonRow, LanguageManager.GetUsedLanguage().ForceClone, delegate //adds forceclone to selected user menu
				{
					PlayerInformation selectedPlayer = PlayerWrappers.GetSelectedPlayer();
					if (selectedPlayer != null)
					{
						PlayerUtils.ChangePlayerAvatar(selectedPlayer.vrcPlayer.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.id, logErrorOnHud: true);
					}
					else
					{
						ConsoleUtils.Info("Player", "Player not found", ConsoleColor.Gray, "InitializeMenu", 411);
						GeneralWrappers.AlertPopup("Warning", "Player not found");
					}
				}, LanguageManager.GetUsedLanguage().ForceCloneDescription);
				new UserInfoMenu();                                                                 //loads user info menu - pretty sure this is broken and related to big menu udpate
				new ActionWheelMenu();                                                             //loads Action menu api - not working
				#endregion

				if (Configuration.GetGeneralConfig().AutoClearCache) { MainUtils.ClearCache(); }  //clears vrc cache if config true
            }
            catch (Exception e3) { ConsoleUtils.Exception("Main", "OnUIManagerInit - Menu Init", e3, "InitializeMenu", 428); }

            #region UIManagerInit->SetupExtraUI->FPS->UIManagerLoaded->UIManagerInit
            //try { ServerAPICore.GetInstance().OnUIManagerInit(); }catch (Exception e4) { ConsoleUtils.Exception("Main", "OnUIManagerInit - ServerAPICore", e4, "InitializeMenu", 438); }
            try
            {
                UserInterface.SetupMenuButtons(); //Sets up Social referesh button (depreciated), Paste from clipboard button (depreciated), notifications hud (broken), Hwid regen button, world referesh button (depreciated)
            }
            catch (Exception e5) { ConsoleUtils.Exception("Main", "OnUIManagerInit - SetupMenuButtons", e5, "InitializeMenu", 447); }
            try
            {
                if (Configuration.GetGeneralConfig().PerformanceUnlimitedFPS && !GeneralWrappers.IsInVR())
                {
                    Application.targetFrameRate = GeneralUtils.GetRefreshRate(); //adjusts desktop maximum FPS
                } 
                //MenuColorHandler.FindMenuItems(); //big menu coloring, finds objects, broken, depreciated with big menu update
            }
            catch (Exception e6) { ConsoleUtils.Exception("Main", "OnUIManagerInit - Post Config", e6, "InitializeMenu", 462); }
            ModuleManager.OnUIManagerLoaded(); //sets off onuiloaded for Modules
            try
            { 
                NetworkManagerPatch.OnUIManagerInit(); //supplies vrcplayer to player join patch
            }
            catch (Exception e7) { ConsoleUtils.Exception("Main", "OnUIManagerInit - SetupMenuButtons", e7, "InitializeMenu", 479); }
            #endregion

            PerformanceProfiler.EndProfiling("OnUIManagerInit"); //closes profiller and reports performance
            ConsoleUtils.Info("Performance", string.Format("OnApplicationStart took: {0:F2} ms", PerformanceProfiler.GetProfiling("OnApplicationStart")), ConsoleColor.Gray, "InitializeMenu", 487);
            ConsoleUtils.Info("Performance", string.Format("OnUIManagerInit took: {0:F2} ms", PerformanceProfiler.GetProfiling("OnUIManagerInit")), ConsoleColor.Gray, "InitializeMenu", 488);
        }
	}
}
