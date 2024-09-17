using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Linq;
using MySuparApp.Models.Admin;

namespace MySuparApp.Controllers.Admin
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private const string ExcelFilePath = "Main DB\\UserList.xlsx";

        static AdminController()
        {
            // If you're using EPPlus in a non-commercial context, set the LicenseContext
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [HttpPost("createuser")]
        public IActionResult CreateUser([FromForm] CreateUserModel model)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(ExcelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return BadRequest("Worksheet not found in the Excel file.");
                    }

                    int lastRow = worksheet.Dimension.End.Row;

                    worksheet.Cells[lastRow + 1, 1].Value = model.id;
                    worksheet.Cells[lastRow + 1, 2].Value = model.email;
                    worksheet.Cells[lastRow + 1, 3].Value = model.name;
                    worksheet.Cells[lastRow + 1, 4].Value = model.msg;
                    worksheet.Cells[lastRow + 1, 5].Value = model.role;

                    package.Save();
                }

                return Ok("User created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("fetchuserlist")]
        public IActionResult FetchUserList([FromQuery] string searchtext = null)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(ExcelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return BadRequest("Worksheet not found in the Excel file.");
                    }

                    // Read all rows starting from the second row (assuming first row is headers)
                    int rowCount = worksheet.Dimension.End.Row;
                    List<CreateUserModel> users = new List<CreateUserModel>();

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var user = new CreateUserModel
                        {
                            id = worksheet.Cells[row, 1].Value?.ToString(),
                            email = worksheet.Cells[row, 2].Value?.ToString(),
                            name = worksheet.Cells[row, 3].Value?.ToString(),
                            msg = worksheet.Cells[row, 4].Value?.ToString(),
                            role = worksheet.Cells[row, 5].Value?.ToString()
                        };
                        users.Add(user);
                    }

                    // If searchtext is "6699a98ll", return all users
                    if (searchtext == "6699a98ll")
                    {
                        return Ok(users);
                    }

                    // If searchtext is null or empty, return an empty list
                    if (string.IsNullOrEmpty(searchtext))
                    {
                        return Ok(new List<CreateUserModel>());
                    }

                    // Otherwise, filter the user list based on the searchtext
                    users = users.Where(u =>
                        (u.name != null && u.name.Contains(searchtext, StringComparison.OrdinalIgnoreCase)) ||
                        (u.email != null && u.email.Contains(searchtext, StringComparison.OrdinalIgnoreCase)) ||
                        (u.msg != null && u.msg.Contains(searchtext, StringComparison.OrdinalIgnoreCase)) ||
                        (u.role != null && u.role.Contains(searchtext, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    return Ok(users);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }

}