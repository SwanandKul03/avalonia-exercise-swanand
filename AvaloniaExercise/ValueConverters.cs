using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using AvaloniaExercise.Models;
using AvaloniaExercise.ViewModels;
using SystemColor = System.Drawing.Color;
using AvaloniaColor = Avalonia.Media.Color;

namespace AvaloniaExercise;

public static class ValueConverters
{
    public static FuncValueConverter<SystemColor, SolidColorBrush> ColorToBrush { get; } =
        new(c => new SolidColorBrush(new AvaloniaColor(c.A, c.R, c.G, c.B)));
}

public static class ThemeColors
{
    public static readonly Color SignalRed = Color.FromRgb(255, 72, 72);
    public static readonly Color SignalAmber = Color.FromRgb(255, 196, 46);
    public static readonly Color SignalGreen = Color.FromRgb(52, 211, 153);
    public static readonly Color Foreground = Color.FromRgb(224, 228, 235);
    public static readonly Color MutedForeground = Color.FromRgb(166, 174, 186);
    public static readonly Color Border = Color.FromRgb(240, 241, 245);

    public static SolidColorBrush Brush(Color c, double opacity = 1.0)
        => new(c) { Opacity = opacity };
}

public class SignalLightActiveConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is not CrossingVisualState state || values[1] is not string targetColor)
            return false;

        var activeColor = state switch
        {
            CrossingVisualState.Idle => "Green",
            CrossingVisualState.Requested => "Amber",
            CrossingVisualState.Active => "Red",
            CrossingVisualState.Cooldown => "Amber",
            _ => ""
        };
        return string.Equals(activeColor, targetColor, StringComparison.OrdinalIgnoreCase);
    }
}

public class SignalLightBrushConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is not CrossingVisualState state || values[1] is not string targetColor)
            return ThemeColors.Brush(ThemeColors.MutedForeground, 0.1);

        var activeColor = state switch
        {
            CrossingVisualState.Idle => "Green",
            CrossingVisualState.Requested => "Amber",
            CrossingVisualState.Active => "Red",
            CrossingVisualState.Cooldown => "Amber",
            _ => ""
        };

        bool isActive = string.Equals(activeColor, targetColor, StringComparison.OrdinalIgnoreCase);
        if (!isActive)
            return ThemeColors.Brush(GetColorForName(targetColor), 0.1);

        return ThemeColors.Brush(GetColorForName(targetColor));
    }

    private static Color GetColorForName(string name) => name switch
    {
        "Red" => ThemeColors.SignalRed,
        "Amber" => ThemeColors.SignalAmber,
        "Green" => ThemeColors.SignalGreen,
        _ => ThemeColors.MutedForeground
    };
}

public class SignalLabelBrushConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is not CrossingVisualState state || values[1] is not string targetColor)
            return ThemeColors.Brush(ThemeColors.MutedForeground, 0.55);

        var activeColor = state switch
        {
            CrossingVisualState.Idle => "Green",
            CrossingVisualState.Requested => "Amber",
            CrossingVisualState.Active => "Red",
            CrossingVisualState.Cooldown => "Amber",
            _ => ""
        };

        bool isActive = string.Equals(activeColor, targetColor, StringComparison.OrdinalIgnoreCase);
        if (!isActive)
            return ThemeColors.Brush(ThemeColors.MutedForeground, 0.55);

        return targetColor switch
        {
            "Red" => ThemeColors.Brush(ThemeColors.SignalRed),
            "Amber" => ThemeColors.Brush(ThemeColors.SignalAmber),
            "Green" => ThemeColors.Brush(ThemeColors.SignalGreen),
            _ => ThemeColors.Brush(ThemeColors.Foreground)
        };
    }
}

public class CountdownVisibilityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3 || values[0] is not CrossingVisualState state || values[1] is not int countdown || values[2] is not string targetColor)
            return false;

        if (countdown <= 0)
            return false;

        var activeColor = state switch
        {
            CrossingVisualState.Idle => "Green",
            CrossingVisualState.Requested => "Amber",
            CrossingVisualState.Active => "Red",
            CrossingVisualState.Cooldown => "Amber",
            _ => ""
        };

        return string.Equals(activeColor, targetColor, StringComparison.OrdinalIgnoreCase);
    }
}

