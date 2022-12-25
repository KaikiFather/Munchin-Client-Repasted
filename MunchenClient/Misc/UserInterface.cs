using System;
using System.Collections.Generic;
using HeathenEngineering.OSK.v2;
using MunchenClient.Config;
using MunchenClient.Core;
using MunchenClient.Core.Compatibility;
using MunchenClient.ModuleSystem.Modules;
using MunchenClient.Patching.Patches;
using MunchenClient.Utils;
using MunchenClient.Wrappers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserEventCarousel = MonoBehaviourPublicObusQu1VaObQu12StUnique;
using UserEventCell = MonoBehaviourPublicTMteCaSiImdiCoUnique;

namespace MunchenClient.Misc
{
	internal class UserInterface
	{
		private static float nextRefreshSocial = 0f;

		private static float nextRefreshWorlds = 0f;

		private static Button confirmButton;

		private static InputField textInputField;

		private static readonly float notificationOffsetX = 250f;

		private static readonly float notificationOffsetY = 345f;

		private static GameObject notificationHudParent;

		private static GameObject notificationHudTemplate;

		private static Color notificationBackgroundFadeIn = new Color(0f, 0f, 0f, 0.6f);

		private static Color notificationBackgroundFadeOut = new Color(0f, 0f, 0f, 0f);

		private static Color notificationTextFadeIn = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private static Color notificationTextFadeOut = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

		private static readonly List<HudNotification> currentlyDisplayedNotifications = new List<HudNotification>();

		internal static void SetupMenuButtons()
		{
            //SetupSocialRefreshButton(); //depreciated, Big menu update & social active updating update
            if (!Configuration.GetGeneralConfig().DisableKeyboardImprovements) { /*SetupScandinavianKeyboard();*/ /*err i dont care to fix*/ }
			//SetupPasteFromClipboardButton(); //depreciated, big menu update
			//SetupNotificationsHud(); //broken
            SetupRegenerateHWIDButton(); //pretty sure this is broken, unsure
			//SetupWorldRefreshButton(); // depreciated, big menu update
		}

		#region astroVRChud
		private static UserEventCarousel _ActiveCarousel { get; set; } //thanks astro for new UI notifi code
        internal static UserEventCarousel ActiveCarousel
        {
			get
			{
				if (_ActiveCarousel == null)
				{
					foreach (var carousel in Resources.FindObjectsOfTypeAll<UserEventCarousel>())
					{
						if (carousel != null)
						{
							if (carousel.field_Private_List_1_MonoBehaviourPublicTMteCaSiImdiCoUnique_1.Count != 0)
							{
								return _ActiveCarousel = carousel;
							}
						}
					}

				}
				return _ActiveCarousel;
			}
        }
		private static Transform _User_Event_Carousel { get; set; }

        internal static Transform User_Event_Carousel
        {
            get
            {
                if (_User_Event_Carousel == null)
                {
                    return _User_Event_Carousel = ActiveCarousel.transform;
                }
                return _User_Event_Carousel;
            }
        }
        internal static void WriteHudMessage(string Text)
        {
            if (ActiveCarousel != null)
            {
                ActiveCarousel.Method_Private_Void_String_Sprite_0(Text, AssetLoader.LoadSpriteFromDisk(Environment.CurrentDirectory + "\\MünchenClient\\Dependencies\\ClientAssets\\Notification.png") );
            }
        }
		#endregion


		#region old notification hud
		internal static void OnUpdate()
		{
			bool flag = false;
			float t = 10f * Time.deltaTime;
			for (int i = 0; i < currentlyDisplayedNotifications.Count; i++)
			{
				currentlyDisplayedNotifications[i].aliveTime -= Time.deltaTime;
				if (currentlyDisplayedNotifications[i].aliveTime <= 0f)
				{
					UnityEngine.Object.Destroy(currentlyDisplayedNotifications[i].gameObject);
					currentlyDisplayedNotifications.RemoveAt(i);
					flag = true;
					continue;
				}
				if (currentlyDisplayedNotifications[i].aliveTime <= 1f)
				{
					currentlyDisplayedNotifications[i].background.color = Color.Lerp(currentlyDisplayedNotifications[i].background.color, notificationBackgroundFadeOut, t);
					currentlyDisplayedNotifications[i].text.faceColor = Color32.Lerp(currentlyDisplayedNotifications[i].text.faceColor, notificationTextFadeOut, t);
				}
				else
				{
					currentlyDisplayedNotifications[i].background.color = Color.Lerp(currentlyDisplayedNotifications[i].background.color, notificationBackgroundFadeIn, t);
					currentlyDisplayedNotifications[i].text.faceColor = Color32.Lerp(currentlyDisplayedNotifications[i].text.faceColor, notificationTextFadeIn, t);
				}
				Vector3 localPosition = Vector3.Lerp(currentlyDisplayedNotifications[i].gameObject.transform.localPosition, currentlyDisplayedNotifications[i].targetPosition, t);
				currentlyDisplayedNotifications[i].gameObject.transform.localPosition = localPosition;
			}
			if (flag)
			{
				UpdateNotificationTargetPositions();
			}
		}

