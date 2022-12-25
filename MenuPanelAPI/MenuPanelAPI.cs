using UnityEngine;

namespace MenuPanelAPI
{
	internal class MenuPanelAPI
	{
		private static GameObject menuPanelUserPanel;

		private static GameObject menuPanelButtonReference;

		internal static GameObject GetMenuUserPanel()
		{
			if (menuPanelUserPanel == null)
			{
				menuPanelUserPanel = GameObject.Find("MenuContent/Screens/UserInfo");  //fixed for Guid change
			}
			return menuPanelUserPanel;
		}

		internal static GameObject GetMenuButtonTemplate(string buttonTemplate)
		{
			if (menuPanelButtonReference == null)
			{
				menuPanelButtonReference = GetMenuUserPanel().transform.Find(buttonTemplate).gameObject;
			}
			return menuPanelButtonReference;
		}

		internal static GameObject GetTabParent(string column)
		{
			return GetMenuUserPanel().transform.Find(column).gameObject;
		}
	}
}
