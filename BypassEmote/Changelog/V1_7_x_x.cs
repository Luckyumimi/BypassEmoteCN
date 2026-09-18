using Dalamud.Interface;
using NoireLib.Changelog;
using System.Collections.Generic;

namespace BypassEmote.Changelog.Versions;

public class V1_7_x_x : BaseChangelogVersion
{
    public override List<ChangelogVersion> GetVersions() => new()
    {
        CreateV1_7_2_0(),
        CreateV1_7_3_1(),
    };

    private static ChangelogVersion CreateV1_7_2_0()
        => new ChangelogVersion
        {
            Version = new(1, 7, 2, 0),
            Date = "03-04-2026",
            Title = L.T("Emote handling enhancement"),
            TitleColor = Blue,
            Description = L.T("Enhances the target handling and the emote bypassing."),
            Entries = new List<ChangelogEntry>
            {
                Header(L.T("New Features"), Orange, 0, FontAwesomeIcon.Book),
                EntryBullet(L.T("BypassEmote now handles hotbar emote slots. If you click a locked emote hotbar slot (greyed out), the emote will play."), Orange, 1),
                Separator(),
                Header(L.T("Bug fixes"), LightRed, 0, FontAwesomeIcon.Bug),
                EntryBullet(L.T("The soft target is now properly handled. If you have both a target and a soft target, bypassing an emote will prioritize the soft target."), White, 1),
                EntryBullet(L.T("Various bug fixes over the versions."), White, 1),
                Separator(),
                Header(L.T("Technical Changes"), Blue, 0, FontAwesomeIcon.Wrench),
                EntryBullet(L.T("Reworked the IPC Data being sent to consumers."), White, 1),
                EntryBullet(L.T("Various technical enhancements."), White, 1),
            }
        };

    private static ChangelogVersion CreateV1_7_3_1()
        => new ChangelogVersion
        {
            Version = new(1, 7, 3, 1),
            Date = "05-04-2026",
            Title = L.T("Hotbar integration enhancement"),
            TitleColor = Blue,
            Description = L.T("Allows to add emotes to hotbars."),
            Entries = new List<ChangelogEntry>
            {
                Header(L.T("New Features"), Orange, 0, FontAwesomeIcon.Book),
                EntryBullet(L.T("Added the possibility to assign emotes to hotbar slots.\nRight click an emote in the main UI, and an option will appear if the emote is assignable."), Orange, 1),
                EntryBullet(L.T("Added a new config option for enabling/disabling bypassing emotes on emote hotbar slot click."), White, 1),
            }
        };
}