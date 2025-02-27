﻿using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MyMarkdown
    {
        private Dictionary<string, object>? _options;

        private IJSObjectReference module = default!;

        private MMarkdown MMarkdown = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? WrapClass { get; set; }

        private bool Show => _options is not null;

        protected async override Task OnInitializedAsync()
        {
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/vditor-helper.js");
            await SetOptions();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await AfterMarkdownRender();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task SetOptions()
        {
            string lang = await SettingsService.Get(SettingType.Language);
            lang = lang.Replace("-", "_");
            string theme = ThemeService.Dark ? "dark" : "light";
            var previewTheme = new Dictionary<string, object?>()
            {
                { "current", ThemeService.Dark ? "dark" : "light" },
                { "path", "npm/vditor/3.9.6/dist/css/content-theme" }
            };
            var preview = new Dictionary<string, object?>()
            {
                { "theme", previewTheme },
            };
            var link = new Dictionary<string, object?>()
            {
                { "isOpen", false }
            };
            var btnImage = new Dictionary<string, object?>()
            {
                {"hotkey", "⇧⌘I" },
                {"name", "image" },
                {"tipPosition", "n" },
                {"tip", I18n.T("Vditor.AddImage") },
                {"className", "" },
                {"icon", "<svg><use xlink:href=\"#vditor-icon-image\"></use></svg>" },
            };

            var btnAudio = new Dictionary<string, object?>()
            {
                {"hotkey","⇧⌘A" },
                {"name","audio" },
                {"tipPosition","n" },
                {"tip",I18n.T("Vditor.AddAudio") },
                {"className","" },
                {"icon","<svg><use xlink:href=\"#vditor-icon-audio\"></use></svg>" },
            };
            var btnVideo = new Dictionary<string, object?>()
            {
                {"hotkey","⇧⌘V" },
                {"name","video" },
                {"tipPosition","n" },
                {"tip",I18n.T("Vditor.AddVideo") },
                {"className","" },
                {"icon","<svg><use xlink:href=\"#vditor-icon-video\"></use></svg>" },
            };

            _options = new()
            {
                { "mode", "ir" },
                { "toolbar", new object[]{"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list" , "check", "code","inline-code","link",btnImage,btnAudio,btnVideo}},
                { "placeholder", I18n.T("Write.ContentPlace")! },
                { "cdn", "npm/vditor/3.9.6" },
                { "lang", lang },
                { "icon","material" },
                { "theme", theme },
                { "preview", preview },
                { "link", link }
            };
        }

        private async Task AfterMarkdownRender()
        {
            await Task.Delay(1000);
            await PreventInputLoseFocus();
        }

        private async Task PreventInputLoseFocus()
        {
            //点击工具栏不会丢失焦点
            await module.InvokeVoidAsync("preventInputLoseFocus", null);
        }

        private async void HandleToolbarButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "image":
                    await AddImageAsync();
                    break;
                case "audio":
                    await AddAudioAsync();
                    break;
                case "video":
                    await AddVideoAsync();
                    break;
            }
        }

        private async Task AddImageAsync()
        {
            string? path = await PlatformService.PickPhotoAsync();
            if (path == null)
            {
                return;
            }

            string uri = await AppDataService.CreateAppDataFileAsync(ResourceType.Image, path);
            string html = $"![]({uri})";
            await InsertValueAsync(html);
        }

        private async Task AddAudioAsync()
        {
            string? path = await PlatformService.PickAudioAsync();
            if (path == null)
            {
                return;
            }

            string uri = await AppDataService.CreateAppDataFileAsync(ResourceType.Audio, path);
            string html = $"<audio src=\"{uri}\" controls ></audio>";
            await InsertValueAsync(html);
        }

        private async Task AddVideoAsync()
        {
            string? path = await PlatformService.PickVideoAsync();
            if (path == null)
            {
                return;
            }

            string uri = await AppDataService.CreateAppDataFileAsync(ResourceType.Video, path);
            string html = $"<video src=\"{uri}\" controls ></video>";
            await InsertValueAsync(html);
        }

        public async Task InsertValueAsync(string value)
        {
            await MMarkdown.InsertValueAsync(value);
            await module.InvokeVoidAsync("moveCursorForward", value.Length);
        }
    }
}
