using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace PG.ABBs.Calendar.Organizer.Service.Helper
{
    public static class ApplicationInsightsHelper
    {

        public static Stopwatch stopwatch1 { get; set; }
        public static void SendEventTracking(
            TelemetryClient telemetryClient,
           // Stopwatch stopwatch,
            string apiName,
            string className,
            string function,
            string blockName
            )
        {

            stopwatch1.Stop();
            var metrics = new Dictionary<string, double> { { "processingTime", stopwatch1.Elapsed.TotalMilliseconds } };
            var properties = new Dictionary<string, string>
            {
                { "blockName", blockName },
                { "className", className },
                { "function", function },
            };
            telemetryClient.TrackEvent(apiName, properties, metrics);
            stopwatch1.Restart();
        }

        public static void SendExceptionTracking(
            TelemetryClient telemetryClient,
            Stopwatch stopwatch,
            System.Exception exception,
            string apiName,
            string className,
            string function,
            string blockName
        )
        {
            stopwatch.Stop();
            var metrics = new Dictionary<string, double> { { "processingTime", stopwatch.Elapsed.TotalMilliseconds } };
            var properties = new Dictionary<string, string>
            {
                { "apiName", apiName },
                { "blockName", blockName },
                { "className", className },
                { "function", function },
            };
            telemetryClient.TrackException(exception, properties, metrics);
            stopwatch.Restart();
        }

        public static void SendCustomErrorLog(
            TelemetryClient telemetryClient,
            string message,
            string source,
            string trace,
            string apiName
        )
        {

            telemetryClient.TrackTrace(apiName, SeverityLevel.Warning,
                new Dictionary<string, string> { { "Message", message}
                , { "Source", source }
                , { "StackTrace", trace }
                });

        }

        public static void SendCustomLog(
	        TelemetryClient telemetryClient,
	        string message,
	        string source,
	        string trace,
	        string apiName
        )
        {

	        telemetryClient.TrackTrace(apiName, SeverityLevel.Warning,
		        new Dictionary<string, string> { { "Message", message}
			        , { "Source", source }
			        , { "StackTrace", trace }
		        });

        }
    }
}
