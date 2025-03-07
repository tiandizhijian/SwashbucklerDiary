﻿using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Repository;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomIOC(this IServiceCollection services)
        {
            //仓储相关
            services.AddSingleton<IDiaryRepository,DiaryRepository>();
            services.AddSingleton<ITagRepository,TagRepository>();
            services.AddSingleton<IUserAchievementRepository, UserAchievementRepository>();
            services.AddSingleton<IUserStateModelRepository, UserStateModelRepository>();
            services.AddSingleton<ILocationRepository, LocationRepository>();
            services.AddSingleton<ILogRepository, LogRepository>();
            services.AddSingleton<IResourceRepository, ResourceRepository>();
            //数据服务相关
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<IDiaryService, DiaryService>();
            services.AddSingleton<IAchievementService, AchievementService>();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IResourceService, ResourceService>();
            //功能服务相关
            services.AddSingleton<INavigateService, NavigateService>();
            services.AddSingleton<IIconService,IconService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<II18nService, I18nService>();
            services.AddSingleton<IPlatformService, PlatformService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IVersionService, VersionService>();
            services.AddSingleton<IAppDataService, AppDataService>();
            services.AddSingleton<ILANService, LANService>();
            services.AddScoped<IScreenshotService, ScreenshotService>();
            return services;
        }
    }
}
