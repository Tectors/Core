using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using vj0.Framework.Models;
using vj0.Services;
using vj0.ViewModels;
using vj0.Views.Profiles;

namespace vj0.Views;

public partial class HomeView : ViewBase<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
        
        ViewModel.StartRotation(ViewModel.TagLines, 2700, RotatingTaglineText, useRandom: true);
        ViewModel.StartRotation(ViewModel.Tips, 2000, TipText, TipContainer, true);
    }

    private void OpenDiscord(object? sender, RoutedEventArgs e)
    {
        AppService.OpenLink(DISCORD_LINK);
    }

    private void OpenGitHub(object? sender, RoutedEventArgs e)
    {
        AppService.OpenLink(GITHUB_LINK);
    }

    private void OpenXAccount(object? sender, RoutedEventArgs e)
    {
        AppService.OpenLink(X_LINK);
    }

    private void OpenKoFi(object? sender, RoutedEventArgs e)
    {
        AppService.OpenLink(DONATE_LINK);
        Donate_OnPointerPressed(sender);
    }

    private void GetStarted(object? sender, RoutedEventArgs e)
    {
        Navigation.App.Open(typeof(ProfileSelectionView));
    }

    private void ExploreFiles(object? sender, RoutedEventArgs e)
    {
        MainWM.NavigateToExplorer();
    }

    static bool IsHeartAnimationPlaying;
    
    private async void Donate_OnPointerPressed(object? sender)
    {
        if (IsHeartAnimationPlaying) return;
        
        if (HeartScale?.RenderTransform is not ScaleTransform heartTransform)
        {
            return;
        }

        const double startScale = 1.0;
        const double endScale = 10.0;
        const double durationMs = 400;
        const int stepMs = 6;
        
        HeartScale.Opacity = 1.0;

        var startOpacity = HeartScale.Opacity;
        const double endOpacity = 0.0;

        double elapsed = 0;
        
        IsHeartAnimationPlaying = true;

        while (elapsed < durationMs)
        {
            var time = elapsed / durationMs;
            var eased = new CubicEaseOut().Ease(time);

            var currentScale = startScale + (endScale - startScale) * eased;
            var currentOpacity = startOpacity + (endOpacity - startOpacity) * eased;

            heartTransform.ScaleX = currentScale;
            heartTransform.ScaleY = currentScale;
            HeartScale.Opacity = currentOpacity;

            await Task.Delay(stepMs);
            
            elapsed += stepMs;
        }

        heartTransform.ScaleX = 1.0;
        heartTransform.ScaleY = 1.0;
        
        HeartScale.Opacity = 0.0;
        
        IsHeartAnimationPlaying = false;
    }

    private void Donate_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        Donate_OnPointerPressed(sender);
    }
    
    static bool IsLogoAnimationPlaying;
    
    private async void PlayLogoAnimation()
    {
        if (IsLogoAnimationPlaying) return;
        
        if (Logo?.RenderTransform is not ScaleTransform Transform)
        {
            return;
        }

        const double startScale = 1.0;
        const double endScale = 3.5;
        const double durationMs = 30;
        const int stepMs = 1;

        Logo.Opacity = 1.0;

        var startOpacity = Logo.Opacity;
        const double endOpacity = 0.0;

        double elapsed = 0;
        
        IsLogoAnimationPlaying = true;

        while (elapsed < durationMs)
        {
            var time = elapsed / durationMs;
            var eased = new CubicEaseOut().Ease(time);

            var currentScale = startScale + (endScale - startScale) * eased;
            var currentOpacity = startOpacity + (endOpacity - startOpacity) * eased;

            Transform.ScaleX = currentScale;
            Transform.ScaleY = currentScale;
            Logo.Opacity = currentOpacity;

            await Task.Delay(stepMs);
            
            elapsed += stepMs;
        }

        Transform.ScaleX = 1.0;
        Transform.ScaleY = 1.0;
        
        Logo.Opacity = 0.0;
        
        IsLogoAnimationPlaying = false;
    }
    
    private static readonly Random _random = new();

    private void InputElement_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (_random.NextDouble() <= 0.1)
        {
            PlayLogoAnimation();
        }
    }

    private void Visual_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        DispatcherTimer.RunOnce(() => PlayLogoAnimation(), TimeSpan.FromSeconds(0.1));
    }
}