		internal static void UpdateNotificationTargetPositions()
		{
			int num = 0;
			for (int num2 = currentlyDisplayedNotifications.Count; num2 > 0; num2--)
			{
				currentlyDisplayedNotifications[num2 - 1].targetPosition = new Vector3(notificationOffsetX, 35f * (float)num - notificationOffsetY, 0f);
				num++;
			}
		}

		internal static void ClearNotificationHud()
		{
			for (int i = 0; i < currentlyDisplayedNotifications.Count; i++)
			{
				UnityEngine.Object.Destroy(currentlyDisplayedNotifications[i].gameObject);
			}
			currentlyDisplayedNotifications.Clear();
		}

		internal static void AddNotificationToHud(string text)
		{
			if (true/*!ApplicationBotHandler.IsBot()*/)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(notificationHudTemplate, notificationHudParent.transform);
				gameObject.name = "Munchen Notification";
				gameObject.transform.localPosition = new Vector3(300f, -345f, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
				HudNotification hudNotification = new HudNotification
				{
					gameObject = gameObject,
					background = gameObject.GetComponent<ImageThreeSlice>(),
					text = gameObject.GetComponentInChildren<TextMeshProUGUI>(),
					aliveTime = 10f
				};
				hudNotification.background.color = new Color(0f, 0f, 0f, 0f);
				hudNotification.text.faceColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
				hudNotification.text.text = text;
				gameObject.SetActive(value: true);
				currentlyDisplayedNotifications.Add(hudNotification);
				UpdateNotificationTargetPositions();
			}
		}

		internal static void SetupNotificationsHud()
		{
			notificationHudParent = GameObject.Find("UnscaledUI/HudContent_Old/Hud/AlertTextParent");  //fixed for Guid change
			notificationHudTemplate = UnityEngine.Object.Instantiate(notificationHudParent.transform.Find("Capsule").gameObject, notificationHudParent.transform);
			notificationHudTemplate.transform.Find("Text").gameObject.SetActive(value: true);
			notificationHudTemplate.name = "Munchen Alert";
		}
		#endregion 

		private static void SetupRegenerateHWIDButton()
		{
			GameObject original = GameObject.Find("MenuContent/Screens/Authentication/StoreLoginPrompt/ButtonCreate"); //fixed this string to be appropriate for guid change
			GameObject gameObject = UnityEngine.Object.Instantiate(original);
			gameObject.gameObject.name = "RegenerateHWID";
			gameObject.GetComponentInChildren<Text>().text = "Regenerate HWID";
			gameObject.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
			gameObject.GetComponentInChildren<Button>().onClick.AddListener((Action)delegate
			{
				GeneralWrappers.AlertAction("Notice", "Are you sure you want to regenerate HWID?", "Regenerate", delegate
				{
					UnityEnginePatch.GenerateHardwareIdentifier(GeneralUtils.fastRandom);
					GeneralWrappers.ClosePopup();
				}, "Cancel", delegate
				{
					GeneralWrappers.ClosePopup();
				});
			});
			GameObject gameObject2 = GameObject.Find("MenuContent/Screens/Authentication/StoreLoginPrompt"); //fixed this string to be appropriate for guid change
			gameObject.transform.SetParent(gameObject2.transform, worldPositionStays: false);
			gameObject.transform.localPosition = new Vector3(0f, -444f, 0f);
		}

