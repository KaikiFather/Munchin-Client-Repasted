using System;
using System.Linq;
using MunchenClient.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using VRC.UI.Elements.Tooltips;

namespace UnchainedButtonAPI
{
	//todo Find new tooltip in map
	internal class QuickMenuToggleButton : QuickMenuButtonBase
	{
		private UIToggleTooltip toggleTooltip; //Map missing UiToggleTooltip, probably due to Menu Update.

		private Toggle toggleHandler;

		private string toggleText;

		private bool toggleTempDisableActions;

		private Action toggleActionOn;

		private Sprite toggleIconOn;

		private Action toggleActionOff;

		private Sprite toggleIconOff;

		internal QuickMenuToggleButton(QuickMenuButtonRow parentRow, string text, bool state, Action actionOn, string tooltipOn, Action actionOff, string tooltipOff)
		{
			buttonParentName = parentRow.GetParentMenuName();
			InitializeButton(parentRow.GetGameObject().transform, text, state, actionOn, tooltipOn, actionOff, tooltipOff);
		}

		internal QuickMenuToggleButton(string parentRow, string text, bool state, Action actionOn, string tooltipOn, Action actionOff, string tooltipOff) //zero refs
		{
			buttonParentName = parentRow;
			//InitializeButton(QuickMenuUtils.GetQuickMenu().transform.Find(parentRow + "/ScrollRect/Viewport/VerticalLayoutGroup"), text, state, actionOn, tooltipOn, actionOff, tooltipOff);
			InitializeButton(GameObject.Find(parentRow + "/ScrollRect/Viewport/VerticalLayoutGroup").transform, text, state, actionOn, tooltipOn, actionOff, tooltipOff);
		}

		private void InitializeButton(Transform parent, string text, bool state, Action actionOn, string tooltipOn, Action actionOff, string tooltipOff)
		{
			buttonObject = UnityEngine.Object.Instantiate(QuickMenuTemplates.GetToggleButtonTemplate(), parent); //ERROR
			buttonObject.name = $"Button_Toggle{QuickMenuUtils.GetQuickMenuIdentifier()}{text}{QuickMenuUtils.GetQuickMenuUniqueIdentifier()}";
			toggleTooltip = buttonObject.GetComponent<UIToggleTooltip>(); //Map missing UiToggleTooltip, probably due to Menu Update.//Map missing UiToggleTooltip, probably due to Menu Update.
			toggleHandler = buttonObject.GetComponent<Toggle>();
			toggleText = text;
			buttonText = buttonObject.transform.Find("Text_H4").GetComponentInChildren<TextMeshProUGUI>();
			buttonText.transform.localPosition = Vector3.zero;
			GameObject gameObject = buttonObject.transform.Find("Icon_Off").gameObject;
			UnityEngine.Object.Destroy(gameObject.GetComponent<StyleElement>());
			buttonIcon = gameObject.GetComponent<Image>();
			buttonIcon.transform.localPosition = new Vector3(-0.0003f, -25f, 0.056f);
			buttonIcon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			buttonIcon.color = Color.white;
			toggleHandler.onValueChanged = new Toggle.ToggleEvent();
			toggleHandler.onValueChanged.AddListener((Action<bool>)OnToggleClicked);
			buttonObject.transform.Find("Icon_On").gameObject.SetActive(value: false);
			UpdateToggleVisuals();
			SetToggleToolTip(tooltipOn, tooltipOff);
			SetToggleActions(actionOn, actionOff);
			SetIcons(QuickMenuUtils.GetToggleIconOn(), QuickMenuUtils.GetToggleIconOff());
			SetToggleState(state);
			SetActive(active: true);
		}

		internal void SetToggleToolTip(string tooltipOn, string tooltipOff)
		{
            //buttonObject.GetComponents<UIToggleTooltip>().ToList().ForEach(x => x.field_Public_String_0 = tooltipOn);
            //buttonObject.GetComponents<UIToggleTooltip>().ToList().ForEach(x => x.field_Public_String_1 = tooltipOff);

			toggleTooltip.field_Public_String_0 = tooltipOff; //Map missing UiToggleTooltip, probably due to Menu Update.
			toggleTooltip.field_Public_String_1 = tooltipOn;
		}

		internal void UpdateToggleVisuals()
		{
			buttonText.text = toggleText;
			buttonIcon.sprite = (toggleHandler.isOn ? toggleIconOn : toggleIconOff);
		}

		internal void SetToggleState(bool state)
		{
            if (state != toggleHandler.isOn)
			{
				toggleTempDisableActions = true;
				toggleHandler.Set(state);
				toggleTooltip.field_Private_Boolean_1 = !state; //Map missing UiToggleTooltip, probably due to Menu Update
				toggleTempDisableActions = false;
			}
			UpdateToggleVisuals();
		}

		internal void SetToggleActions(Action actionOn, Action actionOff)
		{
			toggleActionOn = actionOn;
			toggleActionOff = actionOff;
		}

		internal void OnToggleClicked(bool state)
		{
			if (state)
			{
				if (!toggleTempDisableActions)
				{
					toggleActionOn?.Invoke();
				}
			}
			else if (!toggleTempDisableActions)
			{
				toggleActionOff?.Invoke();
			}
			UpdateToggleVisuals();
		}

		internal void SetIcons(Sprite iconOn, Sprite iconOff)
		{
			toggleIconOn = iconOn;
			toggleIconOff = iconOff;
		}
	}
}
