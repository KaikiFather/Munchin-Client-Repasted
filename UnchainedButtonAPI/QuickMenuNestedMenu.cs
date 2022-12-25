using System;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using MunchenClient.Misc;
using MunchenClient.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Menus;
using Object = UnityEngine.Object;

namespace UnchainedButtonAPI
{
	internal class QuickMenuNestedMenu : QuickMenuButtonBase
	{
		internal UIPage uiPage;

		private string nestedMenuName;

		private bool menuAccessible;

		private string menuAccessibleError;

		internal QuickMenuNestedMenu(QuickMenuButtonRow parentRow, string text, string tooltip, bool createButtonOnParent = true)
		{
            QuickMenuNestedMenu quickMenuNestedMenu = this;
            buttonParentName = parentRow.GetParentMenuName();
			InitializeNestedMenu(text);
			buttonHandler = buttonObject.transform.Find("Header_H1/LeftItemContainer/Button_Back").GetComponent<Button>();
			buttonHandler.gameObject.SetActive(value: true);
			SetAction(delegate
			{
                parentRow.parentCustomMenu?.ShowMenu(); //shows the menu
				QuickMenuUtils.OpenMenu(parentRow.GetParentMenuName(), clearStackPage: true); //opens the menu??
			});
			if (createButtonOnParent)
			{
                new QuickMenuSingleButton(parentRow, text, delegate { quickMenuNestedMenu.ShowMenu(); }, tooltip);
			}
		}

		internal QuickMenuNestedMenu(string text) //called by the main menu class to create the main menu 
		{
			InitializeNestedMenu(text);
			buttonParentName = nestedMenuName;
		}

		private void InitializeNestedMenu(string text) 
		{
            int quickMenuUniqueIdentifier = QuickMenuUtils.GetQuickMenuUniqueIdentifier();
            buttonObject = Object.Instantiate(QuickMenuTemplates.GetNestedMenuTemplate(), GameObject.Find("Container/Window/QMParent").transform);
            buttonObject.GetComponentInChildren<Canvas>(includeInactive: true).enabled = true;
            buttonObject.GetComponentInChildren<CanvasGroup>(includeInactive: true).enabled = true;
            buttonObject.GetComponentInChildren<GraphicRaycaster>(includeInactive: true).enabled = true;
            buttonObject.name = $"Menu_{QuickMenuUtils.GetQuickMenuIdentifier()}{text}{quickMenuUniqueIdentifier}";
			buttonObject.transform.SetSiblingIndex(QuickMenuUtils.GetFirstModalIndex());
			buttonText = buttonObject.transform.Find("Header_H1/LeftItemContainer/Text_Title").GetComponent<TextMeshProUGUI>();
			nestedMenuName = $"QuickMenu{QuickMenuUtils.GetQuickMenuIdentifier()}{text}{quickMenuUniqueIdentifier}";
			GameObject gameObject = buttonObject.transform.Find("ScrollRect/Viewport/VerticalLayoutGroup").gameObject;
			for (int i = 0; i < gameObject.transform.GetChildCount(); i++)
			{
				Object.Destroy(gameObject.transform.GetChild(i).gameObject);
			}
			Object.DestroyImmediate(buttonObject.GetComponent<LaunchPadQMMenu>());
			uiPage = buttonObject.AddComponent<UIPage>();
			uiPage.field_Public_String_0 = nestedMenuName;
			uiPage.field_Private_Boolean_1 = true;
			uiPage.field_Private_List_1_UIPage_0 = new List<UIPage>();
			uiPage.field_Private_List_1_UIPage_0.Add(uiPage);
			QuickMenuMunchenPage quickMenuMunchenPage = buttonObject.AddComponent<QuickMenuMunchenPage>();
			quickMenuMunchenPage.uiPage = uiPage;
			QuickMenuUtils.AddMenuToController(this);
			menuAccessible = true;
			SetMenuText(text);
			SetActive(active: false);
		}

		internal void SetMenuText(string text)
		{
			buttonText.text = text;
		}

		internal void SetMenuAccessibility(bool state, string accessError)
		{
			menuAccessible = state;
			menuAccessibleError = accessError;
		}

		internal string GetMenuName()
		{
			return nestedMenuName;
		}

		internal void NewShowMenu()
        {

        }

		internal void ShowMenu() //here be error
		{
			if (!menuAccessible) { UserInterface.WriteHudMessage(menuAccessibleError); return; } //returns is menu is inaccessable

            List<UIPage> PageList = uiPage.field_Private_List_1_UIPage_0;

			if (!QuickMenuUtils.GetQuickMenu().prop_MenuStateController_0.field_Private_UIPage_0.field_Public_String_0.Contains("Player Options"))
			{
                for (int i = 1; i < PageList.Count; i++) //loops through page list
				{ string PageName = PageList[i].field_Public_String_0;

                    if (PageName == "QuickMenuHoveredUser") //if page menu is hovered user
					{
                        PageList[i].gameObject.SetActive(value: false);
                        PageList.RemoveAt(i);
					}
					else if (PageName == "QuickMenuSelectedUserLocal") //else if page menu is selected user
					{
                        PageList[i].gameObject.SetActive(value: false);
                        PageList.RemoveAt(i);
						uiPage.gameObject.SetActive(value: true);
					}
				}
			}
            buttonObject.active = true;
            QuickMenuUtils.OpenMenu(this, clearStackPage: true);
            OnMenuShownCallback(); //pointless
		}

		internal void OnMenuUnshown()
		{
		}

		internal virtual void OnMenuShownCallback()
		{
		}
	}
}
