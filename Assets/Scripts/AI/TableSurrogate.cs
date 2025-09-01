using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AI
{
    public class TableSurrogate<TRow, TCol, TValue> : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Table<TRow, TCol, TValue> table = (Table<TRow, TCol, TValue>)obj;
            if(obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj), "Object cannot be null");
            }
            info.AddValue("TableValues", table.TableValues, typeof(Dictionary<(TRow row, TCol col), TValue>));
        }
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Table<TRow, TCol, TValue> table = (Table<TRow, TCol, TValue>)obj;
            if (obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj), "Object cannot be null");
            }
            table.SetTable((Dictionary<(TRow row, TCol col), TValue>)info.GetValue("TableValues", typeof(Dictionary<(TRow row, TCol col), TValue>)));
            return table;
        }
    }
}