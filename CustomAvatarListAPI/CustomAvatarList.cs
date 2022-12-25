using System;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;

namespace CustomAvatarListAPI
{
	internal class CustomAvatarList
	{
		private static UiAvatarList templateAvatarList;

		private GameObject avatarListGameObj;

		private UiAvatarList avatarList;

		private Text avatarListTitle;

		internal static UiAvatarList AvatarListTemplate
		{
			get
			{
				if (templateAvatarList == null)
				{
					Transform transform = GameObject.Find("MenuContent/Screens/Avatar/Vertical Scroll View/Viewport").transform; //fixed for Guid change
					GameObject gameObject = transform.Find("FavoriteListTemplate").gameObject;
					GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform.Find("Content"));
					gameObject2.name = "MunchenFavoritesTemplate";
					gameObject2.transform.Find("Expired").gameObject.SetActive(value: false);
					Transform transform2 = gameObject2.transform.Find("Button");
					transform2.GetComponentInChildren<Text>().text = "New List";
					UiAvatarList component = gameObject2.GetComponent<UiAvatarList>();
					//component.field_Public_Category_0 = UiAvatarList.Category.SpecificList; //field_Public_Category_0 and 'Category' missing, likely due to big menu update
					component.StopAllCoroutines();
					gameObject2.SetActive(value: false);
					templateAvatarList = component;
				}
				return templateAvatarList;
			}
		}

		internal UiAvatarList GetAvatarList()
		{
			return avatarList;
		}

		internal Text GetAvatarListText()
		{
			return avatarListTitle;
		}

		internal void RefreshAvatarsList(List<ApiAvatar> avatars, int offset = 0, bool endOfPickers = true, VRCUiContentButton contentHeaderElement = null)
		{
			if (avatarList.scrollRect != null)
			{
				avatarList.scrollRect.normalizedPosition = new Vector2(0f, 0f);
			}
			avatarList.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0(avatars, offset, endOfPickers, contentHeaderElement);
		}

		internal static CustomAvatarList Create(string listName, int index, Action onButtonClick = null)
		{
			MelonLogger.Msg("Due to big menu update, Avatar lists and search are being skipped");
			
			CustomAvatarList customAvatarList = new CustomAvatarList
			{
				avatarListGameObj = UnityEngine.Object.Instantiate(AvatarListTemplate.gameObject, AvatarListTemplate.transform.parent)
			};
            return customAvatarList; //added to skip avatar lists, this might be fucky todo fully skip this in the future
			customAvatarList.avatarListGameObj.gameObject.name = listName;
			customAvatarList.avatarListGameObj.transform.SetSiblingIndex(index);
			customAvatarList.avatarList = customAvatarList.avatarListGameObj.GetComponent<UiAvatarList>();
			customAvatarList.avatarListTitle = customAvatarList.avatarList.GetComponentInChildren<Text>();
			customAvatarList.avatarListTitle.text = listName;
			if (onButtonClick != null)
			{
				GameObject gameObject = customAvatarList.avatarList.transform.Find("GetMoreFavorites").gameObject;
				gameObject.GetComponentInChildren<Text>().text = "Search After Avatar";
				Button componentInChildren = gameObject.GetComponentInChildren<Button>();
				componentInChildren.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
				componentInChildren.onClick.AddListener(onButtonClick);
				gameObject.SetActive(value: true);
			}
			customAvatarList.avatarListGameObj.SetActive(value: true);
			return customAvatarList;
		}
	}
}
