using FluentValidation;
using HospitalAppointment_core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Validators
{
    public class PatientDTOValidator : AbstractValidator<PatientDTO>
    {
        public PatientDTOValidator() { 
            RuleFor(x => x.FirstName).NotEmpty();

            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.TcKimlikNo)
                .NotEmpty()
                .Length(11)
                .Must(BeValidTcKimlik)
                .WithMessage("Geçersiz TC Kimlik Numarası!");
        }

        private bool BeValidTcKimlik(string tcKimlikNo)
        {
            if (tcKimlikNo.Length != 11 || !tcKimlikNo.All(char.IsDigit))
            {
                return false;
            }
            int[] digits = tcKimlikNo.Select(c => int.Parse(c.ToString())).ToArray();
            // TC Kimlik No doğrulama algoritması
            int sumOdd = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int sumEven = digits[1] + digits[3] + digits[5] + digits[7];
            int digit10 = ((sumOdd * 7) - sumEven) % 10;
            int digit11 = (digits.Take(10).Sum()) % 10;
            return digit10 == digits[9] && digit11 == digits[10];
        }

    }
}
