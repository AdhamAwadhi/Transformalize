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
using Transformalize.Libs.FileHelpers.DataLink.Storage;

namespace Transformalize.Libs.FileHelpers.DataLink
{
    /// <summary>
    ///     This class has the responsability to enable the two directional
    ///     transformation.
    ///     <list type="bullet">
    ///         <item> Excel &lt;-> DataStorage</item>
    ///     </list>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Uses an <see cref="DataStorage" /> to accomplish this task.
    ///     </para>
    /// </remarks>
    /// <seealso href="quick_start.html">Quick Start Guide</seealso>
    /// <seealso href="class_diagram.html">Class Diagram</seealso>
    /// <seealso href="examples.html">Examples of Use</seealso>
    /// <seealso href="example_datalink.html">Example of the DataLink</seealso>
    /// <seealso href="attributes.html">Attributes List</seealso>
    public sealed class ExcelDataLinkOleDb
    {
        #region "  Constructor  "

        /// <summary>Create a new instance of the class.</summary>
        /// <param name="provider">
        ///     The <see cref="FileHelpers.Storage.DataStorageperforms the transformation.
        /// </param>
        public ExcelDataLinkOleDb(DataStorage provider)
        {
            mProvider = provider;
            if (mProvider != null)
                mExcelStorage = new ExcelStorageOleDb(provider.RecordType);
            else
                throw new ArgumentException("provider can�t be null", "provider");
        }

        #endregion

        #region "  ExcelStorage  "

        private readonly ExcelStorageOleDb mExcelStorage;

        /// <summary>
        ///     The internal <see cref="T:Transformalize.Libs.FileHelpers.Engines.FileHelperEngine" /> used to the file or stream ops.
        /// </summary>
        public ExcelStorageOleDb ExcelStorage
        {
            get { return mExcelStorage; }
        }

        #endregion

        #region "  DataLinkProvider  "

        private readonly DataStorage mProvider;

        /// <summary>
        ///     The internal <see cref="T:Transformalize.Libs.FileHelpers.DataLink.Storage.DataStorage" /> used to the link ops.
        /// </summary>
        public DataStorage DataStorage
        {
            get { return mProvider; }
        }

        #endregion

        #region "  Last Records "

        private object[] mLastExtractedRecords;

        private object[] mLastInsertedRecords;

        /// <summary>
        ///     An array of the last records extracted from the data source to a file.
        /// </summary>
        public object[] LastExtractedRecords
        {
            get { return mLastExtractedRecords; }
        }

        /// <summary>
        ///     An array of the last records inserted in the data source that comes from a file.
        /// </summary>
        public object[] LastInsertedRecords
        {
            get { return mLastInsertedRecords; }
        }

        #endregion

        #region "  ExtractTo File/Stream   "

        /// <summary>
        ///     Extract records from the data source and insert them to the specified file using the DataLinkProvider
        ///     <see
        ///         cref="FileHelpers.Storage.DataStoragerds" />
        ///     method.
        /// </summary>
        /// <param name="fileName">The files where the records be written.</param>
        /// <returns>True if the operation is successful. False otherwise.</returns>
        public bool ExtractToExcel(string fileName)
        {
            mLastExtractedRecords = mProvider.ExtractRecords();
            mExcelStorage.InsertRecords(mLastExtractedRecords);
            return true;
        }

        #endregion

        #region "  InsertFromFile  "

        /// <summary>
        ///     Extract records from a file and insert them to the data source using the DataLinkProvider
        ///     <see
        ///         cref="FileHelpers.Storage.DataStorageds" />
        ///     method.
        /// </summary>
        /// <param name="excelFileName">The file with the source records.</param>
        /// <returns>True if the operation is successful. False otherwise.</returns>
        public bool InsertFromExcel(string excelFileName)
        {
            mLastInsertedRecords = mExcelStorage.ExtractRecords();
            mProvider.InsertRecords(mLastInsertedRecords);
            return true;
        }

        #endregion
    }
}