		/* Unused
		private static void SetupPasteFromClipboardButton()
		{
			GameObject gameObject = GameObject.Find("MenuContent/Popups/InputPopup/ButtonRight");  //fixed for Guid change
			confirmButton = gameObject.GetComponent<Button>();
			textInputField = GameObject.Find("MenuContent/Popups/InputPopup/InputField").GetComponent<InputField>();  //fixed for Guid change
			if (!Configuration.GetGeneralConfig().DisableKeyboardImprovements)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.gameObject.name = "PasteFromClipboard";
				gameObject2.GetComponentInChildren<Text>().text = "Paste From Clipboard";
				gameObject2.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
				gameObject2.GetComponentInChildren<Button>().onClick.AddListener((Action)delegate
				{
					PasteStringIntoInputField(MainUtils.GetTextFromClipboard());
				});
				GameObject gameObject3 = GameObject.Find("MenuContent/Popups/InputPopup");  //fixed for Guid change
				gameObject2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
				gameObject2.transform.localPosition = new Vector3(435f, -275.8f, 0f);
			}
		}

		private static void SetupScandinavianKeyboard()
		{
			GameObject gameObject = GameObject.Find("MenuContent/Popups/InputPopup/Keyboard"); //fixed this string to be appropriate for guid change
			GameObject gameObject2 = gameObject.transform.Find("Keys/Row 2/Row:1 Column:12").gameObject;
			GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
			gameObject3.name = "Row:1 Column:13";
			gameObject3.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_Text_0.text = "å";
			gameObject3.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_String_0 = "å";
			gameObject3.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_String_1 = "Å";
			gameObject2 = gameObject.transform.Find("Keys/Row 3/Row:2 Column:11").gameObject;
			GameObject gameObject4 = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
			gameObject4.name = "Row:3 Column:13";
			gameObject4.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_Text_0.text = "ø";
			gameObject4.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_String_0 = "ø";
			gameObject4.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_String_1 = "Ø";
			GameObject gameObject5 = UnityEngine.Object.Instantiate(gameObject2, gameObject2.transform.parent);
			gameObject5.name = "Row:1 Column:13";
			gameObject5.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_Text_0.text = "æ";
			gameObject5.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_String_0 = "æ";
			gameObject5.GetComponentInChildren<OnScreenKeyboardKey>().field_Public_String_1 = "Æ";
		}

		private static void SetupWorldRefreshButton()
		{
			GameObject gameObject = GameObject.Find("MenuContent/Screens/Worlds/Current Room/ThisWorldButton"); //fixed this string to be appropriate for guid change
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject.transform.GetComponentInChildren<Button>().gameObject);
			gameObject2.gameObject.name = "RefreshWorlds";
			gameObject2.GetComponentInChildren<Text>().text = "Refresh";
			gameObject2.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
			gameObject2.GetComponentInChildren<Button>().onClick.AddListener((Action)delegate
			{
				if (Time.realtimeSinceStartup > nextRefreshWorlds)
				{
					nextRefreshWorlds = Time.realtimeSinceStartup + 10f;
					UiWorldList[] array = Resources.FindObjectsOfTypeAll<UiWorldList>();
					for (int i = 0; i < array.Length; i++)
					{
						GeneralUtils.RefreshVRCUiList(array[i]);
					}
					GeneralWrappers.AlertPopup("Refresh", "World lists refreshed");
				}
				else
				{
					GeneralWrappers.AlertPopup("Refresh", $"You can refresh in {Math.Floor(nextRefreshWorlds - Time.realtimeSinceStartup)} seconds");
				}
			});
			GameObject gameObject3 = GameObject.Find("MenuContent/Screens/Worlds/Current Room"); //fixed this string to be appropriate for guid change
			gameObject2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
			gameObject2.transform.localPosition = new Vector3(550f, 4.5f, -0.1f);
			if (Configuration.GetGeneralConfig().ColorChangerEnable)
			{
				MenuColorHandler.FindMenuItems();
			}
		}

		private static void SetupSocialRefreshButton()
		{
			GameObject gameObject = GameObject.Find("MenuContent/Screens/Social/UserProfileAndStatusSection/Status/EditStatusButton"); //fixed this string to be appropriate for guid change
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject.transform.GetComponentInChildren<Button>().gameObject);
			gameObject2.gameObject.name = "RefreshSocial";
			gameObject2.GetComponentInChildren<Text>().text = "Refresh";
			gameObject2.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
			gameObject2.GetComponentInChildren<Button>().onClick.AddListener((Action)delegate
			{
				if (Time.realtimeSinceStartup > nextRefreshSocial)
				{
					nextRefreshSocial = Time.realtimeSinceStartup + 30f;
					UiUserList[] array = Resources.FindObjectsOfTypeAll<UiUserList>();
					for (int i = 0; i < array.Length; i++)
					{
						GeneralUtils.RefreshVRCUiList(array[i]);
					}
					GeneralWrappers.AlertPopup("Refresh", "Social lists refreshed");
				}
				else
				{
					GeneralWrappers.AlertPopup("Refresh", $"You can refresh in {Math.Floor(nextRefreshSocial - Time.realtimeSinceStartup)} seconds");
				}
			});
			GameObject gameObject3 = GameObject.Find("MenuContent/Screens/Social/UserProfileAndStatusSection/Status"); //fixed this string to be appropriate for guid change
			gameObject2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
			gameObject2.transform.localPosition = new Vector3(540f, 0f, 0f);
		}

		internal static void PasteStringIntoInputField(string text, bool pressContinue = true)
		{
			if (textInputField == null || confirmButton == null)
			{
				GameObject gameObject = GameObject.Find("MenuContent/Popups/InputPopup/ButtonRight"); //fixed this string to be appropriate for guid change
				confirmButton = gameObject.GetComponent<Button>();
				textInputField = GameObject.Find("MenuContent/Popups/InputPopup/InputField").GetComponent<InputField>(); //fixed this string to be appropriate for guid change
			}
			textInputField.text = text;
			if (pressContinue)
			{
				confirmButton.Press();
			}
		}
		*/
	}
}
