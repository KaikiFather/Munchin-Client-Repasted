using System;
using MunchenClient.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements.Tooltips;
using VRC.UI.Elements.Utilities;

namespace UnchainedButtonAPI
{
	internal class QuickMenuSliderButton : QuickMenuButtonBase
	{
		private Slider buttonSlider;

		private TextMeshProUGUI buttonTextSecondary;

		private Action<float> onValueChangedCallback;

		private bool callbackAllowed = true;

		internal QuickMenuSliderButton(QuickMenuNestedMenu parentRow, string text, float minValue, float maxValue, float starterValue, Action<float> action, string secondaryText, string tooltip)
		{
			buttonParentName = parentRow.GetMenuName();
			InitializeButton(parentRow.GetGameObject().transform.Find("ScrollRect/Viewport/VerticalLayoutGroup"), text, minValue, maxValue, starterValue, action, secondaryText, tooltip);
		}

		internal QuickMenuSliderButton(string parentRow, string text, float minValue, float maxValue, float starterValue, Action<float> action, string secondaryText, string tooltip)
		{
			buttonParentName = parentRow;
			//InitializeButton(QuickMenuUtils.GetQuickMenu().transform.Find(parentRow + "/ScrollRect/Viewport/VerticalLayoutGroup"), text, minValue, maxValue, starterValue, action, secondaryText, tooltip);
			InitializeButton(GameObject.Find(parentRow + "/ScrollRect/Viewport/VerticalLayoutGroup").transform, text, minValue, maxValue, starterValue, action, secondaryText, tooltip);
		}

		private void InitializeButton(Transform parent, string text, float minValue, float maxValue, float starterValue, Action<float> action, string secondaryText, string tooltip)
		{
			//todo add disable for noise suppresion button
            buttonObject = UnityEngine.Object.Instantiate(QuickMenuTemplates.GetSliderTemplate(), parent);
			buttonObject.name = $"Slider_{QuickMenuUtils.GetQuickMenuIdentifier()}{QuickMenuUtils.GetQuickMenuUniqueIdentifier()}";
			buttonObject.transform.Find("CurrentMic").gameObject.SetActive(value: false); //good
            buttonObject.transform.Find("Sliders/MicOutputVolume&NoiseSuppressionButton/InputVolumeSlider").gameObject.SetActive(value: false); //good?
			buttonObject.GetComponent<RectTransform>().sizeDelta = new Vector2(896f, 150f); //fine

            buttonText = buttonObject.transform.Find("Sliders/MicOutputVolume&NoiseSuppressionButton/InputVolumeSlider/Text_QM_H4").GetComponent<TextMeshProUGUI>();
            GameObject gameObject = buttonObject.transform.Find("Sliders/MicOutputVolume&NoiseSuppressionButton/InputVolumeSlider/Text_QM_H4 (1)").gameObject;
			//UnityEngine.Object.Destroy(gameObject.GetComponent<TextBinding>());
			buttonTextSecondary = gameObject.GetComponent<TextMeshProUGUI>();

            GameObject gameObject2 = buttonObject.transform.Find("Sliders/MicOutputVolume&NoiseSuppressionButton/InputVolumeSlider/SliderSnap").gameObject;
			//UnityEngine.Object.Destroy(gameObject2.GetComponent<SliderBinding>());
			//buttonTooltip = gameObject2.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            buttonSlider = gameObject2.GetComponent<SnapSlider>();
			buttonSlider.onValueChanged = new SnapSlider.SliderEvent();
			buttonSlider.onValueChanged.AddListener((Action<float>)OnValueChanged);

            SetSliderAction(action);
			SetSliderMinValue(minValue);
			SetSliderMaxValue(maxValue);
			SetSliderValue(starterValue, invokeAction: false);
			SetButtonText(text);
			SetSecondaryButtonText(secondaryText);
			//SetToolTip(tooltip);
			SetActive(active: true);
		}

		internal void SetSecondaryButtonText(string text)
		{
			buttonTextSecondary.text = text;
		}

		internal void SetSliderMinValue(float value)
		{
			buttonSlider.minValue = value;
		}

		internal void SetSliderMaxValue(float value)
		{
			buttonSlider.maxValue = value;
		}

		internal void SetSliderValue(float value, bool invokeAction = true)
		{
			callbackAllowed = invokeAction;
			buttonSlider.value = value;
			callbackAllowed = true;
		}

		internal void SetSliderAction(Action<float> onValueChanged)
		{
			onValueChangedCallback = onValueChanged;
		}

		private void OnValueChanged(float value)
		{
			if (callbackAllowed)
			{
				onValueChangedCallback(value);
			}
		}
	}
}
