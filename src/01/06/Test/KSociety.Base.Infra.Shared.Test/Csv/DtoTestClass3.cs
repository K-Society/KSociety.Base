// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Test.Csv;
using KSociety.Base.Domain.Shared.Class;

public class DtoTestClass3 : BaseEntity
{
    #region [Propery]
    //public Guid Id { get; private set; }

    public int ClassTypeId { get; private set; }
    public virtual ClassType ClassType { get; protected set; }

    public string Name { get; private set; }

    public string Ip { get; private set; }

    public bool Enable { get; private set; }

    public bool Extra { get; private set; }

    #endregion

    public DtoTestClass3(/*Guid id,*/ int classTypeId, string name, string ip, bool enable)
    {
        //Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = name;
        this.Ip = ip;
        this.Enable = enable;
        this.Name = "Urka";
    }

    public DtoTestClass3(string name, string ip, bool enable)
    {
        this.ClassTypeId = 0;
        this.Name = name;
        this.Ip = ip;
        this.Enable = enable;
        this.Name = "Urka2";
    }

    protected DtoTestClass3()
    {

    }
}
