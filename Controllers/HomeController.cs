using ContractMonthlyClaimSystem.Models;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        // In-memory list acting as a simple database
        public static List<Claim> claimList = new List<Claim>();

        // Homepage
        public IActionResult Home()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View(claimList);
        }

        // Submit claim(GET)
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        //Submit claim (Post)
        [HttpPost]
        public IActionResult Create(double hours, double rate, IFormFile document)
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            string? fileName = null;

            // Handle uploaded file
            if (document != null && document.Length > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                fileName = Path.GetFileName(document.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    document.CopyTo(stream);
                }
            }

            // Get contractor name from session
            string contractorName = HttpContext.Session.GetString("Username") ?? "Demo Contractor";

            // Create a new claim
            var newClaim = new Claim(
                claimList.Count + 1,
                contractorName,
                hours,
                rate,
                DateTime.Now
            )
            {
                FileName = fileName,
                Status = "Pending"
            };

            claimList.Add(newClaim);

            // Confirmation messages
            ViewBag.Hours = hours;
            ViewBag.Rate = rate;
            ViewBag.TotalAmount = newClaim.TotalAmount;
            ViewBag.FileMessage = fileName != null ? "File uploaded: " + fileName : "No file uploaded.";
            ViewBag.SuccessMessage = "Claim submitted successfully!";

            // Redirect to Claim history(Index) after submission
            return RedirectToAction("Index");
        }

        //  CLAIM HISTORY (for Contractor)
        public IActionResult History()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            string contractorName = HttpContext.Session.GetString("Username") ?? "Demo Contractor";
            var userClaims = claimList.Where(c => c.ContractorName == contractorName).ToList();

            return View(userClaims);
        }

        // COORDINATOR DASHBOARD
        public IActionResult CoordinatorDashboard()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View(claimList);
        }

        // APPROVE CLAIM
        [HttpPost]
        public IActionResult ApproveClaim(int id)
        {
            var claim = claimList.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Approved";
            }

            TempData["Message"] = $"Claim ID {id} has been approved ✔";
            return RedirectToAction("CoordinatorDashboard");
        }

        // REJECT CLAIM 
        [HttpPost]
        public IActionResult RejectClaim(int id)
        {
            var claim = claimList.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
            }

            TempData["Message"] = $"Claim ID {id} has been rejected ❌";
            return RedirectToAction("CoordinatorDashboard");
        }

        //  HR DASHBOARD
        public IActionResult HRDashboard()
        {
            if (HttpContext.Session.GetString("UserLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }

            var approvedClaims = claimList.Where(c => c.Status == "Approved").ToList();
            return View(approvedClaims);
        }
        // EXPORT APPROVED CLAIMS TO EXCEL 
        public IActionResult ExportToExcel()
        {
            var approvedClaims = claimList.Where(c => c.Status == "Approved").ToList();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Approved Claims");
                worksheet.Cell(1, 1).Value = "Claim ID";
                worksheet.Cell(1, 2).Value = "Contractor Name";
                worksheet.Cell(1, 3).Value = "Hours Worked";
                worksheet.Cell(1, 4).Value = "Hourly Rate";
                worksheet.Cell(1, 5).Value = "Total Amount";
                worksheet.Cell(1, 6).Value = "Claim Date";
                worksheet.Cell(1, 7).Value = "Status";

                int row = 2;
                foreach (var claim in approvedClaims)
                {
                    worksheet.Cell(row, 1).Value = claim.ClaimID;
                    worksheet.Cell(row, 2).Value = claim.ContractorName;
                    worksheet.Cell(row, 3).Value = claim.HoursWorked;
                    worksheet.Cell(row, 4).Value = claim.HourlyRate;
                    worksheet.Cell(row, 5).Value = claim.TotalAmount;
                    worksheet.Cell(row, 6).Value = claim.ClaimDate.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(row, 7).Value = claim.Status;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ApprovedClaims.xlsx");
                }
            }
        }

        // EXPORT APPROVED CLAIMS TO PDF
        public IActionResult ExportToPDF()
        {
            var approvedClaims = claimList.Where(c => c.Status == "Approved").ToList();

            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var titleFont = FontFactory.GetFont("Helvetica", 16, iTextSharp.text.Font.BOLD);
                var textFont = FontFactory.GetFont("Helvetica", 12, iTextSharp.text.Font.NORMAL);

                doc.Add(new iTextSharp.text.Paragraph("Approved Claims Report", titleFont));
                doc.Add(new iTextSharp.text.Paragraph("Generated on: " + DateTime.Now.ToString("dd MMM yyyy HH:mm"), textFont));
                doc.Add(new iTextSharp.text.Paragraph("\n"));

                var table = new iTextSharp.text.pdf.PdfPTable(6);
                table.AddCell("Claim ID");
                table.AddCell("Contractor Name");
                table.AddCell("Hours Worked");
                table.AddCell("Hourly Rate");
                table.AddCell("Total Amount");
                table.AddCell("Date");

                foreach (var claim in approvedClaims)
                {
                    table.AddCell(claim.ClaimID.ToString());
                    table.AddCell(claim.ContractorName);
                    table.AddCell(claim.HoursWorked.ToString());
                    table.AddCell(claim.HourlyRate.ToString("F2"));
                    table.AddCell(claim.TotalAmount.ToString("F2"));
                    table.AddCell(claim.ClaimDate.ToString("dd/MM/yyyy"));
                }

                doc.Add(table);
                doc.Close();

                return File(stream.ToArray(), "application/pdf", "ApprovedClaims.pdf");
            }
        }


        //  LOGIN 
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            // Simple demo login
            if (username == "test@example.com" && password == "12345")
            {
                HttpContext.Session.SetString("UserLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("Home");
            }

            ViewBag.Error = "Invalid login details!";
            return View();
        }

        //  REGISTER 
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string fullname, string email, string username, string password, string role)
        {
            // You can add database logic here later
            return RedirectToAction("Login");
        }

        //  LOGOUT 
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
