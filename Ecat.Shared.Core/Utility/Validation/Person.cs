using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ecat.Shared.Core.ModelLibrary.User;

namespace Ecat.Shared.Core.Utility.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PersonMappedPropValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext vc)
        {
            var person = value as Person;
            if (person == null)
            {
                ErrorMessage = "Attribute allow only on a person class";
                return new ValidationResult(ErrorMessage);
            }

            var mapGender = typeof(MpGender);

            var listOfApprovedGender = mapGender.GetFields().Where(field => field.IsLiteral).Select(field => field.GetValue(null).ToString()).ToList();

            if (!listOfApprovedGender.Contains(person.MpGender))
            {
                ErrorMessage = "Gender must be of a known value";
                return new ValidationResult(ErrorMessage);
            }

            var mapMilAffil = typeof(MpAffiliation);

            var listOfApprovedAffil = mapMilAffil.GetFields().Where(field => field.IsLiteral).Select(field => field.GetValue(null).ToString()).ToList();

            if (!listOfApprovedAffil.Contains(person.MpAffiliation))
            {
                ErrorMessage = "Military Affiliation must be of a known value";
                return new ValidationResult(ErrorMessage);
            }

            var mapMilPaygrade = typeof(MpPaygrade);

            var listOfApprovedPaygrade = mapMilPaygrade.GetFields().Where(field => field.IsLiteral).Select(field => field.GetValue(null).ToString()).ToList();

            if (!listOfApprovedPaygrade.Contains(person.MpPaygrade))
            {
                ErrorMessage = "Military Paygrade must be of a known value";
                return new ValidationResult(ErrorMessage);
            }

            var mapMilComponent = typeof(MpComponent);

            var listOfApprovedComponent = mapMilComponent.GetFields().Where(field => field.IsLiteral).Select(field => field.GetValue(null).ToString()).ToList();

            if (listOfApprovedComponent.Contains(person.MpComponent)) return ValidationResult.Success;

            ErrorMessage = "Military Component must be of a known value";

            return new ValidationResult(ErrorMessage);
        }
    }

}