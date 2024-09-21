using AuthModel;
using OfficeOpenXml;

namespace GetUserDataRepository
{
    public class GetUserData
    {
        public static UserModel GetUserDetailsFromExcel(string UserEmail = "", string userId = "", string pass = "")
        {
            try
            {
                string filePath = "Main DB\\UserList.xlsx";
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // Check if email is provided
                    if (!string.IsNullOrEmpty(UserEmail))
                    {
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var cellValue = worksheet.Cells[row, 2].Text;

                            if (cellValue == UserEmail)
                            {
                                var userDetails = new UserModel
                                {
                                    FirstName = worksheet.Cells[row, 3].Text,
                                    Role = worksheet.Cells[row, 5].Text,
                                    UserId = worksheet.Cells[row, 1].Text,
                                    LastName = worksheet.Cells[row, 4].Text,
                                    Email = worksheet.Cells[row, 2].Text
                                };

                                return userDetails;
                            }
                        }
                    }

                    // Check if userId and password are provided
                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(pass))
                    {
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var idValue = worksheet.Cells[row, 1].Text;
                            var passValue = worksheet.Cells[row, 6].Text; // Assuming password is in column F (6th column)

                            if (idValue == userId && passValue == pass)
                            {
                                var userDetails = new UserModel
                                {
                                    FirstName = worksheet.Cells[row, 3].Text,
                                    Role = worksheet.Cells[row, 5].Text,
                                    UserId = worksheet.Cells[row, 1].Text,
                                    LastName = worksheet.Cells[row, 4].Text,
                                    Email = worksheet.Cells[row, 2].Text
                                };

                                return userDetails;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading Excel file", ex);
            }

            return new UserModel
            {
                LastName = "User not found",
                Email = UserEmail
            }; // Return null if user details not found
        }

    }
}
