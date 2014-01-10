using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class TimeOfDayOperation : AbstractOperation {

        private readonly string _inKey;
        private readonly string _outKey;
        private readonly string _outType;

        private readonly Dictionary<string, Func<DateTime, double>> _timeMap = new Dictionary<string, Func<DateTime, double>> {
            {"d", (x => x.TimeOfDay.TotalDays)},
            {"day", (x => x.TimeOfDay.TotalDays)},
            {"days", (x => x.TimeOfDay.TotalDays)},
            {"h", (x => x.TimeOfDay.TotalHours)},
            {"hour", (x => x.TimeOfDay.TotalHours)},
            {"hours", (x => x.TimeOfDay.TotalHours)},
            {"m", (x => x.TimeOfDay.TotalMinutes)},
            {"minute", (x => x.TimeOfDay.TotalMinutes)},
            {"minutes", (x => x.TimeOfDay.TotalMinutes)},
            {"s", (x => x.TimeOfDay.TotalSeconds)},
            {"second", (x => x.TimeOfDay.TotalSeconds)},
            {"seconds", (x => x.TimeOfDay.TotalSeconds)},
            {"ms", (x => x.TimeOfDay.TotalMilliseconds)},
            {"milliseconds", (x => x.TimeOfDay.TotalMilliseconds)},
            {"millisecond", (x => x.TimeOfDay.TotalMilliseconds)}
        };

        private readonly Func<DateTime, double> _transformer;

        public TimeOfDayOperation(string inKey, string inType, string outKey, string outType, string timeComponent) {
            _inKey = inKey;
            _outKey = outKey;
            _outType = outType;

            if (inType != "datetime") {
                Error("TimeOfDay operation can only accept DateTime input. Your input is for {0} is {1}.", outKey, inType);
                Environment.Exit(1);
            }

            if (!(new[]{"double","decimal","float"}).Any(s=>s.Equals(outType, StringComparison.OrdinalIgnoreCase))) {
                Error("TimeOfDay operation output must be double, decimal, or float.  Your output for {0} is {1}.", outKey, outType);
                Environment.Exit(1);
            }

            if (!_timeMap.ContainsKey(timeComponent.ToLower())) {
                Error("TimeOfDay operation expects time component to be days, hours, minutes, seconds, or milliseconds.  You have {0}.", timeComponent);
                Environment.Exit(1);
            }

            _transformer = _timeMap[timeComponent.ToLower()];

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                var date = (DateTime)row[_inKey];
                row[_outKey] = Converter(_transformer(date));
                yield return row;
            }
        }

        private object Converter(Double d) {
            return _outType != "double" ? d : Common.ObjectConversionMap[_outType](d);
        }
    }
}