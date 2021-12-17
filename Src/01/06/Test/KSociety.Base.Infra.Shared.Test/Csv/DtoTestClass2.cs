using System;

namespace KSociety.Base.Infra.Shared.Test.Csv;

public class DtoTestClass2
{
    #region [Propery]
    public Guid Id { get; set; }

    public int ClassTypeId { get; set; }

    public string Name { get; set; }

    public string Ip { get; set; }

    public bool Enable { get; set; }

    #endregion

    public DtoTestClass2(Guid id, int classTypeId, string name, string ip, bool enable)
    {
        Id = id;
        ClassTypeId = classTypeId;
        Name = name;
        Ip = ip;
        Enable = enable;
    }

    public DtoTestClass2()
    {

    }
}