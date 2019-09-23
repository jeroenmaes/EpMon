using System;
using System.Collections.Generic;
using System.Text;

namespace EpMon.Data
{
    public interface ITrackableEntity
    {
        DateTime CreatedDateTime { get; set; }
        DateTime ModifiedDateTime { get; set; }
    }
}
