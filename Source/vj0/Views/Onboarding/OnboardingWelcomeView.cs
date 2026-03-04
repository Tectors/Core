using System.Threading.Tasks;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using vj0.Windows;

namespace vj0.Views.Onboarding;

public partial class OnboardingWelcomeView : UserControl
{
    public OnboardingWelcomeView()
    {
        InitializeComponent();
    }

    private void OnStartClicked(object? sender, RoutedEventArgs e)
    {
        var window = VisualRoot as OnboardingWindow;
        window!.GoNext();
    }

    private bool IsLogoAnimationPlaying;
    
    private async void PlayLogoAnimation()
    {
        if (IsLogoAnimationPlaying) return;
        
        if (Logo?.RenderTransform is not ScaleTransform Transform)
        {
            return;
        }

        const double startScale = 1.0;
        const double endScale = 1.5;
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

    private void InputElement_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        PlayLogoAnimation();
    }
}
