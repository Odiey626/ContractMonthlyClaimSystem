using System;

namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }
        public string ContractorName { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public DateTime ClaimDate { get; set; }

        // New fields for Part 2
        public string Status { get; set; }
        public string FileName { get; set; }

        public double TotalAmount
        {
            get { return HoursWorked * HourlyRate; }
        }

        // Constructor
        public Claim(int claimID, string contractorName, double hoursWorked, double hourlyRate, DateTime claimDate)
        {
            ClaimID = claimID;
            ContractorName = contractorName;
            HoursWorked = hoursWorked;
            HourlyRate = hourlyRate;
            ClaimDate = claimDate;
            Status = "Pending"; // Default status
        }

        public override string ToString()
        {
            return $"Claim ID: {ClaimID}, Contractor: {ContractorName}, Hours: {HoursWorked}, Rate: {HourlyRate}, Date: {ClaimDate.ToShortDateString()}, Total: {TotalAmount}, Status: {Status}";
        }
    }
}
