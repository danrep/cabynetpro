using CabynetPro.EnumLibrary.Dictionary;

namespace CabynetPro.EnumLibrary
{
    public enum FileCategory : int
    {
        [EnumDisplayName(DisplayName = "Student Information")]
        StudentInfo = 1,
        [EnumDisplayName(DisplayName = "Student Academic Record")]
        StudentAcademicRecord,
        [EnumDisplayName(DisplayName = "Meeting Notes")]
        MeetingNotes,
        [EnumDisplayName(DisplayName = "Staff Information")]
        StaffInfo,
        [EnumDisplayName(DisplayName = "Financial Documments")]
        FinancialDoc,
        [EnumDisplayName(DisplayName = "Top Secret")]
        TopSecret
    }
}
