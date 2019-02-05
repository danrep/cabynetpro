
using CabynetPro.EnumLibrary.Dictionary;

namespace CabynetPro.EnumLibrary
{
    public enum AuditStates : int
    {
        [EnumDisplayName(DisplayName = "Upload")]
        Upload = 1,
        [EnumDisplayName(DisplayName = "Download")]
        Download,
        [EnumDisplayName(DisplayName = "Delete")]
        Delete,
        [EnumDisplayName(DisplayName = "Share")]
        Share,
        [EnumDisplayName(DisplayName = "Unshare")]
        Unshare
    }
}
