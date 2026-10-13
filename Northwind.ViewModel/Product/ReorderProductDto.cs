using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Product
{
    public class ReorderProductDto : IValidatableObject
    {
        public bool IgnoreLevel { get; set; } = false;

        [Range(1, short.MaxValue, ErrorMessage = "StockMinimum minimal bernilai 1.")]
        public short StockMinimum { get; set; } = 10;

        public short? SetStock { get; set; }
        public short? AddStock { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SetStock.HasValue && AddStock.HasValue)
            {
                yield return new ValidationResult("Hanya boleh mengisi salah satu antara SetStock atau AddStock.", [nameof(SetStock), nameof(AddStock)]);
            }

            if (!SetStock.HasValue && !AddStock.HasValue)
            {
                yield return new ValidationResult("Wajib mengisi salah satu antara SetStock atau AddStock.", [nameof(SetStock), nameof(AddStock)]);
            }

            if (SetStock.HasValue && SetStock.Value < 0)
            {
                yield return new ValidationResult("SetStock tidak boleh bernilai negatif.", [nameof(SetStock)]);
            }

            if (AddStock.HasValue && AddStock.Value <= 0)
            {
                yield return new ValidationResult("AddStock harus lebih besar dari 0.", [nameof(AddStock)]);
            }
        }
    }
}