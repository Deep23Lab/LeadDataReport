using Microsoft.AspNetCore.Mvc;
using LeadTask2.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using OfficeOpenXml;
using System.IO.Packaging;

public class LeadController : Controller
{
   // private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public LeadController(IConfiguration configuration)
    {
       // _context = context;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        // Fetch all leads data
        var leads = GetAllLeads();

        return View(leads);
    }
    
    //====================================================================================================================================
    //Get All Leads
    private IEnumerable<Lead> GetAllLeads()
    {
        var leads = new List<Lead>();
        var connectionString = _configuration.GetConnectionString("AppDbContextConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            using (var command = new SqlCommand("GetAllLeads", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var lead = new Lead
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Course = reader.GetString(reader.GetOrdinal("Course")),
                            SourceFrom = reader.GetString(reader.GetOrdinal("SourceFrom")),
                            Importance = reader.GetString(reader.GetOrdinal("Importance")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            Time = reader.GetTimeSpan(reader.GetOrdinal("Time")),
                            Location = reader.GetString(reader.GetOrdinal("Location")),
                            Type = reader.GetString(reader.GetOrdinal("Type")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            EmailValidate = reader.GetBoolean(reader.GetOrdinal("EmailValidate")),
                            PhoneValidate = reader.GetBoolean(reader.GetOrdinal("PhoneValidate")), 
                            Institution = reader.GetString(reader.GetOrdinal("Institution")),
                            Query = reader.GetString(reader.GetOrdinal("Query")),
                            Servicetype = reader.GetString(reader.GetOrdinal("Servicetype")),
                            Resolved = reader.GetBoolean(reader.GetOrdinal("Resolved")),
                            SyncStatus = reader.GetString(reader.GetOrdinal("SyncStatus")),
                            SyncTime = reader.GetDateTime(reader.GetOrdinal("SyncTime")),
                            BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                            BookingTime = reader.GetTimeSpan(reader.GetOrdinal("BookingTime"))
                        };
                        leads.Add(lead);
                    }
                }
            }
        }

        return leads;
    }

    //====================================================================================================================================

    [HttpPost]
    public IActionResult GenerateReport(DateTime fromDate, DateTime toDate)
    {
        var leads = GetLeadsByDateRange(fromDate, toDate);
        ViewBag.FilteredLeads = leads; // Store filtered leads in ViewBag

        return View("Index", leads); // Return full view with filtered leads
    }

    private IEnumerable<Lead> GetLeadsByDateRange(DateTime fromDate, DateTime toDate)
    {
        var leads = new List<Lead>();
        var connectionString = _configuration.GetConnectionString("AppDbContextConnection");

        using (var connection = new SqlConnection(connectionString))
        {
            using (var command = new SqlCommand("GetLeadsByDateRange", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                command.Parameters.Add(new SqlParameter("@ToDate", toDate));

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var lead = new Lead
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                            Course = reader.GetString(reader.GetOrdinal("Course")),
                            SourceFrom = reader.GetString(reader.GetOrdinal("SourceFrom")),
                            Importance = reader.GetString(reader.GetOrdinal("Importance")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            Time = reader.GetTimeSpan(reader.GetOrdinal("Time")),
                            Location = reader.GetString(reader.GetOrdinal("Location")),
                            Type = reader.GetString(reader.GetOrdinal("Type")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            EmailValidate = reader.GetBoolean(reader.GetOrdinal("EmailValidate")), 
                            PhoneValidate = reader.GetBoolean(reader.GetOrdinal("PhoneValidate")), 
                            Institution = reader.GetString(reader.GetOrdinal("Institution")),
                            Query = reader.GetString(reader.GetOrdinal("Query")),
                            Servicetype = reader.GetString(reader.GetOrdinal("Servicetype")),
                            Resolved = reader.GetBoolean(reader.GetOrdinal("Resolved")), // Adjusted for boolean
                            SyncStatus = reader.GetString(reader.GetOrdinal("SyncStatus")),
                            SyncTime = reader.GetDateTime(reader.GetOrdinal("SyncTime")),
                            BookingDate = reader.GetDateTime(reader.GetOrdinal("BookingDate")),
                            BookingTime = reader.GetTimeSpan(reader.GetOrdinal("BookingTime"))
                        };
                        leads.Add(lead);
                    }
                }
            }
        }

        return leads;
    }

    //==================================================================================================================================
    [HttpPost]
    public ActionResult DownloadExcel(DateTime? fromDate, DateTime? toDate)
    {
        var leads = new List<Lead>();

        if (fromDate.HasValue && toDate.HasValue)
        {
            leads = GetLeadsByDateRange(fromDate.Value, toDate.Value).ToList();
        }
        else
        {
            leads = GetAllLeads().ToList();
        }

        var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Leads");

        // Add header row
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Name";
        worksheet.Cells[1, 3].Value = "Email";
        worksheet.Cells[1, 4].Value = "PhoneNumber";
        worksheet.Cells[1, 5].Value = "Course";
        worksheet.Cells[1, 6].Value = "SourceFrom";
        worksheet.Cells[1, 7].Value = "Importance";
        worksheet.Cells[1, 8].Value = "Created Date";
        worksheet.Cells[1, 9].Value = "Time";
        worksheet.Cells[1, 10].Value = "Location";
        worksheet.Cells[1, 11].Value = "Type";
        worksheet.Cells[1, 12].Value = "State";
        worksheet.Cells[1, 13].Value = "EmailValidate";
        worksheet.Cells[1, 14].Value = "PhoneValidate";
        worksheet.Cells[1, 15].Value = "Institution";
        worksheet.Cells[1, 16].Value = "Query";
        worksheet.Cells[1, 17].Value = "Servicetype";
        worksheet.Cells[1, 18].Value = "Resolved";
        worksheet.Cells[1, 19].Value = "SyncStatus";
        worksheet.Cells[1, 20].Value = "SyncTime";
        worksheet.Cells[1, 21].Value = "BookingDate";
        worksheet.Cells[1, 22].Value = "BookingTime";

        // Add data rows
        for (int i = 0; i < leads.Count; i++)
        {
            var lead = leads[i];
            worksheet.Cells[i + 2, 1].Value = lead.ID;
            worksheet.Cells[i + 2, 2].Value = lead.Name;
            worksheet.Cells[i + 2, 3].Value = lead.Email;
            worksheet.Cells[i + 2, 4].Value = lead.PhoneNumber;
            worksheet.Cells[i + 2, 5].Value = lead.Course;
            worksheet.Cells[i + 2, 6].Value = lead.SourceFrom;
            worksheet.Cells[i + 2, 7].Value = lead.Importance;
            worksheet.Cells[i + 2, 8].Value = lead.CreatedDate.ToString("yyyy-MM-dd");
            worksheet.Cells[i + 2, 9].Value = lead.Time.ToString(@"hh\:mm");
            worksheet.Cells[i + 2, 10].Value = lead.Location;
            worksheet.Cells[i + 2, 11].Value = lead.Type;
            worksheet.Cells[i + 2, 12].Value = lead.State;
            worksheet.Cells[i + 2, 13].Value = lead.EmailValidate.ToString();
            worksheet.Cells[i + 2, 14].Value = lead.PhoneValidate.ToString();
            worksheet.Cells[i + 2, 15].Value = lead.Institution;
            worksheet.Cells[i + 2, 16].Value = lead.Query;
            worksheet.Cells[i + 2, 17].Value = lead.Servicetype;
            worksheet.Cells[i + 2, 18].Value = lead.Resolved;
            worksheet.Cells[i + 2, 19].Value = lead.SyncStatus;
            worksheet.Cells[i + 2, 20].Value = lead.SyncTime.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[i + 2, 21].Value = lead.BookingDate.ToString("yyyy-MM-dd");
            worksheet.Cells[i + 2, 22].Value = lead.BookingTime.ToString(@"hh\:mm");
        }

        // Create a MemoryStream to hold the file data
        var stream = new MemoryStream();
        package.SaveAs(stream);

        // Return the file as a download
        stream.Position = 0; // Reset stream position
        var fileName = $"Leads_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
    //=================================================================================================================================================================

   /* [HttpGet]
    public IActionResult DownloadExcel(DateTime fromDate, DateTime toDate)
    {
        var leads = GetLeadsByDateRange(fromDate, toDate);
        var excelFile = GenerateExcelFile(leads);

        // Generate a unique filename based on the date range
        var fileName = $"Leads_{fromDate:yyyyMMdd}_to_{toDate:yyyyMMdd}.xlsx";

        // Send the Excel file as a download
        return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    public byte[] GenerateExcelFile(IEnumerable<Lead> leads)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Leads");

            // Add column headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "PhoneNumber";
            worksheet.Cells[1, 5].Value = "Course";
            worksheet.Cells[1, 6].Value = "SourceFrom";
            worksheet.Cells[1, 7].Value = "Importance";
            worksheet.Cells[1, 8].Value = "Created Date";
            worksheet.Cells[1, 9].Value = "Time";
            worksheet.Cells[1, 10].Value = "Location";
            worksheet.Cells[1, 11].Value = "Type";
            worksheet.Cells[1, 12].Value = "State";
            worksheet.Cells[1, 13].Value = "EmailValidate";
            worksheet.Cells[1, 14].Value = "PhoneValidate";
            worksheet.Cells[1, 15].Value = "Institution";
            worksheet.Cells[1, 16].Value = "Query";
            worksheet.Cells[1, 17].Value = "Servicetype";
            worksheet.Cells[1, 18].Value = "Resolved";
            worksheet.Cells[1, 19].Value = "SyncStatus";
            worksheet.Cells[1, 20].Value = "SyncTime";
            worksheet.Cells[1, 21].Value = "BookingDate";
            worksheet.Cells[1, 22].Value = "BookingTime";

            int row = 2; // Start from the second row to leave the first row for headers

            foreach (var lead in leads)
            {
                worksheet.Cells[row, 1].Value = lead.ID;
                worksheet.Cells[row, 2].Value = lead.Name;
                worksheet.Cells[row, 3].Value = lead.Email;
                worksheet.Cells[row, 4].Value = lead.PhoneNumber;
                worksheet.Cells[row, 5].Value = lead.Course;
                worksheet.Cells[row, 6].Value = lead.SourceFrom;
                worksheet.Cells[row, 7].Value = lead.Importance;
                worksheet.Cells[row, 8].Value = lead.CreatedDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 9].Value = lead.Time.ToString(@"hh\:mm");
                worksheet.Cells[row, 10].Value = lead.Location;
                worksheet.Cells[row, 11].Value = lead.Type;
                worksheet.Cells[row, 12].Value = lead.State;
                worksheet.Cells[row, 13].Value = lead.EmailValidate;
                worksheet.Cells[row, 14].Value = lead.PhoneValidate;
                worksheet.Cells[row, 15].Value = lead.Institution;
                worksheet.Cells[row, 16].Value = lead.Query;
                worksheet.Cells[row, 17].Value = lead.Servicetype;
                worksheet.Cells[row, 18].Value = lead.Resolved;
                worksheet.Cells[row, 19].Value = lead.SyncStatus;
                worksheet.Cells[row, 20].Value = lead.SyncTime.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[row, 21].Value = lead.BookingDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 22].Value = lead.BookingTime.ToString(@"hh\:mm");

                row++;
            }

            return package.GetAsByteArray();
        }
    }*/
}
