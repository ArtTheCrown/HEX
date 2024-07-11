using HEX.HEX.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEX.HEX.Helpers
{
    internal class SubClasses
    {
    }
    public class Request
    {
        public RequestType RequestType { get; set; }
    }
    public class Response
    {
        public ResponseStatus? Status { get; set; }
        public RequestType RequestType { get; set; }
    }
    public class System
    {
        public ResponseStatus? Status { get; set; }
        public User? User { get; set; }
    }

    public class Authentication
    {
        public ResponseStatus? Status { get; set; }
        public User? User { get; set; }
    }
    public class Registration
    {
        public ResponseStatus? Status { get; set; }
        public User? User { get; set; }
    }
    public class Communication
    {
        public ResponseStatus? Status { get; set; }
    }
    public class Files
    {
        public ResponseStatus? Status { get; set; }
        public FilesType FilesType { get; set; }
    }
    public class Kaizen
    {
        public ResponseStatus? Status { get; set; }
    }
    public class Report
    {
        public ResponseStatus? Status { get; set; }
    }
    public enum ResponseStatus
    {
        Success,
        Failure,
        UnauthorizedError,
        UnexpectedFailure,
        AuthenticationFailed,
        IncorrectCredentials,
        Taken,
        Available,
    }
    public enum FilesType
    {
        ProfilePicture
    }
}
