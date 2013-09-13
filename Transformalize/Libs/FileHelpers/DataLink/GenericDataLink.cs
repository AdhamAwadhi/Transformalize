#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Reflection;
using Transformalize.Libs.FileHelpers.DataLink.Storage;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.DataLink
{
    /// <summary>
    ///     This class has the responsability to enable the two directional
    ///     transformation.
    ///     <list type="bullet">
    ///         <item> DataStorage &lt;-> DataStorage </item>
    ///     </list>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Uses two <see cref="DataStorage" /> to accomplish this task.
    ///     </para>
    /// </remarks>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="class_diagram.html">Class Diagram</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    /// <seealso href="example_datalink.html">Example of the DataLink</seealso>
    /// <seealso href="attributes.html">Attributes List</seealso>
    public sealed class GenericDataLink
    {
        #region "  Constructor  "

        /// <summary>Create a new instance of the class.</summary>
        /// <param name="provider1">
        ///     The First <see cref="DataStorage" /> used to insert/extract records .
        /// </param>
        /// <param name="provider2">
        ///     The Second <see cref="DataStorage" /> used to insert/extract records .
        /// </param>
        public GenericDataLink(DataStorage provider1, DataStorage provider2)
        {
            if (provider1 == null)
                throw new ArgumentException("provider1 can�t be null", "provider1");
            else
                mDataStorage1 = provider1;

            if (provider2 == null)
                throw new ArgumentException("provider2 can�t be null", "provider2");
            else
                mDataStorage2 = provider2;

            ValidateRecordTypes();
        }

        #endregion

        private readonly DataStorage mDataStorage1;
        private readonly DataStorage mDataStorage2;

        private MethodInfo mConvert1to2;
        private MethodInfo mConvert2to1;


        /// <summary>
        ///     The fisrt <see cref="DataStorage" /> of the <see cref="GenericDataLink" />.
        /// </summary>
        public DataStorage DataStorage1
        {
            get { return mDataStorage1; }
        }

        /// <summary>
        ///     The second <see cref="DataStorage" /> of the <see cref="GenericDataLink" />.
        /// </summary>
        public DataStorage DataStorage2
        {
            get { return mDataStorage2; }
        }

        /// <summary>Extract the records from DataStorage1 and Insert them to the DataStorage2.</summary>
        /// <returns>The Copied records.</returns>
        public object[] CopyDataFrom1To2()
        {
            var res = DataStorage1.ExtractRecords();
            DataStorage2.InsertRecords(res);
            return res;
        }

        /// <summary>Extract the records from DataStorage2 and Insert them to the DataStorage1.</summary>
        /// <returns>The Copied records.</returns>
        public object[] CopyDataFrom2To1()
        {
            var res = DataStorage2.ExtractRecords();
            DataStorage1.InsertRecords(res);
            return res;
        }

        private void ValidateRecordTypes()
        {
            if (DataStorage1.RecordType == null)
                throw new BadUsageException("DataLink1 can�t have a null RecordType.");

            if (DataStorage2.RecordType == null)
                throw new BadUsageException("DataLink2 can�t have a null RecordType.");

            if (DataStorage1.RecordType != DataStorage2.RecordType)
            {
                mConvert1to2 = GetTransformMethod(DataStorage1.RecordType, DataStorage2.RecordType);
                if (mConvert1to2 == null)
                    throw new BadUsageException("You must to define a method in the class " + DataStorage1.RecordType.Name + " with the attribute [TransfortToRecord(typeof(" + DataStorage2.RecordType.Name + "))]");

                mConvert2to1 = GetTransformMethod(DataStorage2.RecordType, DataStorage1.RecordType);
                if (mConvert2to1 == null)
                    throw new BadUsageException("You must to define a method in the class " + DataStorage2.RecordType.Name + " with the attribute [TransfortToRecord(typeof(" + DataStorage1.RecordType.Name + "))]");
            }
        }

        private MethodInfo GetTransformMethod(Type sourceType, Type destType)
        {
            var methods = sourceType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
//			foreach (MethodInfo m in methods)
//			{
//				if (m.IsDefined(typeof (TransformToRecordAttribute), false))
//				{
//					TransformToRecordAttribute ta = (TransformToRecordAttribute) m.GetCustomAttributes(typeof (TransformToRecordAttribute), false)[0];
//					if (ta.TargetType == destType)
//					{
//						return m;
//					}
//				}
//			}

            return null;
        }
    }
}