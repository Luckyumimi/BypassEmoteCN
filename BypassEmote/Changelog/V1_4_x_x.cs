using Dalamud.Interface;
using NoireLib.Changelog;
using NoireLib.Helpers;
using System.Collections.Generic;

namespace BypassEmote.Changelog.Versions;

public class V1_4_x_x : BaseChangelogVersion
{
    public override List<ChangelogVersion> GetVersions() => new()
    {
        CreateV1_4_0_0(),
        CreateV1_4_1_0(),
    };

    private static ChangelogVersion CreateV1_4_0_0()
    {
        var pastelPink = ColorHelper.HexToVector4("#ffa3d4ff");
        return new ChangelogVersion
        {
            Version = new(1, 4, 0, 0),
            Date = "16-09-2025",
            Title = L.T("Various QoLs and Changelog System"),
            TitleColor = Blue,
            Description = L.T("Major update introducing a comprehensive changelog system with UI integration, configuration tracking, and enhanced user experience features."),
            Entries = new List<ChangelogEntry>
            {
                Header(L.T("Changelog system"), Orange, 0, FontAwesomeIcon.Book),
                EntryBullet(L.T("Introduced ChangelogWindow UI for viewing updates"), Blue, 1),
                    EntryBullet(L.T("Accessible via main plugin window book button"), null, 2),
                    EntryBullet(L.T("Clean, organized display of version history"), null, 2),
                    EntryBullet(L.T("Interactive elements and color-coded entries"), null, 2),

                EntryBullet(L.T("Comprehensive changelog management system"), null, 1),
                    EntryBullet(L.T("Versioned changelog entries with structured data"), null, 2),
                    EntryBullet(L.T("Automatic changelog version tracking"), null, 2),
                    EntryBullet(L.T("Support for rich formatting and icons"), null, 2),

                Separator(),

                Header(L.T("Added new emote data collected from FFXIVCollect"), Orange, 0, FontAwesomeIcon.Database),
                    EntryBullet(L.T("Added which patch the emote is from"), null, 1),
                    EntryBullet(L.T("Added the obtention methods to get the emote"), null, 1),

                Separator(),

                Header(L.T("Configuration & Settings"), Orange, 0, FontAwesomeIcon.Cog),
                EntryBullet(L.T("Updated configuration system to track changelog versions"), null, 1),
                    EntryBullet(L.T("Show changelog on update"), Blue, 2),
                    EntryBullet(L.T("Last seen changelog version tracking"), null, 2),
                EntryBullet(L.T("Added an option to show update notifications"), Blue, 1),

                Separator(),

                Header(L.T("User Interface"), Orange, 0, FontAwesomeIcon.Eye),
                EntryBullet(L.T("Enhanced main plugin window with new action buttons in the title bar"), null, 1),
                    EntryBullet(L.T("Added changelog button in title bar"), Blue, 2),
                    EntryBullet(L.T("New settings access button"), Blue, 2),
                    Button(L.T("Integrated support button (Ko-Fi)"), pastelPink, L.T("Donate"), null, pastelPink, (m) => { Service.OpenKofi();  }, 1, FontAwesomeIcon.Donate, pastelPink),

                Entry(L.T("Added favorited emotes feature"), Orange, 1, FontAwesomeIcon.Star, Orange),
                    EntryBullet(L.T("Easily access frequently used emotes"), null, 2),
                    EntryBullet(L.T("Mark/unmark emotes as favorites"), null, 2),
                    EntryBullet(L.T("View all favorited emotes in the \"Fav\" tab"), null, 2),

                Entry(L.T("Added an info circle you can hover to see what patch the emote is from, and how to obtain it"), Blue, 1, FontAwesomeIcon.InfoCircle, White),
            }
        };
    }

    private static ChangelogVersion CreateV1_4_1_0()
        => new ChangelogVersion
        {
            Version = new(1, 4, 1, 0),
            Date = "08-10-2025",
            Title = L.T("Improved Emote Detection"),
            TitleColor = Blue,
            Description = L.T("Fixed an incorrect detection of game emotes."),
            Entries = new List<ChangelogEntry>
            {
                Header(L.T("Improved Emote Detection"), Orange),
                EntryBullet(L.T("The game will now display Patch 7.35 game emotes."), Orange, 1),
            }
        };
}