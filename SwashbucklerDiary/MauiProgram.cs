﻿using CommunityToolkit.Maui;
using MauiBlazorToolkit;
using Microsoft.AspNetCore.Components.WebView.Maui;
using SwashbucklerDiary.Extensions;

namespace SwashbucklerDiary
{

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiBlazorToolkit(options =>
                {
                    options.HiddenMacTitleVisibility = true;
                    options.WebViewSoftInputPatch = true;
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureEssentials(essentials =>
                {
                    essentials.UseVersionTracking();
                });


            builder.Services.AddMauiBlazorWebView();

#if IOS || MACCATALYST
            builder.Services.ConfigureMauiHandlers(static handlers => handlers.AddHandler<IBlazorWebView, MyBlazorWebViewHandler>());
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddSerilogConfig();
            builder.Services.AddMasaBlazorConfig();
            builder.Services.AddSqlsugarConfig();
            builder.Services.AddCustomIOC();
            builder.Services.AddIconCache();
            builder.Services.AddMauiExceptionHandle();

            return builder.Build();
        }
    }
}
