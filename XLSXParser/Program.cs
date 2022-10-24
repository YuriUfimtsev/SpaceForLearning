using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

const string fileName = "../../../Data.xlsx";
string[] parameters = { "Last Name", "First Name", "Second Name" };

string GetCellValue(Cell cell, SharedStringTable sharedStringTable)
{
    string result = string.Empty;
    if ( cell != null && cell.CellValue != null && cell.DataType != null && cell.DataType == CellValues.SharedString)
    {
        int sharedStringID = int.Parse(cell.CellValue.Text);
        var text = sharedStringTable.ChildElements[sharedStringID].InnerText;
        result = text;
    }

    else if (cell!= null && cell.CellValue != null)
    {
        result = cell.CellValue.Text;
    }

    return result;
}

Cell? GetCell(Row row, Column column, SharedStringTable sharedStringTable)
{
    foreach (Cell cell in row.Elements<Cell>())
    {
        if (column.Elements<Cell>().First() == cell)
        {
            return cell;
        }
    }

    return null;
}

List<List<Cell>> GetRequiredOrderedColumns(string[] orderedColumnNames, SharedStringTable sharedStringTable, SheetData sheetData)
{
    var columns = new List<List<Cell>>();
    for (var i = 0; i < orderedColumnNames.Length; ++i)
    {
        columns.Add(GetColumnByName(sharedStringTable, sheetData, orderedColumnNames[i]));
    }

    return columns;
}

List<Cell> GetColumnByNumber (SheetData sheetData, int columnNumber)
{
    List<Cell> columns = new List<Cell>();
    foreach (var row in sheetData.Elements<Row>())
    {
        var counter = 0;
        foreach (Cell cell in row)
        {
            if (counter == columnNumber)
            {
                columns.Add(cell);
            }

            ++counter;
        }
    }

    return columns;
}

List<Cell> GetColumnByName(SharedStringTable sharedStringTable, SheetData sheetData, string columnName)
{
    var columnNumber = 0;
    List<Cell> columns = new List<Cell>();
    foreach (Cell cell in sheetData.Elements<Row>().First())
    {
        if (Equals(GetCellValue(cell, sharedStringTable), columnName))
        {
            columns = GetColumnByNumber(sheetData, columnNumber);
        }

        ++columnNumber;
    }

    return columns;
}

using (FileStream filestream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
{
    using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filestream, false))
    {
        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

        SharedStringTablePart sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
        SharedStringTable sharedStringTable = sharedStringTablePart.SharedStringTable;

        var columnsWithRequiredData = GetRequiredOrderedColumns(parameters, sharedStringTable, sheetData);

        if (columnsWithRequiredData == null)
        {
            //smth
        }

        for (var i = 0; i < sheetData.Elements<Row>().Count(); ++i)
        {
            for (var j = 0; j < columnsWithRequiredData!.Count; ++j)
            {
                if (i != 0)
                {
                    Console.Write(GetCellValue(columnsWithRequiredData[j][i], sharedStringTable));
                    Console.Write(' ');
                }
            }
            
            if (i != 0)
            {
                Console.WriteLine();
            }
        }
    }
}
