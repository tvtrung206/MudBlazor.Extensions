﻿using MudBlazor.Extensions.Attribute;
using MudBlazor.Extensions.Helper;
using MudBlazor.Utilities;
using OneOf;
using DC = System.Drawing.Color;

namespace MudBlazor.Extensions.Core;

/// <summary>
/// MudExColor is a readonly struct that provides a convenient and type-safe way to deal with color values in the context of MudBlazor.Extensions components.
/// </summary>
[HasDocumentation("MudExColor.md")]
public readonly struct MudExColor
{
    private readonly OneOf<Color, MudColor, DC, string, uint> _value;

    public MudExColor(OneOf<Color, MudColor, DC, string, uint> value)
    {
        _value = value.IsT3 switch
        {
            true when Enum.TryParse(value.AsT3, out Color color) => color,
            true when MudExColorUtils.TryParseFromHtmlColorName(value.AsT3, out var dc) => dc.ToMudColor(),
            _ => value
        };
    }

    public object Value => _value.Value;
    public bool IsColor => _value.IsT0;
    public bool IsMudColor => _value.IsT1;
    public bool IsDrawingColor => _value.IsT2;
    public bool IsString => _value.IsT3;
    public bool IsInt => _value.IsT4;

    public Color AsColor => _value.AsT0;
    public MudColor AsMudColor => _value.AsT1;
    public string AsString => _value.AsT3;
    public uint AsInt => _value.AsT4;
    public TResult Match<TResult>(Func<Color, TResult> f0, Func<MudColor, TResult> f1, Func<DC, TResult> f2, Func<string, TResult> f3, Func<uint, TResult> f4) => _value.Match(f0, f1, f2, f3, f4);
    public void Switch(Action<Color> f0, Action<MudColor> f1, Action<DC> f2, Action<string> f3, Action<uint> f4) => _value.Switch(f0, f1, f2, f3, f4);


    // Implicit conversions
    public static implicit operator MudExColor(Color c) => new MudExColor(c);
    public static implicit operator MudExColor(MudColor c) => new MudExColor(c);
    public static implicit operator MudExColor(string s) => new MudExColor(s);
    public static implicit operator MudExColor(uint i) => new MudExColor(i);

    public bool Is(Color c) => _value.Value.Equals(c);
    public bool Is(string c) => _value.Value.Equals(c);
    public bool Is(MudColor c) => _value.Value.Equals(c);
    public bool Is(uint c) => _value.Value.Equals(c);

    public override string ToString()
        => Match(
            color => color.ToString(),
            mudColor => mudColor.ToString(),
            dc => dc.ToString(),
            s => s,
            i => i.ToString()
        );

    /// <summary>
    /// Creates a css compatible string representation of the color.
    /// </summary>
    public string ToCssStringValue(MudColorOutputFormats format = MudColorOutputFormats.RGBA)
        => Match(
            color => color.CssVarDeclaration(),
            mudColor => mudColor.ToString(format),
            dc => dc.ToMudColor().ToString(format),
            s => s.ToLower().StartsWith("var") ? s : new MudColor(s).ToString(format),
            //s => new MudColor(s).ToString(format),
            i => FromUInt(i).ToString(format)
        );

    /// <summary>
    /// Creates a MudColor independent of what the underlying type is.
    /// </summary>
    public Task<MudColor> ToMudColorAsync()
        => Match(
            color => color.ToMudColorAsync(),
            Task.FromResult,
            dc => Task.FromResult(dc.ToMudColor()),
            s => Task.FromResult(new MudColor(s)),
            i => Task.FromResult(FromUInt(i))
        );

    /// <summary>
    /// Static helper method to list colors from current Theme
    /// </summary>
    public static async Task<IEnumerable<(string Name, MudColor Color)>> GetColorsFromThemeAsync(int count = 10)
    {
        var themeColors = await MudExCss.GetCssColorVariablesAsync();
        return themeColors
            //.Where(c => !c.Key.Contains("background", StringComparison.InvariantCultureIgnoreCase) && !c.Key.Contains("surface", StringComparison.InvariantCultureIgnoreCase) && !c.Value.IsBlack() && !c.Value.IsWhite() && c.Value.APercentage >= 1.0)
            .Select(x => (x.Key, x.Value))
            .Distinct()
            .Take(count);
    }


    internal static MudColor FromInt(int value)
    {
        return new MudColor((byte)((value >> 16) & 0xFF),
            (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF), (byte)((value >> 24) & 0xFF));
    }

    internal static MudColor FromUInt(uint value)
    {
        return new MudColor((byte)((value >> 16) & 0xFF),
            (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF), (byte)((value >> 24) & 0xFF));
    }

    

}