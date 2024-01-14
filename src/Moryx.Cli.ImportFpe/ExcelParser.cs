using Npoi.Mapper;
using NPOI.SS.UserModel;

namespace Moryx.Cli.ImportFpe
{
    public static class ExcelParser
    {
        public static List<T> ParseExcel<T>(string path)
            where T : class
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = WorkbookFactory.Create(file);
            }

            var importer = new Mapper(workbook);
            var items = importer.Take<T>(0);
            return items.Select(ri => ri.Value).ToList();
        }
    }
}
