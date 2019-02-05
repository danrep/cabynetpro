
using CabynetPro.EnumLibrary.Dictionary;

namespace CabynetPro.EnumLibrary
{
    public enum UserStates : int
    {
        [EnumDisplayName(DisplayName = "Active")]
        Active = 1,
        [EnumDisplayName(DisplayName = "Inactive")]
        Inactive,
        [EnumDisplayName(DisplayName = "New")]
        New
    }
}
