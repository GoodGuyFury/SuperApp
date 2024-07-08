using OfficeOpenXml;
using UserAuthModel;

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
                                FullName = worksheet.Cells[row, 3].Text,
                                Role = worksheet.Cells[row, 5].Text,
                                UserId = worksheet.Cells[row, 1].Text,
                                Message = worksheet.Cells[row, 4].Text,
                                Email = worksheet.Cells[row, 2].Text
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

            return null; // Return null if user details not found
        }
    }
}
