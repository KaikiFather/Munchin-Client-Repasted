using MunchenClient.Core;
using MunchenClient.Misc;
using MunchenClient.Utils;
using UnchainedButtonAPI;

namespace MunchenClient.Menu
{
	internal class MediaControlsMenu : QuickMenuNestedMenu
	{
		internal MediaControlsMenu(QuickMenuButtonRow parent)
			: base(parent, LanguageManager.GetUsedLanguage().MediaControlsMenuName, LanguageManager.GetUsedLanguage().MediaControlsMenuDescription)
		{
			QuickMenuButtonRow parentRow = new QuickMenuButtonRow(this);
			new QuickMenuSpacers(this);
			QuickMenuButtonRow parentRow2 = new QuickMenuButtonRow(this);
			new QuickMenuSingleButton(parentRow, LanguageManager.GetUsedLanguage().MediaControlsPauseUnpause, delegate
			{
				UnmanagedUtils.PlayOrPause();
                UserInterface.WriteHudMessage(LanguageManager.GetUsedLanguage().MediaControlsPauseUnpauseClicked);
			}, LanguageManager.GetUsedLanguage().MediaControlsPauseUnpauseDescription);
			new QuickMenuSingleButton(parentRow, LanguageManager.GetUsedLanguage().MediaControlsNext, delegate
			{
				UnmanagedUtils.Next();
                UserInterface.WriteHudMessage(LanguageManager.GetUsedLanguage().MediaControlsNextClicked);
			}, LanguageManager.GetUsedLanguage().MediaControlsNextDescription);
			new QuickMenuSingleButton(parentRow, LanguageManager.GetUsedLanguage().MediaControlsPrevious, delegate
			{
				UnmanagedUtils.Previous();
                UserInterface.WriteHudMessage(LanguageManager.GetUsedLanguage().MediaControlsPreviousClicked);
			}, LanguageManager.GetUsedLanguage().MediaControlsPreviousDescription);
			new QuickMenuSingleButton(parentRow, LanguageManager.GetUsedLanguage().MediaControlsMuteUnmute, delegate
			{
				UnmanagedUtils.MuteOrUnmute();
                UserInterface.WriteHudMessage(LanguageManager.GetUsedLanguage().MediaControlsMuteUnmuteClicked);
			}, LanguageManager.GetUsedLanguage().MediaControlsMuteUnmuteDescription);
			new QuickMenuSingleButton(parentRow2, LanguageManager.GetUsedLanguage().MediaControlsVolumeDown, delegate
			{
				UnmanagedUtils.VolumeDown();
                UserInterface.WriteHudMessage(LanguageManager.GetUsedLanguage().MediaControlsVolumeDownClicked);
			}, LanguageManager.GetUsedLanguage().MediaControlsVolumeDownDescription);
			new QuickMenuSingleButton(parentRow2, LanguageManager.GetUsedLanguage().MediaControlsVolumeUp, delegate
			{
				UnmanagedUtils.VolumeUp();
                UserInterface.WriteHudMessage(LanguageManager.GetUsedLanguage().MediaControlsVolumeUpClicked);
			}, LanguageManager.GetUsedLanguage().MediaControlsVolumeUpDescription);
		}
	}
}
