namespace KSociety.Base.Infra.Shared.Test.Csv;
using System;

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
        this.Id = id;
        this.ClassTypeId = classTypeId;
        this.Name = name;
        this.Ip = ip;
        this.Enable = enable;
    }

    public DtoTestClass2()
    {

    }
}
