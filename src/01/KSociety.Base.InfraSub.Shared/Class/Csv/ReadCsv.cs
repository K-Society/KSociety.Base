// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class.Csv
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using CsvHelper;
    using Microsoft.Extensions.Logging;

    public static class ReadCsv<TClass>
        where TClass : class
    {
        public static TClass[] Read(ILoggerFactory loggerFactory, string fileName)
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
                        return reader.GetRecords<TClass>().ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error ReadCsv: ");
            }

            return null;
        }

        public static IEnumerable<TClass> Import(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportCsv");

            try
            {
                using (var streamReader = new StreamReader(fileName))
                {
                    var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                    return reader.GetRecords<TClass>().ToArray();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }

            return null;
        }

        public static IEnumerable<TClass> Import(ILoggerFactory loggerFactory, byte[] byteArray)
        {
            var logger = loggerFactory.CreateLogger("ImportCsv");

            try
            {
                using (var streamReader = new StreamReader(new MemoryStream(byteArray)))
                {
                    var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                    return reader.GetRecords<TClass>().ToArray();
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }

            return null;
        }

        //#if NETSTANDARD2_1

        //public static async IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, string fileName,
        //    [EnumeratorCancellation] CancellationToken cancellationToken = default)
        public static IEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, string fileName, CancellationToken cancellationToken = default)
        {
            //var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");

            using (var streamReader = new StreamReader(fileName))
            {
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TClass>(); //.GetAsyncEnumerator(cancellationToken);


                //return result;
                //try
                //{
                //    while (await result.MoveNextAsync())
                //    {
                //        var item = result.Current;
                //        //yield return item;
                //        // Do things here
                //    }
                //}
                //finally
                //{
                //    await result.DisposeAsync();
                //}

                //return result;

                //foreach (var item in result)
                //{
                //    //return item;
                //    //yield return item;
                //    return (IAsyncEnumerable<TClass>) item;
                //}
            }

            //return null;
        }

        //public static async IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, byte[] byteArray,
        //    [EnumeratorCancellation] CancellationToken cancellationToken = default)
        public static IEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, byte[] byteArray, CancellationToken cancellationToken = default)
        {
            //var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");

            using (var streamReader = new StreamReader(new MemoryStream(byteArray)))
            {
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                return reader.GetRecords<TClass>();

                //foreach (var item in result)
                //{
                //    //yield return item;
                //    return (IAsyncEnumerable<TClass>)item;
                //}
            }

            //return null;
        }

        //#endif
    }
}
