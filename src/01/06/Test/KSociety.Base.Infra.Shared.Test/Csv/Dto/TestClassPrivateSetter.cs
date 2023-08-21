namespace KSociety.Base.Infra.Shared.Test.Csv.Dto;
using System;

public class TestClassPrivateSetter
{

    #region [Propery]
    public Guid Id { get; private set; }

    public int ClassTypeId { get; private set; }

    public string Name { get; private set; }

    public string Ip { get; private set; }

    public bool Enable { get; private set; }

    public string Ahh { get; private set; }

    #endregion

    public TestClassPrivateSetter(Guid id, int classTypeId, string name, string ip, bool enable, string ahh)
    {
        this.Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = name;
        this.Ip = ip;
        this.Enable = enable;
        this.Ahh = ahh;
    }

    public TestClassPrivateSetter(Guid id, int classTypeId, string name, string ip, bool enable, string ahh, string test)
    {
        this.Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = name;
        this.Ip = ip;
        this.Enable = enable;
        this.Ahh = ahh;
        this.Ahh = test;
    }

    public TestClassPrivateSetter(Guid id, int classTypeId, string ip, bool enable)
    {
        this.Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = "MaDai";
        this.Ip = ip;
        this.Enable = enable;
        this.Ahh = "MaDai";
    }

    //Do not use this constructor.
    //public TestClassPrivateSetter()
    //{

    //}
}
