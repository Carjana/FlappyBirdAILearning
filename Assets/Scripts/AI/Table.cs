using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AI
{
    public class Table<TRow, TCol, TValue>
    {
        public Dictionary<(TRow, TCol), TValue> TableValues { get; private set; } = new();
        public HashSet<TRow> Rows { get; private set; } = new();
        public HashSet<TCol> Cols { get; private set; } = new();

        public Table() { }
        
        public Table(TRow[] rows, TCol[] cols, TValue defaultValue)
        {
            AddRows(rows, defaultValue);
            AddColumns(cols, defaultValue);
        }

        private void AddRows(TRow[] rows, TValue defaultValue)
        {
            foreach (TRow row in rows)
            {
                AddRow(row, defaultValue);
            }
        }

        private void AddColumns(TCol[] cols, TValue defaultValue)
        {
            foreach (TCol col in cols)
            {
                AddColumn(col, defaultValue);
            }
        }

        public void AddRow(TRow row, TValue defaultValue)
        {
            if (!Rows.Add(row))
                return;
            foreach (TCol col in Cols)
            {
                TableValues.Add((row, col), defaultValue);
            }
        }
        
        public void AddColumn(TCol col, TValue defaultValue)
        {
            if (!Cols.Add(col))
                return;
            foreach (TRow row in Rows)
            {
                TableValues.Add((row, col), defaultValue);
            }
        }
        
        public bool TryGetValue(TRow row, TCol col, out TValue value) => TableValues.TryGetValue((row, col), out value);
        
        public void SetValue(TRow row, TCol col, TValue value)
        {
            if (!Rows.Contains(row) || !Cols.Contains(col))
                return;
            TableValues[(row, col)] = value;
        }
        
        public (TRow, TValue)[] GetRows(TCol col)
        {
            if (!Cols.Contains(col))
                return Array.Empty<(TRow, TValue)>();
            
            List<(TRow, TValue)> result = new();
            foreach (TRow row in Rows)
            {
                if (TableValues.TryGetValue((row, col), out TValue value))
                {
                    result.Add((row, value));
                }
            }
            return result.ToArray();
        }
        
        public (TCol, TValue)[] GetColumns(TRow row)
        {
            if (!Rows.Contains(row))
                return Array.Empty<(TCol, TValue)>();
            
            List<(TCol, TValue)> result = new();
            foreach (TCol col in Cols)
            {
                if (TableValues.TryGetValue((row, col), out TValue value))
                {
                    result.Add((col, value));
                }
            }
            return result.ToArray();
        }
    }
}