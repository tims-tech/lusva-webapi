using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LUSVA.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace LUSVA.WebApi.Middleware
{
  /// <summary>
  /// Extends JwtBearerHandler by using implementation.
  /// All code except for the HandleAuthenticateAsync() comes from
  /// Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler
  /// This class adds an additional check to the authenticate process
  /// </summary>
  public class LusvaJwtBearerHandler : AuthenticationHandler<JwtBearerOptions>
  {
    private OpenIdConnectConfiguration _configuration;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      var result = await JwtHandleAuthenticateAsync();
      if (result == null || !result.Succeeded || !((LusvaJwtBearerOptions)Options).ValidateSecurityStamp)
        return result;

      // Check that security stamp is still valid
      var ticket = result.Ticket;
      var token = ((string) Request.Headers["Authorization"]).Split(' ').Last();
      var claim = await ((LusvaJwtBearerOptions) Options).TokenService.GetSecurityStampFromTokenAsync(token);
      var securitystamp = claim.Value;
      var user = await ((LusvaJwtBearerOptions) Options).UserManager.GetUserAsync(Context.User);
      if (user.SecurityStamp != securitystamp)
        return AuthenticateResult.Fail(new InvalidTokenException("Token Expired"));
      return AuthenticateResult.Success(ticket);
    }

    /// <summary>
    /// Searches the 'Authorization' header for a 'Bearer' token. If the 'Bearer' token is found, it is validated using <see cref="T:Microsoft.IdentityModel.Tokens.TokenValidationParameters" /> set in the options.
    /// </summary>
    /// <returns></returns>
    protected async Task<AuthenticateResult> JwtHandleAuthenticateAsync()
    {
      string token = (string) null;
      AuthenticateResult result = (AuthenticateResult) null;
      int num = 0;
      object obj;
      try
      {
        MessageReceivedContext messageReceivedContext = new MessageReceivedContext(this.Context, this.Options);
        await this.Options.Events.MessageReceived(messageReceivedContext);
        if (messageReceivedContext.CheckEventResult(out result))
          return result;
        token = messageReceivedContext.Token;
        if (string.IsNullOrEmpty(token))
        {
          string str = (string) this.Request.Headers["Authorization"];
          if (string.IsNullOrEmpty(str))
            return AuthenticateResult.Skip();
          if (str.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = str.Substring("Bearer ".Length).Trim();
          if (string.IsNullOrEmpty(token))
            return AuthenticateResult.Skip();
        }
        if (this._configuration == null && this.Options.ConfigurationManager != null)
        {
          LusvaJwtBearerHandler jwtBearerHandler = this;
          OpenIdConnectConfiguration connectConfiguration = jwtBearerHandler._configuration;
          OpenIdConnectConfiguration configurationAsync =
            await this.Options.ConfigurationManager.GetConfigurationAsync(this.Context.RequestAborted);
          jwtBearerHandler._configuration = configurationAsync;
          jwtBearerHandler = (LusvaJwtBearerHandler) null;
        }
        TokenValidationParameters validationParameters = this.Options.TokenValidationParameters.Clone();
        if (this._configuration != null)
        {
          if (validationParameters.ValidIssuer == null && !string.IsNullOrEmpty(this._configuration.Issuer))
          {
            validationParameters.ValidIssuer = this._configuration.Issuer;
          }
          else
          {
            string[] strArray = new string[1]
            {
              this._configuration.Issuer
            };
            validationParameters.ValidIssuers = validationParameters.ValidIssuers == null
              ? (IEnumerable<string>) strArray
              : (IEnumerable<string>)
              Enumerable.Concat<string>((IEnumerable<string>) validationParameters.ValidIssuers,
                (IEnumerable<string>) strArray);
          }
          validationParameters.IssuerSigningKeys = validationParameters.IssuerSigningKeys == null
            ? (IEnumerable<SecurityKey>) this._configuration.SigningKeys
            : (IEnumerable<SecurityKey>)
            Enumerable.Concat<SecurityKey>((IEnumerable<SecurityKey>) validationParameters.IssuerSigningKeys,
              (IEnumerable<SecurityKey>) this._configuration.SigningKeys);
        }
        List<Exception> validationFailures = (List<Exception>) null;
        foreach (
          ISecurityTokenValidator securityTokenValidator in
          (IEnumerable<ISecurityTokenValidator>) this.Options.SecurityTokenValidators)
        {
          if (securityTokenValidator.CanReadToken(token))
          {
            ClaimsPrincipal principal;
            SecurityToken validatedToken;
            try
            {
              principal = securityTokenValidator.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
              this.Logger.TokenValidationFailed(token, ex);
              if (this.Options.RefreshOnIssuerKeyNotFound && this.Options.ConfigurationManager != null &&
                  ex is SecurityTokenSignatureKeyNotFoundException)
                this.Options.ConfigurationManager.RequestRefresh();
              if (validationFailures == null)
                validationFailures = new List<Exception>(1);
              validationFailures.Add(ex);
              continue;
            }
            this.Logger.TokenValidationSucceeded();
            AuthenticationTicket authenticationTicket1 = new AuthenticationTicket(principal,
              new AuthenticationProperties(), this.Options.AuthenticationScheme);
            TokenValidatedContext validatedContext = new TokenValidatedContext(this.Context, this.Options);
            AuthenticationTicket authenticationTicket2 = authenticationTicket1;
            validatedContext.Ticket = authenticationTicket2;
            SecurityToken securityToken = validatedToken;
            validatedContext.SecurityToken = securityToken;
            TokenValidatedContext tokenValidatedContext = validatedContext;
            await this.Options.Events.TokenValidated(tokenValidatedContext);
            if (tokenValidatedContext.CheckEventResult(out result))
              return result;
            AuthenticationTicket ticket = tokenValidatedContext.Ticket;
            if (this.Options.SaveToken)
            {
              AuthenticationProperties properties = ticket.Properties;
              AuthenticationToken[] authenticationTokenArray = new AuthenticationToken[1];
              int index = 0;
              AuthenticationToken authenticationToken = new AuthenticationToken();
              authenticationToken.Name = "access_token";
              string str = token;
              authenticationToken.Value = str;
              authenticationTokenArray[index] = authenticationToken;
              properties.StoreTokens((IEnumerable<AuthenticationToken>) authenticationTokenArray);
            }
            return AuthenticateResult.Success(ticket);
          }
        }
        if (validationFailures == null)
          return AuthenticateResult.Fail("No SecurityTokenValidator available for token: " + token ?? "[null]");
        AuthenticationFailedContext authenticationFailedContext = new AuthenticationFailedContext(this.Context,
          this.Options)
        {
          Exception =
            validationFailures.Count == 1
              ? validationFailures[0]
              : (Exception) new AggregateException((IEnumerable<Exception>) validationFailures)
        };
        await this.Options.Events.AuthenticationFailed(authenticationFailedContext);
        return !authenticationFailedContext.CheckEventResult(out result)
          ? AuthenticateResult.Fail(authenticationFailedContext.Exception)
          : result;
      }
      catch (Exception ex)
      {
        obj = (object) ex;
        num = 1;
      }
      if (num == 1)
      {
        Exception ex = (Exception) obj;
        this.Logger.ErrorProcessingMessage(ex);
        AuthenticationFailedContext authenticationFailedContext = new AuthenticationFailedContext(this.Context,
          this.Options)
        {
          Exception = ex
        };
        await this.Options.Events.AuthenticationFailed(authenticationFailedContext);
        if (authenticationFailedContext.CheckEventResult(out result))
          return result;
        Exception source = obj as Exception;
        if (source == null)
          throw (Exception) obj;
        ExceptionDispatchInfo.Capture(source).Throw();
        authenticationFailedContext = (AuthenticationFailedContext) null;
      }
      obj = (object) null;
      token = (string) null;
      AuthenticateResult authenticateResult = null;
      return authenticateResult;
    }

    protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
    {
      AuthenticateResult authenticateResult = await this.HandleAuthenticateOnceAsync();
      JwtBearerChallengeContext eventContext = new JwtBearerChallengeContext(this.Context, this.Options,
        new AuthenticationProperties(context.Properties))
      {
        AuthenticateFailure = authenticateResult != null ? authenticateResult.Failure : (Exception) null
      };
      if (this.Options.IncludeErrorDetails && eventContext.AuthenticateFailure != null)
      {
        eventContext.Error = "invalid_token";
        eventContext.ErrorDescription = LusvaJwtBearerHandler.CreateErrorDescription(eventContext.AuthenticateFailure);
      }
      await this.Options.Events.Challenge(eventContext);
      if (eventContext.HandledResponse)
        return true;
      if (eventContext.Skipped)
        return false;
      this.Response.StatusCode = 401;
      if (string.IsNullOrEmpty(eventContext.Error) && string.IsNullOrEmpty(eventContext.ErrorDescription) &&
          string.IsNullOrEmpty(eventContext.ErrorUri))
      {
        this.Response.Headers.Add("WWW-Authenticate", (StringValues) this.Options.Challenge);
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder(this.Options.Challenge);
        if (this.Options.Challenge.IndexOf(" ", StringComparison.Ordinal) > 0)
          stringBuilder.Append(',');
        if (!string.IsNullOrEmpty(eventContext.Error))
        {
          stringBuilder.Append(" error=\"");
          stringBuilder.Append(eventContext.Error);
          stringBuilder.Append("\"");
        }
        if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
        {
          if (!string.IsNullOrEmpty(eventContext.Error))
            stringBuilder.Append(",");
          stringBuilder.Append(" error_description=\"");
          stringBuilder.Append(eventContext.ErrorDescription);
          stringBuilder.Append('"');
        }
        if (!string.IsNullOrEmpty(eventContext.ErrorUri))
        {
          if (!string.IsNullOrEmpty(eventContext.Error) || !string.IsNullOrEmpty(eventContext.ErrorDescription))
            stringBuilder.Append(",");
          stringBuilder.Append(" error_uri=\"");
          stringBuilder.Append(eventContext.ErrorUri);
          stringBuilder.Append('"');
        }
        this.Response.Headers.Add("WWW-Authenticate", (StringValues) stringBuilder.ToString());
      }
      return false;
    }

    private static string CreateErrorDescription(Exception authFailure)
    {
      IEnumerable<Exception> exceptions;
      if (authFailure is AggregateException)
        exceptions = (IEnumerable<Exception>) (authFailure as AggregateException).InnerExceptions;
      else
        exceptions = (IEnumerable<Exception>) new Exception[1]
        {
          authFailure
        };
      List<string> stringList = new List<string>();
      foreach (Exception exception in exceptions)
      {
        if (exception is SecurityTokenInvalidAudienceException)
          stringList.Add("The audience is invalid");
        else if (exception is SecurityTokenInvalidIssuerException)
          stringList.Add("The issuer is invalid");
        else if (exception is SecurityTokenNoExpirationException)
          stringList.Add("The token has no expiration");
        else if (exception is SecurityTokenInvalidLifetimeException)
          stringList.Add("The token lifetime is invalid");
        else if (exception is SecurityTokenNotYetValidException)
          stringList.Add("The token is not valid yet");
        else if (exception is SecurityTokenExpiredException)
          stringList.Add("The token is expired");
        else if (exception is SecurityTokenSignatureKeyNotFoundException)
          stringList.Add("The signature key was not found");
        else if (exception is SecurityTokenInvalidSignatureException)
          stringList.Add("The signature is invalid");
      }
      return string.Join("; ", (IEnumerable<string>) stringList);
    }

    protected override Task HandleSignOutAsync(SignOutContext context)
    {
      throw new NotSupportedException();
    }

    protected override Task HandleSignInAsync(SignInContext context)
    {
      throw new NotSupportedException();
    }
  }
}