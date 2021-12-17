using System;

namespace KSociety.Base.Infra.Shared.Test.Csv;

public class DtoTestClass
{
    //#region [Propery]
    //public Guid Id { get; private set; }

    //public int ClassTypeId { get; private set; }

    //public string Name { get; private set; }

    //public string Ip { get; private set; }

    //public bool Enable { get; private set; }

    //public string Ahh { get; private set; }

    //#endregion

    #region [Propery]
    public Guid Id { get; set; }

    public int ClassTypeId { get; set; }

    public string Name { get; set; }

    public string Ip { get; set; }

    public bool Enable { get; set; }

    public string Ahh { get; set; }

    #endregion

    public DtoTestClass(Guid id, int classTypeId, string name, string ip, bool enable, string ahh)
    {
        Id = id;
        ClassTypeId = classTypeId;
        Name = name;
        Ip = ip;
        Enable = enable;
        Ahh = ahh;
    }

    //public DtoTestClass()
    //{

    //}
}