// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class.Csv
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ReadCsvClassMap<TEntity, TClassMap>
        where TEntity : class
        where TClassMap : ClassMap<TEntity>
    {

        public static TEntity[]? Read(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ReadCsv");
            var csvFileName = @"." + fileName + @".csv";
            logger?.LogTrace("ReadCsv csvFileName: {0}", csvFileName);

            var assembly = Assembly.GetCallingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(csvFileName));
            logger?.LogTrace("ReadCsv resourceName: {0}", resourceName);

            try
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var streamReader = new StreamReader(stream ?? throw new InvalidOperationException()))
                    {
                        var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                        reader.Context.RegisterClassMap<TClassMap>();
                        return reader.GetRecords<TEntity>().ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error ReadCsv: ");
            }

            return null;
        }

        public static IEnumerable<TEntity>? Import(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportCsv");

            try
            {
                using (var streamReader = new StreamReader(fileName))
                {
                    var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                    reader.Context.RegisterClassMap<TClassMap>();

                    return reader.GetRecords<TEntity>().ToArray();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }

            return null;
        }

        public static IEnumerable<TEntity>? Import(ILoggerFactory loggerFactory, byte[] byteArray)
        {
            var logger = loggerFactory?.CreateLogger("ImportCsv");

            try
            {
                using (var streamReader = new StreamReader(new MemoryStream(byteArray)))
                {
                    var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                    reader.Context.RegisterClassMap<TClassMap>();
                    return reader.GetRecords<TEntity>().ToArray();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }

            return null;
        }

        public static IAsyncEnumerable<TEntity>? ImportAsync(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");
            IAsyncEnumerable<TEntity>? output = null;

            try
            {
                using (var streamReader = new StreamReader(fileName))
                {
                    var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                    reader.Context.RegisterClassMap<TClassMap>();
                    output = reader.GetRecordsAsync<TEntity>();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv.ImportAsync: ");
            }

            return output;
        }

        public static async IAsyncEnumerable<TEntity> ImportAsync(ILoggerFactory loggerFactory, byte[] byteArray,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            //var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");

            using (var streamReader = new StreamReader(new MemoryStream(byteArray)))
            {
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                reader.Context.RegisterClassMap<TClassMap>();
                var result = reader.GetRecordsAsync<TEntity>(cancellationToken);

                await foreach (var item in result.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }
    }
}
