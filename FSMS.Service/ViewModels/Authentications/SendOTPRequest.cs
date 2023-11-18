﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSMS.Service.ViewModels.Authentications
{
    public class SendOTPRequest
    {
        [Required(ErrorMessage = "User email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }

}
