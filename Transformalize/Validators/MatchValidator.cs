﻿#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {

    /// <summary>
    /// returns true if matches any of the input
    /// </summary>
    public class MatchValidator : StringValidate {
        private readonly Regex _regex;
        private readonly Field _input;
        private readonly BetterFormat _betterFormat;

        public MatchValidator(IContext context) : base(context) {

            if (!Run)
                return;

            _input = SingleInput();
#if NETS10
            _regex = new Regex(context.Operation.Pattern);
#else
            _regex = new Regex(context.Operation.Pattern, RegexOptions.Compiled);
#endif

            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"{context.Field.Label} must match the regular expression pattern: {Context.Operation.Pattern.Replace("{", "{{").Replace("}", "}}")}.";
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            if (IsInvalid(row, _regex.Match(GetString(row, _input)).Success)) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            return row;
        }
    }
}