
using System.Data.Common;

namespace FlightApp
{
    internal static class ConsoleOutput
    {
        public static void PrintData(IEnumerable<string> data)
        {
            foreach (var info in data)
            {
                Console.WriteLine(info);
            }
        }

        internal static void PrintTable(QueryTableResult queryTable)
        {
            var columnSizes = queryTable.Columns.ToDictionary(
                c => c.ColumnName,
                c => Math.Max(
                        queryTable.Rows.Max(r => r.Cells.TryGetValue(c.ColumnName, out var cellData) ? cellData.Length : 0),
                        c.ColumnName.Length)
                );

            Console.WriteLine();
            PrintHeader(queryTable.Columns, columnSizes);
            PrintRowSeparator(queryTable.Columns, columnSizes);
            PrintRows(queryTable.Columns, queryTable.Rows, columnSizes);
            Console.WriteLine();
        }

        private static void PrintHeader(IEnumerable<QueryTableColumn> columns, Dictionary<string, int> columnSizes)
        {
            var isFirst = true;
            foreach (var column in columns)
            {
                if (isFirst)
                {
                    Console.Write(' ');
                }
                else
                {
                    PrintColumnSeparator();
                }
                isFirst = false;
                Console.Write(column.ColumnName.PadRight(columnSizes[column.ColumnName]));
            }
            Console.WriteLine();
        }

        private static void PrintColumnSeparator() => Console.Write(" | ");

        private static void PrintRowSeparator(IEnumerable<QueryTableColumn> columns, Dictionary<string, int> columnSizes)
        {
            var isFirst = true;
            foreach (var column in columns)
            {
                if (isFirst)
                {
                    Console.Write('-');
                }
                else
                {
                    Console.Write("-+-");
                }
                isFirst = false;
                Console.Write(new string('-', columnSizes[column.ColumnName]));
            }
            Console.WriteLine('-');
        }

        private static void PrintRows(IEnumerable<QueryTableColumn> columns, IEnumerable<QueryTableRow> rows, Dictionary<string, int> columnSizes)
        {
            foreach (var row in rows)
            {
                var isFirst = true;
                foreach (var column in columns)
                {
                    if (isFirst)
                    {
                        Console.Write(' ');
                    }
                    else
                    {
                        PrintColumnSeparator();
                    }
                    isFirst = false;
                    if (!row.Cells.TryGetValue(column.ColumnName, out var cellData))
                    {
                        cellData = string.Empty;
                    }
                    Console.Write(cellData.PadLeft(columnSizes[column.ColumnName]));
                }
                Console.WriteLine();
            }
        }
    }
}
