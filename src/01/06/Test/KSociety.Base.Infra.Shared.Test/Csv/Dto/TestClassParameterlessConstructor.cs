// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Test.Csv.Dto;
using System;

public class TestClassParameterlessConstructor
{
    #region [Propery]
    public Guid Id { get; set; }

    public int ClassTypeId { get; set; }

    public string Name { get; set; }

    public string Ip { get; set; }

    public bool Enable { get; set; }

    public string Ahh { get; set; }

    #endregion

    public TestClassParameterlessConstructor(Guid id, int classTypeId, string name, string ip, bool enable, string ahh)
    {
        this.Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = name;
        this.Ip = ip;
        this.Enable = enable;
        this.Ahh = ahh;
    }

    public TestClassParameterlessConstructor(Guid id, int classTypeId, string name, string ip, bool enable, string ahh, string test)
    {
        this.Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = name;
        this.Ip = ip;
        this.Enable = enable;
        this.Ahh = ahh;
        this.Ahh = test;
    }

    public TestClassParameterlessConstructor(Guid id, int classTypeId, string ip, bool enable)
    {
        this.Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = "MaDai";
        this.Ip = ip;
        this.Enable = enable;
        this.Ahh = "MaDai";
    }

    public TestClassParameterlessConstructor()
    {

    }
}
