namespace Vanguard.Core.Results
{
    /// <summary>
    /// Extension methods for working with Result objects
    /// </summary>
    public static class ResultExtensions
    {
        public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
        {
            return result.IsSuccess
                ? Result.Success(mapper(result.Value))
                : Result.Failure<TOut>(result.Error);
        }

        public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> mapper)
        {
            var result = await resultTask;
            return result.Map(mapper);
        }

        public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<TOut>> mapper)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return Result.Failure<TOut>(result.Error);

            var mappedValue = await mapper(result.Value);
            return Result.Success(mappedValue);
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
        {
            if (result.IsFailure)
                return result;

            return predicate(result.Value)
                ? result
                : Result.Failure<T>(errorMessage);
        }

        public static Result OnSuccess(this Result result, Action action)
        {
            if (result.IsSuccess)
                action();

            return result;
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (result.IsSuccess)
                action(result.Value);

            return result;
        }

        public static Result OnFailure(this Result result, Action action)
        {
            if (result.IsFailure)
                action();

            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action action)
        {
            if (result.IsFailure)
                action();

            return result;
        }
    }
}