using MudBlazor;

namespace KNP.Server.Services;

public static class ThemePalettes
{
    private static MudTheme Build(string primary, string secondary, string info, string success)
    {
        return new MudTheme
        {
            PaletteDark = new PaletteDark
            {
                Primary = primary, Secondary = secondary, Tertiary = secondary,
                Background = "#08050f", BackgroundGray = "#0f0a1a", Surface = "#150e22",
                DrawerBackground = "#0f0a1a", AppbarBackground = "#0f0a1a", AppbarText = "#f5f3ff",
                TextPrimary = "#f5f3ff", TextSecondary = "#c4c4cc",
                ActionDefault = "#c4c4cc", ActionDisabled = "#52525b",
                Divider = "#2a1f3d", DividerLight = "#1e1530",
                LinesDefault = "#2a1f3d", LinesInputs = "#3a2b52",
                TableLines = "#2a1f3d", TableStriped = "#0f0a1a",
                Info = info, Success = success, Warning = "#f59e0b", Error = "#ef4444",
                Dark = "#0f0a1a",
            },
            PaletteLight = new PaletteLight
            {
                Primary = primary, Secondary = secondary, Tertiary = secondary,
                Background = "#f7f6fb", BackgroundGray = "#ecebf3", Surface = "#ffffff",
                DrawerBackground = "#ffffff", AppbarBackground = "#ffffff", AppbarText = "#1c1c1c",
                TextPrimary = "#1c1c1c", TextSecondary = "#52525b",
                ActionDefault = "#52525b", ActionDisabled = "#a1a1aa",
                Divider = "#d4d4d8", DividerLight = "#e4e4e7",
                LinesDefault = "#d4d4d8", LinesInputs = "#a1a1aa",
                TableLines = "#e4e4e7", TableStriped = "#fafafa",
                Info = info, Success = success, Warning = "#d97706", Error = "#dc2626",
                Dark = "#1c1c1c",
            },
            LayoutProperties = new LayoutProperties { DefaultBorderRadius = "10px" }
        };
    }

    public static MudTheme Purple => Build("#a855f7", "#ec4899", "#22d3ee", "#10b981");
    public static MudTheme Blue   => Build("#3b82f6", "#8b5cf6", "#06b6d4", "#10b981");
    public static MudTheme Green  => Build("#10b981", "#22d3ee", "#34d399", "#84cc16");
    public static MudTheme Pink   => Build("#ec4899", "#f43f5e", "#f97316", "#10b981");

    public static MudTheme For(string accent) => accent switch
    {
        "blue"  => Blue,
        "green" => Green,
        "pink"  => Pink,
        _       => Purple,
    };
}