using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Helpers
{
    public static class LogHelper
    {
        public static void ShouldHaveLoggedInformation(
            this ILogger logger,
            string logMessage
        )
        {
            logger.ShouldHaveLogged(LogLevel.Information, logMessage);
        }

        public static void ShouldHaveLoggedWarning()
        {
            //
        }

        public static void ShouldHaveLoggedError(
            this ILogger logger,
            string logMessage
        )
        {
            logger.ShouldHaveLogged(LogLevel.Error, logMessage);
        }

        private static void ShouldHaveLogged(
            this ILogger logger,
            LogLevel logLevel,
            string logMessage
        )
        {
            logger.Received()
                .Log(
                    Arg.Is<LogLevel>(l => l == logLevel),
                    Arg.Any<EventId>(),
                    Arg.Is<object>(s => s.ToString() == logMessage),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<object, Exception?, string>>()
                );
        }
    }
}