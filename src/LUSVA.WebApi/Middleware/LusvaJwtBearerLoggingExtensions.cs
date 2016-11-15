using System;
using Microsoft.Extensions.Logging;

namespace LUSVA.WebApi.Middleware
{
  public static class LusvaJwtBearerLoggingExtensions
  {
    private static Action<ILogger, string, Exception> _tokenValidationFailed =
      LoggerMessage.Define<string>(LogLevel.Information, (EventId) 1, "Failed to validate the token {Token}.");

    private static Action<ILogger, Exception> _tokenValidationSucceeded = LoggerMessage.Define(LogLevel.Information,
      (EventId) 2, "Successfully validated the token.");

    private static Action<ILogger, Exception> _errorProcessingMessage = LoggerMessage.Define(LogLevel.Error, (EventId) 3,
      "Exception occurred while processing message.");

    public static void TokenValidationFailed(this ILogger logger, string token, Exception ex)
    {
      _tokenValidationFailed(logger, token, ex);
    }

    public static void TokenValidationSucceeded(this ILogger logger)
    {
      _tokenValidationSucceeded(logger, (Exception) null);
    }

    public static void ErrorProcessingMessage(this ILogger logger, Exception ex)
    {
      _errorProcessingMessage(logger, ex);
    }
  }
}