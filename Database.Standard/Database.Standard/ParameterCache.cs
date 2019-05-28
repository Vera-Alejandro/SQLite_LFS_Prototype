using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace Interstates.Control.Database
{
    /// <summary>
    /// <para>
    /// Provides parameter caching services for dynamic parameter discovery of stored procedures.
    /// Eliminates the round-trip to the aQuery to derive the parameters and types when a command
    /// is executed more than once.
    /// </para>
    /// </summary>
    [ComVisible(false)]
    public class ParameterCache
    {
        private CachingMechanism _cache = new CachingMechanism();

        /// <summary>
        /// <para>
        /// Populates the parameter collection for a command wrapper from the cache 
        /// or performs a round-trip to the aQuery to query the parameters.
        /// </para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to add the parameters.</para>
        /// </param>
        /// <param name="query">
        /// <para>The aQuery to use to set the parameters.</para>
        /// </param>
        public void SetParameters(DbCommand command, QueryBase query)
        {
            if (command == null) throw new ArgumentNullException("Command");
            if (query == null) throw new ArgumentNullException("Query");


            if (AlreadyCached(command, query))
            {
                AddParametersFromCache(command, query);
            }
            else
            {
                query.DiscoverParameters(command);
                IDataParameter[] copyOfParameters = CreateParameterCopy(command);

                this._cache.AddParameterSetToCache(query.ConnectionString, command, copyOfParameters);
            }
        }

        /// <summary>
        /// <para>Empties the parameter cache.</para>
        /// </summary>
        protected internal void Clear()
        {
            this._cache.Clear();
        }

        /// <summary>
        /// <para>Adds parameters to a command using the cache.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to add the parameters.</para>
        /// </param>
        /// <param name="query">The aQuery to use.</param>
        protected virtual void AddParametersFromCache(DbCommand command, QueryBase query)
        {
            IDataParameter[] parameters = this._cache.GetCachedParameterSet(query.ConnectionString, command);

            foreach (IDataParameter p in parameters)
            {
                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// <para>Checks to see if a cache entry exists for a specific command on a specific connection</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to check.</para>
        /// </param>
        /// <param name="query">The aQuery to check.</param>
        /// <returns>True if the parameters are already cached for the provided command, false otherwise</returns>
        private bool AlreadyCached(IDbCommand command, QueryBase query)
        {
            return this._cache.IsParameterSetCached(query.ConnectionString, command);
        }

        private static IDataParameter[] CreateParameterCopy(DbCommand command)
        {
            IDataParameterCollection parameters = command.Parameters;
            IDataParameter[] parameterArray = new IDataParameter[parameters.Count];
            parameters.CopyTo(parameterArray, 0);

            return CachingMechanism.CloneParameters(parameterArray);
        }
    }
}
