using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.DataAccess.Seed.Models;

namespace WX.B2C.User.Verification.DataAccess.Seed.Validators
{
    internal static class Validators
    {
        private static readonly Dictionary<Type, Action<object[]>> _validators = new();

        static Validators()
        {
            Register<VerificationPolicy>(Validate);
            Register<CheckVariant>(Validate);
            Register<ValidationRule>(Validate);
        }

        private static void Register<T>(Action<T> validate)
        {
            _validators.Add(typeof(T), InternalValidation(validate));

            static Action<object[]> InternalValidation(Action<T> validate) =>
                objects =>
                {
                    var parameters = objects.Select(param => (T)param).ToArray();
                    foreach (var parameter in parameters)
                    {
                        validate(parameter);
                    }
                };
        }

        public static T[] Validate<T>(this T[] entities)
        {
            var type = typeof(T);
            var canValidate = _validators.ContainsKey(type);
            if (canValidate)
            {
                _validators[type].Invoke(entities.Select(arg => (object)arg).ToArray());
            }

            return entities;
        }

        private static void Validate(VerificationPolicy verificationPolicy)
        {
            new VerificationPolicyValidator().ValidateAndThrow(verificationPolicy);
        }

        private static void Validate(CheckVariant checkVariant)
        {
            new CheckVariantValidator().ValidateAndThrow(checkVariant);
        }

        private static void Validate(ValidationRule rule)
        {
            new ValidationRuleValidator().ValidateAndThrow(rule);
        }
    }
}