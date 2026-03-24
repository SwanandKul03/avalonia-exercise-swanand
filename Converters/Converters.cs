using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using RealTimePedestrianControlSystem.Models;

namespace RealTimePedestrianControlSystem.Converters;

// ─── Color constants matching index.css ───
public static class ThemeColors
{
    public static readonly Color SignalRed = Color.FromRgb(255, 72, 72);
    public static readonly Color SignalAmber = Color.FromRgb(255, 196, 46);
    public static readonly Color SignalGreen = Color.FromRgb(52, 211, 153);
    public static readonly Color Background = Color.FromRgb(10, 14, 21);
    public static readonly Color Foreground = Color.FromRgb(224, 228, 235);
    public static readonly Color MutedForeground = Color.FromRgb(166, 174, 186);
    public static readonly Color Border = Color.FromRgb(240, 241, 245);

    public static SolidColorBrush Brush(Color c, double opacity = 1.0)
        => new(c) { Opacity = opacity };
}

// ─── Signal light: is this specific light active? ───
// Parameters: [0]=CrossingState, [1]=target color string ("Red","Amber","Green")
public class SignalLightActiveConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is not CrossingStateEnum state || values[1] is not string targetColor)
            return false;

        var activeColor = state switch
        {
            CrossingStateEnum.Idle => "Green",
            CrossingStateEnum.Requested => "Amber",
            CrossingStateEnum.Active => "Red",
            CrossingStateEnum.Cooldown => "Amber",
            _ => ""
        };
        return string.Equals(activeColor, targetColor, StringComparison.OrdinalIgnoreCase);
    }
}

// ─── Signal light fill brush (active glow vs dim) ───
// Parameter: color name string
public class SignalLightBrushConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is not CrossingStateEnum state || values[1] is not string targetColor)
            return ThemeColors.Brush(ThemeColors.MutedForeground, 0.1);

        var activeColor = state switch
        {
            CrossingStateEnum.Idle => "Green",
            CrossingStateEnum.Requested => "Amber",
            CrossingStateEnum.Active => "Red",
            CrossingStateEnum.Cooldown => "Amber",
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

// ─── Countdown visibility: show only when active light has countdown > 0 ───
// Parameters: [0]=CrossingState, [1]=Countdown, [2]=target color
public class CountdownVisibilityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3 || values[0] is not CrossingStateEnum state ||
            values[1] is not int countdown || values[2] is not string targetColor)
            return false;

        if (countdown <= 0) return false;

        var activeColor = state switch
        {
            CrossingStateEnum.Idle => "Green",
            CrossingStateEnum.Requested => "Amber",
            CrossingStateEnum.Active => "Red",
            CrossingStateEnum.Cooldown => "Amber",
            _ => ""
        };
        return string.Equals(activeColor, targetColor, StringComparison.OrdinalIgnoreCase);
    }
}

// ─── State label text ───
public class StateLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => "Traffic Moving",
            CrossingStateEnum.Requested => "Request Pending",
            CrossingStateEnum.Active => "Crossing Active",
            CrossingStateEnum.Cooldown => "Resetting Signal",
            _ => ""
        } : "";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── State description text ───
public class StateDescriptionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => "Vehicles may proceed.",
            CrossingStateEnum.Requested => "Prepare to stop.",
            CrossingStateEnum.Active => "Pedestrians may cross.",
            CrossingStateEnum.Cooldown => "Wait for green.",
            _ => ""
        } : "";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Crossing trigger button label ───
public class TriggerLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => "Request Crossing",
            CrossingStateEnum.Requested => "Requested",
            CrossingStateEnum.Active => "Walk",
            CrossingStateEnum.Cooldown => "Clearing",
            _ => ""
        } : "";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Trigger button enabled only in idle ───
public class IsIdleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingStateEnum state && state == CrossingStateEnum.Idle;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Trigger button border/glow brush ───
public class TriggerGlowConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => ThemeColors.SignalGreen,
            CrossingStateEnum.Requested => ThemeColors.SignalAmber,
            CrossingStateEnum.Active => ThemeColors.SignalRed,
            CrossingStateEnum.Cooldown => ThemeColors.SignalAmber,
            _ => ThemeColors.MutedForeground
        } : ThemeColors.MutedForeground;
        return ThemeColors.Brush(color, 0.3);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Trigger text color ───
public class TriggerTextBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => ThemeColors.SignalGreen,
            CrossingStateEnum.Requested => ThemeColors.SignalAmber,
            CrossingStateEnum.Active => ThemeColors.SignalRed,
            CrossingStateEnum.Cooldown => ThemeColors.SignalAmber,
            _ => ThemeColors.Foreground
        } : ThemeColors.Foreground;
        return ThemeColors.Brush(color);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Show progress ring only when not idle ───
