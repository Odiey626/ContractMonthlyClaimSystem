using System;
using System.Collections.Generic;
using System.Linq;
using ContractMonthlyClaimSystem.Models;
namespace ContractMonthlyClaimsSystem.Controllers
{
    public class ClaimController
    {
        // List to store all claims
        private static List<Claim> claimList = new List<Claim>();

        // ✅ Add a new claim
        public void AddClaim(Claim claim)
        {
            claimList.Add(claim);
        }

        // ✅ Return all claims
        public List<Claim> GetAllClaims()
        {
            return claimList;
        }

        // ✅ Search for a claim by contractor name
        public List<Claim> SearchByContractor(string contractorName)
        {
            return claimList
                .Where(c => c.ContractorName != null &&
                            c.ContractorName.ToLower().Contains(contractorName.ToLower()))
                .ToList();
        }

        //  Delete a claim by ID
        public bool DeleteClaim(int claimID)
        {
            Claim claimToRemove = claimList.FirstOrDefault(c => c.ClaimID == claimID);
            if (claimToRemove != null)
            {
                claimList.Remove(claimToRemove);
                return true;
            }
            return false;
        }

        //  Calculate total amount of all claims
        public double GetTotalAmount()
        {
            return claimList.Sum(c => c.TotalAmount);
        }

        //  Validate claim data before adding
        public bool ValidateClaimData(double hoursWorked, double hourlyRate)
        {
            return hoursWorked > 0 && hourlyRate > 0;
        }
    }
}
