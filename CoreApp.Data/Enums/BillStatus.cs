using System.ComponentModel;

namespace CoreApp.Data.Enums
{
    public enum BillStatus
    {
        [Description("Mới")]
        New,
        [Description("Đang xử lý")]
        InProgress,
        [Description("Trả lại")]
        Returned,
        [Description("Đã hủy")]
        Cancelled,
        [Description("Đã hoàn thành")]
        Completed
    }
}
