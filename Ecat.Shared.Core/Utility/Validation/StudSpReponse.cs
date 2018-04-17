using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Shared.Core.ModelLibrary.Learner;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.Core.Utility.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StudSpReponseValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var response = value as SpResponse;
            if (response == null)
            {
                ErrorMessage = "Attribute allow only on a student spResponse class";
                return new ValidationResult(ErrorMessage);
            }

            var mapResponse = typeof(MpSpItemResponse);
            var listOfApprovedResponses = mapResponse.GetFields().Where(field => field.IsLiteral).Select(field => field.GetValue(null).ToString()).ToList();

            if (!listOfApprovedResponses.Contains(response.MpItemResponse))
            {
                ErrorMessage = $"The item resppnse must be of a known value: present value is {response.MpItemResponse}";
                return new ValidationResult(ErrorMessage);
            }

            if (response.ItemModelScore >= 0 && response.ItemModelScore <= 6) return ValidationResult.Success;

            ErrorMessage = $"Item model score is not valid, out of range 0-6, score is {response.ItemModelScore}";

            return new ValidationResult(ErrorMessage);
        }
    }
}
