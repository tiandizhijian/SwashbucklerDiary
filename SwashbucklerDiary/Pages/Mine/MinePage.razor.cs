﻿using Microsoft.AspNetCore.Components;
using Serilog;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class MinePage : ImportantComponentBase
    {
        private string? Language;

        private ThemeState ThemeState;

        private string? UserName;

        private string? Sign;

        private string? Avatar;

        private bool ShowLanguage;

        private bool ShowThemeState;

        private bool ShowFeedback;

        private bool ShowPreviewImage;

        private int DiaryCount;

        private long WordCount;

        private int ActiveDayCount;

        private static readonly Dictionary<string, ThemeState> ThemeStates = new()
        {
            {"ThemeState.System",ThemeState.System },
            {"ThemeState.Light",ThemeState.Light },
            {"ThemeState.Dark",ThemeState.Dark },
        };

        private Dictionary<string, List<DynamicListItem>> ViewLists = new();

        private List<DynamicListItem> FeedbackTypes = new();

        private Dictionary<string, string> FeedbackTypeDatas = new();

        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadViewAsync();
            await LoadSettings();
            await SetAvatar();
            await UpdateStatisticalData();
            await base.OnInitializedAsync();
        }

        protected override async Task OnResume()
        {
            await LoadSettings();
            await SetAvatar();
            await UpdateStatisticalData();
            await base.OnResume();
        }

        private void LoadView()
        {
            ViewLists = new()
            {
                {
                    "Mine.Data",
                    new()
                    {
                        new(this, "Mine.Backups","mdi-folder-sync-outline",() => To("backups")),
                        new(this, "Mine.Export","mdi-export",() => To("export")),
                        new(this, "Mine.Achievement.Name","mdi-trophy-outline",() => To("achievement")),
                    }
                },
                {
                    "Mine.Settings",
                    new()
                    {
                        new(this,"Mine.Settings","mdi-cog-outline",() => To("setting")),
                        new(this,"Mine.Languages","mdi-web",() => ShowLanguage = true),
                        new(this,"Mine.Night","mdi-weather-night",() => ShowThemeState = true),
                    }
                },
                {
                    "Mine.Other",
                    new()
                    {
                        new(this,"Mine.Feedback","mdi-email-outline",() => ShowFeedback = true),
                        new(this,"Mine.About","mdi-information-outline",() => To("about")),
                    }
                }
            };
            FeedbackTypes = new()
            {
                new(this, "Email","mdi-email-outline",SendMail),
                new(this, "Github","mdi-github",ToGithub),
                new(this, "QQGroup","mdi-qqchat",OpenQQGroup),
            };
        }

        private async Task LoadViewAsync()
        {
            FeedbackTypeDatas = await PlatformService.ReadJsonFileAsync<Dictionary<string, string>>("wwwroot/json/feedback-type/feedback-type.json");
        }

        private async Task LanguageChanged(string value)
        {
            Language = value;
            I18n.SetCulture(value);
            await SettingsService.Save(SettingType.Language, value);
            UserName = await SettingsService.Get(SettingType.UserName, I18n.T("AppName"));
            Sign = await SettingsService.Get(SettingType.Sign, I18n.T("Mine.Sign"));
        }

        private async Task SendMail()
        {
            var mail = FeedbackTypeDatas["Email"];
            try
            {
                if (PlatformService.IsMailSupported())
                {
                    List<string> recipients = new() { mail };

                    await PlatformService.SendEmail(recipients);
                }
                else
                {
                    await PlatformService.SetClipboard(mail);
                    await AlertService.Success(I18n.T("Mine.MailCopy"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Mine.SendMailFail"));
            }
        }

        private async Task ToGithub()
        {
            var githubUrl = FeedbackTypeDatas["Github"];
            await PlatformService.OpenBrowser(githubUrl);
        }

        private async Task ThemeStateChanged(ThemeState value)
        {
            ThemeState = value;
            ThemeService.SetThemeState(value);
            await SettingsService.Save(SettingType.ThemeState, (int)ThemeState);
        }

        private async Task OpenQQGroup()
        {
            var qqGroup = FeedbackTypeDatas["QQGroup"];
            try
            {
                bool flag = await PlatformService.OpenQQGroup();
                if (!flag)
                {
                    await PlatformService.SetClipboard(qqGroup);
                    await AlertService.Success(I18n.T("Mine.QQGroupCopy"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Mine.QQGroupError"));
            }
        }

        private Task Search(string? value)
        {
            To($"searchAppFunction?query={value}");
            return Task.CompletedTask;
        }

        private int GetWordCount(List<DiaryModel> diaries)
        {
            var type = (WordCountType)Enum.Parse(typeof(WordCountType), I18n.T("Write.WordCountType")!);
            return DiaryService.GetWordCount(diaries,type);
        }

        private async Task LoadSettings()
        {
            Language = await SettingsService.Get(SettingType.Language);
            UserName = await SettingsService.Get(SettingType.UserName, I18n.T("AppName"));
            Sign = await SettingsService.Get(SettingType.Sign, I18n.T("Mine.Sign"));
            int themeState = await SettingsService.Get(SettingType.ThemeState);
            ThemeState = (ThemeState)themeState;
        }

        private async Task SetAvatar()
        {
            Avatar = await SettingsService.Get(SettingType.Avatar);
        }

        private async Task UpdateStatisticalData()
        { 
            var diries = await DiaryService.QueryAsync(it => !it.Private);
            DiaryCount = diries.Count;
            WordCount = GetWordCount(diries);
            ActiveDayCount = diries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
        }
    }
}
