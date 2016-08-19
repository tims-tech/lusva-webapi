﻿

using System.ComponentModel.DataAnnotations;

namespace LUSVA.WebApi.Models
{
  public class SignInModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }
}