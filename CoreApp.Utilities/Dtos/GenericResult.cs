namespace CoreApp.Utilities.Dtos
{
    public class GenericResult
    {
        public GenericResult()
        {
        }

        public GenericResult(bool status)
        {
            Status = status;
        }

        public GenericResult(bool status, object data)
        {
            Status = status;
            Data = data;
        }

        public GenericResult(bool status, string message)
        {
            Status = status;
            Message = message;
        }

        public object Data { get; set; }

        public bool Status { get; set; }

        public string Message { get; set; }
    }
}
