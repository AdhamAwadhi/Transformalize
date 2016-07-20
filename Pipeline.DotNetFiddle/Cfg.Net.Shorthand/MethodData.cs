#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
namespace Cfg.Net.Shorthand
{
    public class MethodData
    {
        public MethodData(Method method, Signature signature, Target target)
        {
            Method = method;
            Signature = signature;
            Target = target;
        }

        public Method Method { get; set; }
        public Signature Signature { get; set; }
        public Target Target { get; set; }
    }
}