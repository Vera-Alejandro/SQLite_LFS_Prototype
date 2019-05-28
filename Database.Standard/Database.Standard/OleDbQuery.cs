using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace Interstates.Control.Database
{
    [ComVisible(false)]
    public sealed class OleDbQuery : QueryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbQuery"/> class with a database profile <see cref="DatabaseProfile"/>.
        /// </summary>
        /// <param name="profile">The database profile that contains the connection to the database.</param>
        public OleDbQuery(DatabaseProfile profile)
            : base(profile)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbQuery"/> class with a connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public OleDbQuery(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbQuery"/> class with a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public OleDbQuery(DbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// <para>Gets the parameter token used to delimit parameters for the database.</para>
        /// </summary>
        /// <value>
        /// <para>The '@' symbol.</para>
        /// </value>
        private char ParameterToken
        {
            get { return '@'; }
        }

        protected override void DeriveParameters(System.Data.Common.DbCommand discoveryCommand)
        {
            if (discoveryCommand.CommandType == System.Data.CommandType.StoredProcedure)
            {
                OleDbCommandBuilder.DeriveParameters((OleDbCommand)discoveryCommand);
            }
            else
            {
                // Add the return parameter normally expected by stored procs
                discoveryCommand.Parameters.Add(CreateParameter(discoveryCommand, "@RETURN_VALUE"));
                discoveryCommand.Parameters[0].Direction = ParameterDirection.ReturnValue;

                // Find the parameters in the string based on the parameter token
                string strSearch = @"\" + ParameterToken + "[a-zA-Z0-9]+";
                MatchCollection matchParameters = Regex.Matches(discoveryCommand.CommandText, strSearch);
                foreach (Match paramMatch in matchParameters)
                {
                    discoveryCommand.Parameters.Add(CreateParameter(discoveryCommand, paramMatch.Value));
                }
            }
        }

        /// <summary>
        /// Returns the starting index for parameters in a command.
        /// </summary>
        /// <returns>The starting index for parameters in a command.</returns>
        protected override int UserParametersStartIndex()
        {
            return 1;
        }
    }
}
