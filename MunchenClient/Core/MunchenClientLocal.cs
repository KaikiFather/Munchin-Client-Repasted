using System.Collections.Generic;
using System.Net.Http;
using MelonLoader;
using MunchenClient.Menu.Others;

namespace MunchenClient.Core
{
	public class MunchenClientLocal : MelonMod
	{
		public static HarmonyLib.Harmony _Harmony;

        public override void OnApplicationStart()
		{
            _Harmony = HarmonyInstance;
            HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36");
			httpClient.DefaultRequestHeaders.Add("Client-Agent", "MunchenClient/1.0 (VRChatMansionGang)");
			MunchenClient.OnApplicationStart(httpClient, new List<string> { "beta", "specialBenefits" }, "DEBUGMODE", debugMode: true);
		}

		public override void OnApplicationQuit()
		{
			MunchenClient.OnApplicationQuit();
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			MunchenClient.OnLevelWasLoaded(buildIndex);
		}

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
		{
			MunchenClient.OnLevelWasInitialized(buildIndex);
		}

		public override void OnUpdate()
		{
			MunchenClient.OnUpdate();
		}

		public override void OnLateUpdate()
		{
			MunchenClient.OnLateUpdate();
		}

		public override void OnFixedUpdate()
		{
			MunchenClient.OnFixedUpdate();
		}
	}
}
