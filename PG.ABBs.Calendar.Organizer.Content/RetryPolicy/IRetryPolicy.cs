using Polly.Retry;

namespace PG.ABBs.Calendar.Organizer.Content.RetryPolicy
{
    public interface IRetryPolicy
    {
        public AsyncRetryPolicy RetryPolicyAsync(string errorMessage);
    }
}
