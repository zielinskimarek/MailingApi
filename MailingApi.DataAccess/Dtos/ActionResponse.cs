namespace MailingApi.DataAccess.Dtos
{
    public class ActionResponse<T>
    {
        public ActionResponse(T response, string message)
        {
            Response = response;
            Message = message;
        }

        public T Response { get; }

        public string Message { get; }
    }
}
