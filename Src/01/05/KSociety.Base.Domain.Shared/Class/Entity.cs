using System;
using System.ComponentModel;
using System.Reflection;
using KSociety.Base.Domain.Shared.Event;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Domain.Shared.Class;

/// <inheritdoc/>
public class Entity : BaseEntity
{
    #region [Constructor]


    #endregion


    /// <summary>
    /// Modify the value of the domain entity field.
    /// </summary>
    /// <param name="fieldName">Domain entiy field name.</param>
    /// <param name="value">New value of the domaint entiy field.</param>
    public void ModifyField(string fieldName, string value)
    {
        Logger?.LogTrace("ModifyField Entity: {0}.{1} - {2} - {3}",
            GetType().FullName, 
            MethodBase.GetCurrentMethod()?.Name, 
            fieldName, value);

        AddEntityModifyFieldEvent(fieldName, value);
        try
        {
            var field = GetType().GetProperty(fieldName); //, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.CanWrite)
            {
                //Console.WriteLine("1 " + field.Name);
                var t = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
                object safeValue = null;

                //If is byte[]
                if (t == typeof(byte[]))
                {
                    if (value != null)
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
                            Logger?.LogWarning("ModifyField Entity: {0}.{1} - {2} - {3}: Byte Array on data!",
                                GetType().FullName,
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

                    safeValue = (value == null)
                        ? null
                        : TypeDescriptor.GetConverter(t).ConvertFromInvariantString(value);
                }

                field.SetValue(this, safeValue, null);
                //Console.WriteLine("3");
            }
            else
            {
                Logger?.LogWarning("ModifyField Entity: {0}.{1} - {2} - {3}: Can not write OR null!",
                    GetType().FullName, 
                    MethodBase.GetCurrentMethod()?.Name, fieldName, value);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "ModifyField");
        }
    }

    private void AddEntityModifyFieldEvent(string fieldName, string fieldValue)
    {
        var entityModifyField = new EntityModifyField(fieldName, fieldValue, DateTime.Now);

        AddDomainEvent(entityModifyField);
    }
}