using System;

namespace CoreApp.Data.Interfaces
{
    public interface IDateTracking
    {
        DateTime DateCreated { get; set; }

        DateTime? DateModified { get; set; }
    }
}
