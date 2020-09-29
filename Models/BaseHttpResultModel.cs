namespace Twitchbot.Common.Base.Models
{
    public class HttpResultModel<T> where T : class
    {
        public bool Result { get; set; } = true;
        public string ErrorMessage { get; set; }
        public string BusinessMessage { get; set; }
        public T Model { get; set; }

        public void PerformResult(bool result, string errorMessage, string businessMessage, T model)
        {
            Result = result;
            ErrorMessage = errorMessage;
            BusinessMessage = businessMessage;
            Model = model;
        }
    }
}