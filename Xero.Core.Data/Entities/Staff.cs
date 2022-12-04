using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xero.Core.Data.Entities
{
    public class Staff
    {
            [Key]
            public int SQLID { get; set; }
            public string? ABN { get; set; }
            public string? AccountNo { get; set; }
            public string? Address1 { get; set; }
            public string? Address2 { get; set; }
            [Column("Annual Leave Balance")]
            public double? AnnualLeaveBalance { get; set; }
            public string? Award { get; set; }
            public string? AwardLevel { get; set; }
            public int? BRID { get; set; }
            public string? CALDStatus { get; set; }
            public bool? CaseManager { get; set; }    
            public string? Category { get; set; }
            public string? CH_1_1 { get; set; }
            public string? CH_1_2 { get; set; }
            public string? CH_1_3 { get; set; }
            public string? CH_1_4 { get; set; }
            public string? CH_1_5 { get; set; } 
            public string? CH_1_6 { get; set; } 
            public string? CH_1_7 { get; set; } 
            public string? CH_2_1 { get; set; }
            public string? CH_2_2 { get; set; } 
            public string? CH_2_3 { get; set; }
            public string? CH_2_4 { get; set; }
            public string? CH_2_5 { get; set; }
            public string? CH_2_6 { get; set; }
            public string? CH_2_7 { get; set; }
            public int? COID { get; set; }
            public DateTime? CommencementDate { get; set; }
            public bool? CompetencyException { get; set; }

            [Column("Conflict of Interest")]
            public bool?  ConflictofInterest { get; set; }
            public string? Contact1 { get; set; }
            public string? Contact2 { get; set; }
            public string? Contact3 { get; set; }
            public string? ContactIssues { get; set; }
            public string? CSTDA_DISABILITYGROUP { get; set; }   
            public string? CSTDA_INDIGINOUS { get; set; }
            public string? CSTDA_OtherDisabilities { get; set; }
            public string? CurrentLocation { get; set; }
            public string? DaelibsID { get; set; }
            public string? DefaultCHGTravelBetweenPayType { get; set; }
            public string? DefaultCHGTravelWithinPayType { get; set; }
            public string? DefaultClientCancelPayType { get; set; }
            public string? DefaultLeavePayType { get; set; }
            public string? DefaultNCTravelBetweenActivity { get; set; }
            public string? DefaultNCTravelBetweenPayType { get; set; }
            public string? DefaultNCTravelBetweenProgram { get; set; }
            public string? DefaultNCTravelWithinActivity { get; set; }
            public string? DefaultNCTravelWithinPayType { get; set; }
            public string? DefaultNCTravelWithinProgram { get; set; }
            public string? DefaultUnallocateLeaveActivity { get; set; }
            public string? DefaultUnallocateLeavePayType { get; set; }
            public bool DeletedRecord { get; set; }
            public string? DLicence { get; set; }
            public DateTime? DOB { get; set; }
            public int? DPID { get; set; }
            public string? EMAIL { get; set; }
            public bool? EmailCompetencyReminders { get; set; }
            public bool? EmailTimesheet { get; set; }
            public string? EmployeeOf { get; set; }
            public bool ExcludeClientAdminFromPay { get; set; }
            public bool? ExcludeFromAwardInterpretation { get; set; }
            public bool ExcludeFromConflictChecking { get; set; }
            public bool? ExcludeFromInterpretation { get; set; }
            public bool? ExcludeFromPayExport { get; set; }
            public bool ExcludeFromTravelInterpretation { get; set; }
            public string? ExtEmployeeID { get; set; }
            public string? FilePhoto { get; set; }
            public string? FirstName { get; set; }

            public bool?  FLAG_EmailMessage { get; set; }
            public bool? FLAG_EmailSMSMessage { get; set; }
            public bool? FLAG_ExcludeFromLogDisplay { get; set; }
            public bool? FLAG_ExcludeShiftAlerts { get; set; }
            public bool? FLAG_InAppMessage { get; set; }
            public bool? FLAG_MessageOnRosterPublish { get; set; }
            public bool? FLAG_MessageOnShiftChange { get; set; }

            public string? Gender { get; set; }

            [Column("Holiday Group Description")]
            public string? HolidayGroupDescription { get; set; }
            public string? HRS_DAILY_MAX { get; set; }
            public string? HRS_DAILY_MIN { get; set; }
            public string? HRS_FNIGHTLY_MAX { get; set; }
            public string? HRS_FNIGHTLY_MIN { get; set; }
            public string? HRS_WEEKLY_MAX { get; set; }
            public string? HRS_WEEKLY_MIN { get; set; }

            public bool? IncludeInSchedule { get; set; }
            public bool IncludeLaundry { get; set; }
            public bool IncludeUniform { get; set; }
            public bool? InUse { get; set; }
            public bool? IsRosterable { get; set; }

            public double? JobFTE { get; set; }
            public string? JobStatus { get; set; }
            public string? JobTitle { get; set; }
            public double? JobWeighting { get; set; }
            public bool? KMAgainstTravelOnly { get; set; }
            public string? LastName { get; set; }
            public DateTime? LeaveReturnDate { get; set; }
            public DateTime? LeaveStartDate { get; set; }
            public string? MiddleNames { get; set; }
            public string? NDIAStaffLevel { get; set; }
            public string? NRegistration { get; set; }
            public bool OnLeave { get; set; }

            public Int16? PAN_AutoLogout { get; set; }
            public bool? PAN_ConfirmPin { get; set; }
            public bool? PAN_Custompin { get; set; }
            public bool? PAN_LoneWorker { get; set; }
            public bool? PAN_LoneWorkerManager { get; set; }

            public string? PAN_Manager { get; set; }
            public bool? PAN_ManualTimeEntryhhmm { get; set; }
            public bool? PAN_MsgingSupervisor { get; set; }
            public string? PAN_STATUS { get; set; }
            public bool? PAN_timeoffother { get; set; }
            public bool? PAN_Uploaded { get; set; }
            public string? PayGroup { get; set; }
            public int? PinCode { get; set; }
            public string? Postcode { get; set; }
            public string? PreferredName { get; set; }
            public string? Profile { get; set; }
            public string? PublicHolidayRegion { get; set; }
            public bool?  QueryPayTypeOnAllocate { get; set; }
            public string? Rating { get; set; }
            public string? ReservedBy { get; set; }

            public bool SB1 { get; set; } = false;
            public bool SB10 { get; set; } = false;
            public bool SB11 { get; set; } = false;
            public bool SB12 { get; set; } = false;
            public bool SB13 { get; set; } = false;
            public bool SB14 { get; set; } = false;
            public bool SB15 { get; set; } = false;
            public bool SB16 { get; set; } = false;
            public bool SB17 { get; set; } = false;
            public bool SB18 { get; set; } = false;
            public bool SB19 { get; set; } = false;
            public bool SB2 { get; set; } = false;
            public bool SB20 { get; set; } = false;
            public bool SB21 { get; set; } = false;
            public bool SB22 { get; set; } = false;
            public bool SB23 { get; set; } = false;
            public bool SB24 { get; set; } = false;
            public bool SB25 { get; set; } = false;
            public bool SB26 { get; set; } = false;
            public bool SB27 { get; set; } = false;
            public bool SB28 { get; set; } = false;
            public bool SB29 { get; set; } = false;
            public bool SB3 { get; set; } = false;
            public bool SB30 { get; set; } = false;
            public bool SB31 { get; set; } = false;
            public bool SB32 { get; set; } = false;
            public bool SB33 { get; set; } = false;
            public bool SB34 { get; set; } = false;
            public bool SB35 { get; set; } = false;
            public bool SB4 { get; set; } = false;
            public bool SB5 { get; set; } = false;
            public bool SB6 { get; set; } = false;
            public bool SB7 { get; set; } = false;
            public bool SB8 { get; set; } = false;
            public bool SB9 { get; set; } = false;

            public string? ServiceRegion { get; set; }
            [Column("Sick Leave Balance")]
            public double? SickLeaveBalance { get; set; }
            public string? StaffGroup { get; set; }
            public bool StaffReserverd { get; set; }
            public string? STAFFTEAM { get; set; }
            public int? StaffTimezoneOffset { get; set; }
            public string? STF_CODE { get; set; }
            public string? STF_DAELIBS_PIN { get; set; }
            public string? STF_DEPARTMENT { get; set; }
            public string? STF_NOTES { get; set; }
            public string? STF_PANZTEL_PIN { get; set; }
            public string? SubCategory { get; set; }
            public string? Suburb { get; set; }

            [Column("Suitable Duties", TypeName = "bit")]
            public bool? SuitableDuties { get; set; }

            public string? SuperFund { get; set; }
            public string? SuperPercent { get; set; }
            public string? TaxFileNumber { get; set; }
            public string? Telephone { get; set; }

            public DateTime? TerminationDate { get; set; }
            public string? Title { get; set; }
            public string? UBDMap { get; set; }
            public string? UBDRef { get; set; }
            public string? UnallocateLeaveTreatment { get; set; }
            public string? UniqueID { get; set; }
            public string? ViewGroupContact1 { get; set; }
            public string? VisaStatus { get; set; }
            public string? VRegistration { get; set; }
            public bool? Workcover { get; set; }
            public DateTime? xEndDate { get; set; }
    }
}
