// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Domain.Shared.Class
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Event;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc/>
    public class Entity : BaseEntity
    {
        /// <summary>
        /// Modify the value of the domain entity field.
        /// </summary>
        /// <param name="fieldName">Domain entity field name.</param>
        /// <param name="value">New value of the domain entity field.</param>
        public void ModifyField(string fieldName, string value)
        {
            this.Logger?.LogTrace("ModifyField Entity: {0}.{1} - {2} - {3}", this.GetType().FullName,
                MethodBase.GetCurrentMethod()?.Name,
                fieldName, value);

            this.AddEntityModifyFieldEvent(fieldName, value);
            try
            {
                var field = this.GetType().GetProperty(fieldName); //, BindingFlags.Public | BindingFlags.Instance);
                if (field != null && field.CanWrite)
                {
                    //Console.WriteLine("1 " + field.Name);
                    var t = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
                    object? safeValue = null;

                    //If is byte[]
                    if (t == typeof(byte[]))
                    {
                        if (!String.IsNullOrEmpty(value))
                        {
                            var splitResult = value.Split('-');

                            if (splitResult.Length > 0)
                            {
                                var byteArray = new byte[splitResult.Length];

                                for (var i = 0; i < splitResult.Length; i++)
                                {
                                    byteArray[i] = Convert.ToByte(splitResult[i], 16);
                                }

                                safeValue = byteArray;
                            }
                            else
                            {
                                this.Logger?.LogWarning("ModifyField Entity: {0}.{1} - {2} - {3}: Byte Array on data!", this.GetType().FullName,
                                    MethodBase.GetCurrentMethod()?.Name,
                                    fieldName, value);
                            }
                        }
                    }
                    //if (t == typeof(Guid))
                    //{
                    //    ////T t = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(text);
                    //    //safeValue = (value == null)
                    //    //    ? null
                    //    //    : TypeDescriptor.GetConverter(t).ConvertFromInvariantString(value);
                    //}
                    else
                    {
                        //Console.WriteLine("2");
                        //safeValue = (value == null) ? null : Convert.ChangeType(value, t);

                        safeValue = (String.IsNullOrEmpty(value))
                            ? null
                            : TypeDescriptor.GetConverter(t).ConvertFromInvariantString(value);
                    }

                    field.SetValue(this, safeValue, null);
                    //Console.WriteLine("3");
                }
                else
                {
                    this.Logger?.LogWarning("ModifyField Entity: {0}.{1} - {2} - {3}: Can not write OR null!", this.GetType().FullName,
                        MethodBase.GetCurrentMethod()?.Name, fieldName, value);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "ModifyField");
            }
        }

        private void AddEntityModifyFieldEvent(string fieldName, string fieldValue)
        {
            var entityModifyField = new EntityModifyField(fieldName, fieldValue);

            this.AddDomainEvent(entityModifyField);
        }
    }
}
