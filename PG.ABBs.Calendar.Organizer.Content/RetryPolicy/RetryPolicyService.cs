using System;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace PG.ABBs.Calendar.Organizer.Content.RetryPolicy
{
    public class RetryPolicyService :IRetryPolicy
    {
        private readonly ILogger logger;

        public RetryPolicyService(ILogger<RetryPolicyService> loggerProvider)
        {
            this.logger = loggerProvider;
        }
        public AsyncRetryPolicy RetryPolicyAsync(string errorMessage)
        {
            var polly = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(15),
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        context["retryAttempt"] = retryAttempt;
                        var message =
                            $"{errorMessage}, retry Count: {context["retryAttempt"]},  timespan {timespan}, exception {outcome.Message}, {outcome.StackTrace}";
                        logger.LogWarning(message);
                    });

            return polly;
        }
    }
}
