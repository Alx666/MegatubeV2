using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//__makeref
//__refvalue
//(T) Convert.ChangeType(value, typeof(T));
namespace MegatubeV2
{
    public class SmartParser<T> where T : new()
    {
        private static char[] separator;

        private StreamReader reader;
        private Dictionary<string, int> indices;
        private List<Mapping> mapping;
        private static Regex regex;
        private string currentLine;

        public bool EndOfSection
        {
            get
            {
                return reader.EndOfStream || string.IsNullOrEmpty(currentLine);
            }
        }

        static SmartParser()
        {
            separator   = new char[] { ',' };

            regex = new Regex("(?<=^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)(?:$|)");
            
        }

        public SmartParser(StreamReader sr)
        {
            reader  = sr;
            mapping = new List<Mapping>();
        }

        public SmartParser(StreamReader sr, string header) : this(sr)
        {
            this.SeekSection(header);
        }

        public void SeekSection(string header)
        {
            while (!reader.EndOfStream)
            {
                currentLine = reader.ReadLine();

                if (currentLine == header)
                {
                    indices = regex.Matches(currentLine).Cast<Match>().Select((s, i) => new { s.Value, i }).ToDictionary(k => k.Value, k => k.i);
                    currentLine = reader.ReadLine();
                    return;
                }
            }

            throw new EndOfStreamException("Could not locate the header before the end of the stream");
        }

        public T ReadLine()
        {            
            T item = new T();
            
            MatchCollection rawLine = regex.Matches(currentLine);
            int count = rawLine.Count;

            foreach (var map in mapping)
            {
                string textualValue = rawLine[indices[map.Field]].Value;

                switch (map.Code)
                {
                    case TypeCode.Boolean:  ((Action<T, bool>)map.Action).Invoke(item,      bool.Parse(textualValue));      break;
                    case TypeCode.Char:     ((Action<T, char>)map.Action).Invoke(item,      char.Parse(textualValue));      break;
                    case TypeCode.SByte:    ((Action<T, sbyte>)map.Action).Invoke(item,     sbyte.Parse(textualValue));     break;
                    case TypeCode.Byte:     ((Action<T, byte>)map.Action).Invoke(item,      byte.Parse(textualValue));      break;
                    case TypeCode.Int16:    ((Action<T, short>)map.Action).Invoke(item,     short.Parse(textualValue));     break;
                    case TypeCode.UInt16:   ((Action<T, ushort>)map.Action).Invoke(item,    ushort.Parse(textualValue));    break;
                    case TypeCode.Int32:    ((Action<T, int>)map.Action).Invoke(item,       int.Parse(textualValue));       break;
                    case TypeCode.UInt32:   ((Action<T, uint>)map.Action).Invoke(item,      uint.Parse(textualValue));      break;
                    case TypeCode.Int64:    ((Action<T, long>)map.Action).Invoke(item,      long.Parse(textualValue));      break;
                    case TypeCode.UInt64:   ((Action<T, ulong>)map.Action).Invoke(item,     ulong.Parse(textualValue));     break;
                    case TypeCode.Single:   ((Action<T, float>)map.Action).Invoke(item,     float.Parse(textualValue));     break;
                    case TypeCode.Double:   ((Action<T, double>)map.Action).Invoke(item,    double.Parse(textualValue));    break;
                    case TypeCode.Decimal:  ((Action<T, decimal>)map.Action).Invoke(item,   decimal.Parse(textualValue));   break;
                    case TypeCode.DateTime: ((Action<T, DateTime>)map.Action).Invoke(item,  DateTime.Parse(textualValue));  break;
                    case TypeCode.String:   ((Action<T, string>)map.Action).Invoke(item, textualValue);                     break;

                    default: throw new NotSupportedException();
               }                                                                
            }

            currentLine = reader.ReadLine();
            return item;
        }

        public void Map<K>(string field, Action<T, K> setter)
        {
            mapping.Add(new Mapping(field, Type.GetTypeCode(typeof(K)), setter));
        }

        
        private struct Mapping
        {
            public Mapping(string field, TypeCode code, object action)
            {
                Field   = field;
                Code    = code;
                Action  = action;
            }

            public string   Field;
            public TypeCode Code;
            public object   Action;
        }
    }
}
