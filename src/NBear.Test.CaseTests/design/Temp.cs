using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common.Design;

namespace Temp
{
    public interface TempContractBase
    {
        string BaseID { get; set; }
    }

    public interface TempContract1 : TempContractBase
    {
        [PrimaryKey]
        Guid ID { get; set; }
        string C1 { get; set; }
        string BaseID { get; set; }
    }

    public interface TempPerson : Entity, TempContract1
    {
        string Name { get; set; }
    }

    public interface TempUser : TempPerson
    {
        string Email { get; set; }
    }

    public interface TempLocalUser : TempUser
    {
        string LoginID { get; set; }
    }
}