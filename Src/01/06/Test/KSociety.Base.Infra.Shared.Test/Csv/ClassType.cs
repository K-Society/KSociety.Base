using System.Collections.Generic;

namespace KSociety.Base.Infra.Shared.Test.Csv
{
    public class ClassType
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Mean { get; private set; }

        public virtual ICollection<DtoTestClass3> DtoTestClass3s { get; set; }

        public ClassType(int id, string name, string mean)
        {
            Id = id;
            Name = name;
            Mean = mean;
        }
    }
}
