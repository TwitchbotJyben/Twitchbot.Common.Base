namespace Twitchbot.Common.Base.Models
{
    /// <summary>
    /// Model of an Http Request.
    /// </summary>
    /// <typeparam name="T">The type of the business model.</typeparam>
    public class HttpResultModel<T> where T : class
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool Result { get; set; } = true;
        /// <summary>
        /// Filled with the exception message if <see cref="Result"/> is false.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Filled with a business message in connection with the requested operation.
        /// </summary>
        public string BusinessMessage { get; set; }
        /// <summary>
        /// Gets or sets the object of <see cref="T"/>.
        /// </summary>
        public T Model { get; set; }

        /// <summary>
        /// Fill all the properties of <see cref="HttpResultModel{T}"/>.
        /// </summary>
        /// <param name="result">The <see cref="Result"/>.</param>
        /// <param name="errorMessage">The <see cref="ErrorMessage"/></param>
        /// <param name="businessMessage">The <see cref="BusinessMessage"/>.</param>
        /// <param name="model">The <see cref="Model"/>.</param>
        public void PerformResult(bool result, string errorMessage, string businessMessage, T model)
        {
            Result = result;
            ErrorMessage = errorMessage;
            BusinessMessage = businessMessage;
            Model = model;
        }
    }
}