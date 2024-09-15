using OfficeOpenXml;
using userAuthModel;

namespace UserDataRepository
{
    public class GetUserDataRepository
    {
        public  static UserInfo GetUserDetailsFromExcel(string UserEmail)
        {
            try
            {
                string filePath = "Main DB\\UserList.xlsx";
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var cellValue = worksheet.Cells[row, 2].Text;

                        if (cellValue == UserEmail)
                        {
                            var userDetails = new UserInfo
                            {
                                fullName = worksheet.Cells[row, 3].Text,
                                role = worksheet.Cells[row, 5].Text,
                                userId = worksheet.Cells[row, 1].Text,
                                message = worksheet.Cells[row, 4].Text,
                                email = worksheet.Cells[row, 2].Text
                            };

                            return userDetails;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading Excel file", ex);
            }

            return new UserInfo
            {
                message = "User not found",
                email = UserEmail
            }; // Return null if user details not found
        }
    }
}
