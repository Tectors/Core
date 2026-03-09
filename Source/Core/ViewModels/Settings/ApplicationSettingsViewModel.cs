using System.Text.Json.Serialization;

using Avalonia;

using CommunityToolkit.Mvvm.ComponentModel;

using CUE4Parse.UE4.Versions;

using FluentAvalonia.UI.Media.Animation;

using Core.Framework.Models;

namespace Core.ViewModels.Settings;

public partial class ApplicationSettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _version = string.Empty;
    
    [ObservableProperty] private bool _completedOnboarding;
    [ObservableProperty] private bool _loadRecentProfileOnLaunch = true;
    [ObservableProperty] private bool _saveWindowResolution = true;
    [ObservableProperty] private PixelSize _lastWindowResolution;
    
    [ObservableProperty] private ELanguage _gameLanguage = ELanguage.English;
    [ObservableProperty] private bool _useTabTransitions = true;
    
    [JsonIgnore] public NavigationTransitionInfo Transition => UseTabTransitions
        ? new SlideNavigationTransitionInfo()
        : new SuppressNavigationTransitionInfo();
    
    partial void OnGameLanguageChanged(ELanguage value)
    {
        if (MainWM.CurrentProfile is null || MainWM.CurrentProfile.Provider is null) return;

        Tasks.Run(() => MainWM.CurrentProfile.SetLanguage(value));
    }
}
