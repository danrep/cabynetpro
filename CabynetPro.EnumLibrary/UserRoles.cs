using CabynetPro.EnumLibrary.Dictionary;

namespace CabynetPro.EnumLibrary
{
    public enum UserRoles : int
    {
        [EnumDisplayName(DisplayName = "System Administrator")]
        SystemAdministrator = 1,
        [EnumDisplayName(DisplayName = "Data Administrator")]
        DataAdministrator,
        [EnumDisplayName(DisplayName = "Staff")]
        Staff 
    }
}