public class NotIdleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingStateEnum state && state != CrossingStateEnum.Idle;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Vehicle status text ───
public class VehicleStatusTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => "PROCEED",
            CrossingStateEnum.Requested or CrossingStateEnum.Cooldown => "CAUTION",
            CrossingStateEnum.Active => "STOPPED",
            _ => ""
        } : "";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Vehicle status brush ───
public class VehicleStatusBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var c = value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => ThemeColors.SignalGreen,
            CrossingStateEnum.Requested or CrossingStateEnum.Cooldown => ThemeColors.SignalAmber,
            CrossingStateEnum.Active => ThemeColors.SignalRed,
            _ => ThemeColors.MutedForeground
        } : ThemeColors.MutedForeground;
        return ThemeColors.Brush(c);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Pedestrian status text ───
public class PedestrianStatusTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Active => "CROSSING",
            _ => "WAITING"
        } : "WAITING";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Pedestrian status brush ───
public class PedestrianStatusBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var c = value is CrossingStateEnum state && state == CrossingStateEnum.Active
            ? ThemeColors.SignalGreen : ThemeColors.SignalRed;
        return ThemeColors.Brush(c);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Car icon brush (matches vehicleColor) ───
public class CarBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CrossingStateEnum state) return ThemeColors.Brush(ThemeColors.MutedForeground, 0.4);
        return state switch
        {
            CrossingStateEnum.Idle => ThemeColors.Brush(ThemeColors.SignalGreen),
            CrossingStateEnum.Requested or CrossingStateEnum.Cooldown => ThemeColors.Brush(ThemeColors.SignalAmber),
            CrossingStateEnum.Active => ThemeColors.Brush(ThemeColors.SignalRed, 0.4),
            _ => ThemeColors.Brush(ThemeColors.MutedForeground, 0.4)
        };
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Pedestrian icon brush ───
public class PedestrianIconBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CrossingStateEnum state) return ThemeColors.Brush(ThemeColors.SignalRed, 0.4);
        return state == CrossingStateEnum.Active
            ? ThemeColors.Brush(ThemeColors.SignalGreen)
            : ThemeColors.Brush(ThemeColors.SignalRed, 0.4);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Zebra stripe opacity ───
public class ZebraOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is CrossingStateEnum state && state == CrossingStateEnum.Active ? 0.5 : 0.1;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Pedestrian badge text brush ───
public class PedBadgeTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var c = value is PedestrianStatus s ? s switch
        {
            PedestrianStatus.Waiting => ThemeColors.SignalAmber,
            PedestrianStatus.Crossing => ThemeColors.SignalGreen,
            _ => ThemeColors.MutedForeground
        } : ThemeColors.MutedForeground;
        return ThemeColors.Brush(c);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Pedestrian status label ───
public class PedStatusLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is PedestrianStatus s ? s switch
        {
            PedestrianStatus.Waiting => "Waiting",
            PedestrianStatus.Crossing => "Crossing",
            PedestrianStatus.Cleared => "Cleared",
            _ => ""
        } : "";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Peak hour background ───
public class PeakHourBgConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? ThemeColors.Brush(ThemeColors.SignalAmber, 0.1) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.05);
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Peak hour text ───
public class PeakHourTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? ThemeColors.Brush(ThemeColors.SignalAmber) : ThemeColors.Brush(ThemeColors.MutedForeground, 0.4);
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

// ─── Mini signal light in crossing zone header ───
// Parameter: "VehicleRed", "VehicleAmber", "VehicleGreen", "PedGreen", "PedRed"
public class MiniSignalBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CrossingStateEnum state || parameter is not string target)
            return ThemeColors.Brush(ThemeColors.MutedForeground, 0.1);

        bool vehiclesGo = state == CrossingStateEnum.Idle;
        bool isAmber = state == CrossingStateEnum.Requested || state == CrossingStateEnum.Cooldown;
        bool isWalk = state == CrossingStateEnum.Active;

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

// ─── Progress ring stroke brush ───
public class ProgressRingStrokeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var c = value is CrossingStateEnum state ? state switch
        {
            CrossingStateEnum.Idle => ThemeColors.SignalGreen,
            CrossingStateEnum.Requested => ThemeColors.SignalAmber,
            CrossingStateEnum.Active => ThemeColors.SignalRed,
            CrossingStateEnum.Cooldown => ThemeColors.SignalAmber,
            _ => ThemeColors.Border
        } : ThemeColors.Border;
        return ThemeColors.Brush(c);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
