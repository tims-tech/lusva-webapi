using System;

namespace LUSVA.WebApi.Services
{
  public class InvalidTokenException : Exception
  {
    public InvalidTokenException(string message) : base(message)
    {
    }
  }
}