﻿using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Helper;

namespace MudBlazor.Extensions.Components;

/// <summary>
/// SplitPanelItem for MudExSplitPanel component
/// </summary>
public partial class MudExSplitPanelItem
{
    [CascadingParameter] public MudExSplitPanel Panel { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }

    [Parameter] public int MinSize { get; set; }
    [Parameter] public CssUnit SizeUnit { get; set; } = CssUnit.Pixels;


    private string GetClass()
    {
        return $"{(Panel?.ColumnLayout == true ? "mud-width-full" : "mud-height-full")}";
    }

    protected virtual string GetStyle()
    {
        return MudExStyleBuilder.GenerateStyleString(new
        {
            //Flex = 1,
            Overflow = "auto",
            MinWidth = Panel?.ColumnLayout == true ? MinSize.ToString() : "unset",
            MinHeight = !Panel?.ColumnLayout == true ? MinSize.ToString() : "unset",
        }, SizeUnit, Style);
    }
}