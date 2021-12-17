using System;

namespace KSociety.Base.Infra.Shared.Test.Csv.Dto;

public class TestClass
{
    #region [Propery]
    public Guid Id { get; set; }

    public int ClassTypeId { get; set; }

    public string Name { get; set; }

    public string Ip { get; set; }

    public bool Enable { get; set; }

    public string Ahh { get; set; }

    #endregion

    public TestClass(Guid id, int classTypeId, string name, string ip, bool enable, string ahh)
    {
        Id = id;
        ClassTypeId = classTypeId;
        Name = name;
        Ip = ip;
        Enable = enable;
        Ahh = ahh;
    }

    public TestClass(Guid id, int classTypeId, string name, string ip, bool enable, string ahh, string test)
    {
        Id = id;
        ClassTypeId = classTypeId;
        Name = name;
        Ip = ip;
        Enable = enable;
        Ahh = ahh;
        Ahh = test;
    }

    public TestClass(Guid id, int classTypeId, string ip, bool enable)
    {
        Id = id;
        ClassTypeId = classTypeId;
        Name = "MaDai";
        Ip = ip;
        Enable = enable;
        Ahh = "MaDai";
    }

    //public TestClass()
    //{

    //}
}