public class IsActiveCrossingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingVisualState state && state == CrossingVisualState.Active;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class TriggerLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingVisualState state ? state switch
        {
            CrossingVisualState.Idle => "Request Crossing",
            CrossingVisualState.Requested => "Requested",
            CrossingVisualState.Active => "Walk",
            CrossingVisualState.Cooldown => "Clearing",
            _ => ""
        } : "";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class IsIdleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingVisualState state && state == CrossingVisualState.Idle;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class TriggerGlowConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is CrossingVisualState state ? state switch
        {
            CrossingVisualState.Idle => ThemeColors.SignalGreen,
            CrossingVisualState.Requested => ThemeColors.SignalAmber,
            CrossingVisualState.Active => ThemeColors.SignalRed,
            CrossingVisualState.Cooldown => ThemeColors.SignalAmber,
            _ => ThemeColors.MutedForeground
        } : ThemeColors.MutedForeground;
        return ThemeColors.Brush(color, 0.3);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class TriggerTextBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is CrossingVisualState state ? state switch
        {
            CrossingVisualState.Idle => ThemeColors.SignalGreen,
            CrossingVisualState.Requested => ThemeColors.SignalAmber,
            CrossingVisualState.Active => ThemeColors.SignalRed,
            CrossingVisualState.Cooldown => ThemeColors.SignalAmber,
            _ => ThemeColors.Foreground
        } : ThemeColors.Foreground;
        return ThemeColors.Brush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class NotIdleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingVisualState state && state != CrossingVisualState.Idle;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class VehicleStatusTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingVisualState state ? state switch
        {
            CrossingVisualState.Idle => "PROCEED",
            CrossingVisualState.Requested or CrossingVisualState.Cooldown => "CAUTION",
            CrossingVisualState.Active => "STOPPED",
            _ => ""
        } : "";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class VehicleStatusBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is CrossingVisualState state ? state switch
        {
            CrossingVisualState.Idle => ThemeColors.SignalGreen,
            CrossingVisualState.Requested or CrossingVisualState.Cooldown => ThemeColors.SignalAmber,
            CrossingVisualState.Active => ThemeColors.SignalRed,
            _ => ThemeColors.MutedForeground
        } : ThemeColors.MutedForeground;
        return ThemeColors.Brush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PedestrianStatusTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingVisualState state ? state == CrossingVisualState.Active ? "CROSSING" : "WAITING" : "WAITING";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PedestrianStatusBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => ThemeColors.Brush(value is CrossingVisualState state && state == CrossingVisualState.Active ? ThemeColors.SignalGreen : ThemeColors.SignalRed);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class CarBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CrossingVisualState state)
            return ThemeColors.Brush(ThemeColors.MutedForeground, 0.4);

        return state switch
        {
            CrossingVisualState.Idle => ThemeColors.Brush(ThemeColors.SignalGreen),
            CrossingVisualState.Requested or CrossingVisualState.Cooldown => ThemeColors.Brush(ThemeColors.SignalAmber),
            CrossingVisualState.Active => ThemeColors.Brush(ThemeColors.SignalRed, 0.4),
            _ => ThemeColors.Brush(ThemeColors.MutedForeground, 0.4)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PedestrianIconBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CrossingVisualState state)
            return ThemeColors.Brush(ThemeColors.SignalRed, 0.4);

        return state == CrossingVisualState.Active
            ? ThemeColors.Brush(ThemeColors.SignalGreen)
            : ThemeColors.Brush(ThemeColors.SignalRed, 0.4);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class ZebraOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingVisualState state && state == CrossingVisualState.Active ? 0.5 : 0.1;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PedBadgeTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is PedestrianStatus status ? status switch
        {
            PedestrianStatus.WaitingToCross => ThemeColors.SignalAmber,
            PedestrianStatus.Crossing => ThemeColors.SignalGreen,
            _ => ThemeColors.MutedForeground
        } : ThemeColors.MutedForeground;
        return ThemeColors.Brush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PedStatusLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is PedestrianStatus status ? status switch
        {
            PedestrianStatus.WaitingToCross => "Waiting",
            PedestrianStatus.Crossing => "Crossing",
            PedestrianStatus.Crossed => "Crossed",
            _ => ""
        } : "";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PeakHourBgConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? ThemeColors.Brush(ThemeColors.SignalAmber, 0.1) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.05);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PeakHourTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? ThemeColors.Brush(ThemeColors.SignalAmber) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.4);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class MiniSignalBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CrossingVisualState state || parameter is not string target)
            return ThemeColors.Brush(ThemeColors.MutedForeground, 0.1);

        bool vehiclesGo = state == CrossingVisualState.Idle;
        bool isAmber = state == CrossingVisualState.Requested || state == CrossingVisualState.Cooldown;
        bool isWalk = state == CrossingVisualState.Active;

        return target switch
        {
            "VehicleRed" => !vehiclesGo ? ThemeColors.Brush(ThemeColors.SignalRed) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.1),
            "VehicleAmber" => isAmber ? ThemeColors.Brush(ThemeColors.SignalAmber) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.1),
            "VehicleGreen" => vehiclesGo ? ThemeColors.Brush(ThemeColors.SignalGreen) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.1),
            "PedGreen" => isWalk ? ThemeColors.Brush(ThemeColors.SignalGreen) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.1),
            "PedRed" => !isWalk ? ThemeColors.Brush(ThemeColors.SignalRed) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.1),
            _ => ThemeColors.Brush(ThemeColors.MutedForeground, 0.1)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class ProgressRingStrokeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is CrossingVisualState state ? state switch
        {
            CrossingVisualState.Idle => ThemeColors.SignalGreen,
            CrossingVisualState.Requested => ThemeColors.SignalAmber,
            CrossingVisualState.Active => ThemeColors.SignalRed,
            CrossingVisualState.Cooldown => ThemeColors.SignalAmber,
            _ => ThemeColors.Border
        } : ThemeColors.Border;
        return ThemeColors.Brush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class PedestrianSpeciesIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is PedestrianSpecies species ? $"fa-solid {species.ToIconName()}" : "fa-solid fa-person";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
