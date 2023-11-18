﻿using FluentValidation;
using FSMS.Service.Enums;
using FSMS.Service.ViewModels.Fruits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSMS.Service.Validations.FruitFarmer
{
    public class UpdateFruitFarmerValidator : AbstractValidator<UpdateFruitFarmer>
    {
        public UpdateFruitFarmerValidator()
        {

            RuleFor(o => o.FruitName)
              .Cascade(CascadeMode.StopOnFirstFailure)
              .NotEmpty().WithMessage("{PropertyName} is empty")
              .Length(1, 100).WithMessage("{PropertyName} must be less than or equals 100 characters.");
            RuleFor(o => o.FruitDescription)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} is empty")
                .MaximumLength(200).WithMessage("{PropertyName} must be less than or equals 200 characters.");
            RuleFor(o => o.Price)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} is empty")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0");
            RuleFor(o => o.QuantityAvailable)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} is empty")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0");
            RuleFor(o => o.OriginCity)
               .NotEmpty().WithMessage("{PropertyName} is empty");
            RuleFor(g => g.OrderType)
               .NotEmpty().WithMessage("{PropertyName} is empty")
               .IsEnumName(typeof(OrderTypeEnum)).WithMessage("{PropertyName} must be Order or Pre-order");

        }
        protected bool IsValidImageExtension(string filename)
        {
            string validExtensionsPattern = @"\.(jpeg|png|bmp|webp)$";
            if (Regex.IsMatch(filename, validExtensionsPattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
