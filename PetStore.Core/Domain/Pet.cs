using System;

namespace PetStore.Core.Domain
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public PetType Type { get; set; }

        private bool IsNameMinimumLength()
        {
            return !String.IsNullOrEmpty(Name) && Name.Length >= 5;
        }

        private bool IsUnderMaxAge()
        {
            var maxAge = DateTime.UtcNow.AddYears(-20);
            return DateOfBirth > maxAge;
        }

        private bool IsUnderMaxWeight()
        {
            var maxWeight = 50;
            return Weight <= maxWeight;
        }

        public bool IsValid()
        {
            return IsUnderMaxAge() && IsUnderMaxWeight() && IsNameMinimumLength();
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Date of Birth: {DateOfBirth.ToShortDateString()}, Weight: {Weight}, Type: {Type}";
        }
    }
}