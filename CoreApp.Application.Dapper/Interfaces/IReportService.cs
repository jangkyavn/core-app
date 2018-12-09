using CoreApp.Application.Dapper.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Dapper.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<RevenueReportViewModel>> GetReportsAsync(string fromDate, string toDate);
    }
}
