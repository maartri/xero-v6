using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Xero.Core.Data.Entities
{
    public class UserInfo
    {
        [Key]
        public int Recnum { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Usertype { get; set; }
        public string StaffCode { get; set; }
        public string loginMode { get; set; }
        public string homeBranch { get; set; }
        public int? System { get; set; }
        public int? Recipients { get; set; }
        public int? Staff { get; set; }
        public int? Roster { get; set; }
        public int? DayManager { get; set; }
        public int? Timesheet { get; set; }
        public bool? TimesheetPreviousPeriod { get; set; }
        public int? Statistics { get; set; }
        public int? Financial { get; set; }
        public bool ReportPreview { get; set; }
        public int? InvoiceEnquiry { get; set; }
        public bool AllowTypeAhead { get; set; }
        public int? SuggestedTimesheets { get; set; }
        public bool? PayIntegrityCheck { get; set; }
        public int? TimesheetUpdate { get; set; }
        public bool AccessCDC { get; set; }
        //recipient tab
        public string RecipientRecordView { get; set; }
        public string StaffRecordView { get; set; }
        public bool DisableBtnReferral { get; set; }
        public bool DisableBtnReferOn { get; set; }
        public bool DisableBtnNotProceeding { get; set; }
        public bool DisableBtnAssess { get; set; }
        public bool DisableBtnAdmit { get; set; }
        public bool DisableBtnUnWait { get; set; }
        public bool DisableBtnDischarge { get; set; }
        public bool DisableBtnSuspend { get; set; }
        public bool DisableBtnReinstate { get; set; }
        public bool DisableBtnDecease { get; set; }
        public bool DisableBtnAdmin { get; set; }
        public bool DisableBtnItem { get; set; }
        public bool DisableBtnPrint { get; set; }
        public bool DisableBtnRoster { get; set; }
        public bool DisableBtnMaster { get; set; }
        public bool DisableBtnOnHold { get; set; }
        public bool? AddNewRecipient { get; set; }
        public bool? CanChangeClientCode { get; set; }
        public bool CanEditNDIA { get; set; }
        public int AllowProgramTransition { get; set; }
        // viewscope tab
        public bool ViewAllBranches { get; set; }
        public string ViewFilterBranches { get; set; }
        public bool ViewAllProgram { get; set; }
        public string ViewFilter { get; set; }
        public bool ViewAllCategories { get; set; }
        public string ViewFilterCategory { get; set; }
        public bool ViewAllCoordinators { get; set; }
        public string ViewFilterCoord { get; set; }
        public bool ViewAllReminders { get; set; }
        public string ViewFilterReminders { get; set; }
        public bool ViewAllStaffCategories { get; set; }
        public string ViewFilterStaffCategory { get; set; }
        //document tab
        public bool? CanMoveImportedDocuments { get; set; }
        public bool KeepOriginalAsImport { get; set; }
        public string RecipientDocFolder { get; set; }
        public bool Force_RecipDocFolder { get; set; }
        public string ONIImportExportFolder { get; set; }
        public bool Force_ONIImportFolder { get; set; }
        public string ONIArchiveFolder { get; set; }
        public bool Force_ONIArchiveFolder { get; set; }
        public string StaffDocFolder { get; set; }
        public bool Force_StaffDocFolder { get; set; }
        public string StaffRostersFolder { get; set; }
        public bool Force_StaffRosterFolder { get; set; }
        public string ReportExportFolder { get; set; }
        public bool Force_ReportExportFolder { get; set; }
        public string ReportSavesFolder { get; set; }
        public bool Force_ReportSavesFolder { get; set; }
        public string HACCMDSFolder { get; set; }
        public bool Force_HACCMDSFolder { get; set; }
        public string CSTDAMDSFolder { get; set; }
        public bool Force_CSTDAMDSFolder { get; set; }
        public string NRCPMDSFolder { get; set; }
        public bool Force_NRCPMDSFolder { get; set; }
        public string PayExportFolder { get; set; }
        public bool Force_PayExportFolder { get; set; }
        public string BillingExportFolder { get; set; }
        public bool Force_BillingExportFolder { get; set; }
        //Roster tab
        public bool ChangeMasterRoster { get; set; }
        public bool? AllowRosterReallocate { get; set; }
        public bool? AllowMasterSaveAs { get; set; }
        public int? ManualRosterCopy { get; set; }
        public int? AutoCopyRoster { get; set; }
        public bool? CanRosterOvertime { get; set; }
        public bool? CanRosterBreakless { get; set; }
        public bool? CanRosterConflicts { get; set; }
        public bool? EditRosterRecord { get; set; }
        public bool? OwnRosterOnly { get; set; }
        //dayManagerForm
        public bool? UseDMv2 { get; set; }
        public bool? APPROVEDAYMANAGER { get; set; }
        public bool? RECIPMGTVIEW { get; set; }
        public bool? AllowStaffSwap { get; set; }
        public bool AdminChangeOutputType { get; set; }
        public bool AdminChangeProgram { get; set; }
        public bool AdminChangeActivityCode { get; set; }
        public bool AdminChangePaytype { get; set; }
        public bool AdminChangeDebtor { get; set; }
        public bool AdminChangeBillAmount { get; set; }
        public bool adminChangeBillQty { get; set; }
        public bool AttendeesChangeBillAmount { get; set; }
        public bool AttendeesChangeBillQty { get; set; }
        public bool AdminChangePayQty { get; set; }
        public bool? LowChangeActivityCode { get; set; }
        //client portal tab
        public bool? AllowsMarketing { get; set; }
        public bool? ViewPackageStatement { get; set; }
        public bool? CanManagePreferences { get; set; }
        public bool? AllowBooking { get; set; }
        public bool? CanCreateBooking { get; set; }
        public int? BookingLeadTime { get; set; }
        public bool? CanChooseProvider { get; set; }
        public bool? ShowProviderPhoto { get; set; }
        public bool? CanSeeProviderPhoto { get; set; }
        public bool? CanSeeProviderGender { get; set; }
        public bool? CanSeeProviderAge { get; set; }
        public bool? CanSeeProviderReviews { get; set; }
        public bool? CanEditProviderReviews { get; set; }
        public bool? HideProviderName { get; set; }
        public bool? CanManageServices { get; set; }
        public bool? CanCancelService { get; set; }
        public bool? CanQueryService { get; set; }
        // main Menu Form 
        public int MMPublishPrintRosters { get; set; }
        public int MMTimesheetProcessing { get; set; }
        public int MMBilling { get; set; }
        public int MMPriceUpdates { get; set; }
        public int MMDexUploads { get; set; }
        public int MMNDIA { get; set; }
        public int MMHacc { get; set; }
        public int MMOtherDS { get; set; }
        public int MMAccounting { get; set; }
        public bool MMAnalyseBudget { get; set; }
        public bool MMAtAGlance { get; set; }
        // mOBILE FORM
        public bool? AllowTravelEntry { get; set; }
        public bool? AllowLeaveEntry { get; set; }

        public bool? AllowIncidentEntry { get; set; }
        public bool? AllowPicUpload { get; set; }
        public bool? EnableRosterAvailability { get; set; }
        public bool? AllowViewBookings { get; set; }
        public bool? AcceptBookings { get; set; }
        public bool ViewClientDocuments { get; set; }
        public bool ViewClientCareplans { get; set; }
        public bool? AllowViewGoalPlans { get; set; }
        public bool AllowTravelClaimWithoutNote { get; set; }
        public bool? AllowMTASaveUserPass { get; set; }

        public bool? AllowOPNote { get; set; }
        public bool? AllowCaseNote { get; set; }
        public bool AllowClinicalNoteEntry { get; set; }
        public bool? AllowRosterNoteEntry { get; set; }
        public bool SuppressEmailOnRosterNote { get; set; }
        public bool? EnableEmailNotification { get; set; }
        public bool UseOPNoteAsShiftReport { get; set; }
        public bool UseServiceNoteAsShiftReport { get; set; }
        public string EnableViewNoteCases { get; set; }
        public bool ShiftReportReminder { get; set; }
        public int? UserSessionLimit { get; set; }
        public int? MobileFutureLimit { get; set; }
        public string TMMode { get; set; }
        public bool? MTAAutRefreshOnLogin { get; set; }
        public bool HideClientPhoneInApp { get; set; }
        public bool? HideAddress { get; set; }
        public bool? AllowSetTime { get; set; }

        public bool? AllowAddAttendee { get; set; }
        public bool MultishiftAdminAndMultiple { get; set; }
        public bool RestrictTravelSameDay { get; set; }
        public bool PushPhonePrefix { get; set; }
        public string PhonePrefix { get; set; }
        public bool? Enable_Shift_End_Alarm { get; set; }
        public bool? Enable_Shift_Start_Alarm { get; set; }
        public int? CheckAlertInterval { get; set; }

        [Column("HidePortalBalance", TypeName = "bit")]
        [DefaultValue(false)]
        public bool? HidePortalBalance { get; set; }
        public DateTime? EndDate { get; set; }

        [Column("ShowAllRecipients", TypeName = "bit")]
        [DefaultValue(false)]
        public bool? ShowAllRecipients { get; set; }

        public string CustomDM2 { get; set; }
    }
}
