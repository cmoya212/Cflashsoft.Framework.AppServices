using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Represents an application error or warning.
    /// </summary>
    public class ErrorResult
    {
        private ErrorResultKind _errorKind = ErrorResultKind.Error;
        private int _errorCode = 0;
        private string _message = string.Empty;
        private object _data = null;

        /// <summary>
        /// The kind of error.
        /// </summary>
        public ErrorResultKind ErrorKind { get => _errorKind; }
        /// <summary>
        /// Application specific number that can be used to classify the error.
        /// </summary>
        public int ErrorCode { get => _errorCode; }
        /// <summary>
        /// The text of the error message.
        /// </summary>
        public string Message { get => _message; }
        /// <summary>
        /// Application specific additional data.
        /// </summary>
        public object Data { get => _data; }

        /// <summary>
        /// Initializes a new instance of the ErrorResult class
        /// </summary>
        public ErrorResult()
        {

        }

        /// <summary>
        /// Initializes a new instance of the ErrorResult class
        /// </summary>
        public ErrorResult(ErrorResultKind errorKind, string message)
        {
            _errorKind = errorKind;
            _message = message;
        }

        /// <summary>
        /// Initializes a new instance of the ErrorResult class
        /// </summary>
        public ErrorResult(ErrorResultKind errorKind, int errorCode, string message)
        {
            _errorKind = errorKind;
            _errorCode = errorCode;
            _message = message;
        }

        /// <summary>
        /// Initializes a new instance of the ErrorResult class
        /// </summary>
        public ErrorResult(ErrorResultKind errorKind, int errorCode, string message, object data)
        {
            _errorKind = errorKind;
            _errorCode = errorCode;
            _message = message;
            _data = data;
        }
    }
}
