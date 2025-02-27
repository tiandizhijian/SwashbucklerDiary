﻿using BlazorComponent;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class AlertSettingPage : ImportantComponentBase
    {
        private int _timeout;

        private bool ShowTimeout;

        private bool AchievementsAlert;

        private readonly Dictionary<string, string> Timeouts = new();

        protected override async Task OnInitializedAsync()
        {
            SetTimeouts();
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private StringNumber Timeout
        {
            get => (_timeout / 1000).ToString();
            set => SetTimeout(value);
        }

        private async Task LoadSettings()
        {
            _timeout = await SettingsService.Get(SettingType.AlertTimeout);
            AchievementsAlert = await SettingsService.Get(SettingType.AchievementsAlert);
        }

        private async void SetTimeout(StringNumber value)
        {
            if(_timeout == value)
            {
                return;
            }

            _timeout = value.ToInt32() * 1000;
            AlertService.SetTimeout(_timeout);
            await SettingsService.Save(SettingType.AlertTimeout, _timeout);
        }

        private void SetTimeouts()
        {
            for (int i = 0; i < 5; i++)
            {
                Timeouts.Add((i + 1).ToString(), string.Empty);
            }
        }
    }
}
