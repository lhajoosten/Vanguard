﻿namespace Vanguard.Core.Results
{
    /// <summary>
    /// Represents the result of an operation with success/failure status
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);

        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(value, true, string.Empty);

        public static Result<T> Failure<T>(string error) => new(default, false, error);
    }

    /// <summary>
    /// Represents the result of an operation with a return value and success/failure status
    /// </summary>
    public class Result<T> : Result
    {
        public T Value { get; }

        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            Value = value;
        }
    }
}