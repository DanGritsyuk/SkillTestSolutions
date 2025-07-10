using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyDirectory.Common.Entities
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public virtual required Department Department { get; set; }

        /// <summary>
        /// Calculates the employee's current age based on their birth date
        /// </summary>
        /// <returns>
        /// The employee's age in full years as of today's date.
        /// </returns>
        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;

            // Корректировка если день рождения еще не наступил
            if (BirthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }

        /// <summary>
        /// Calculates the employee's tenure (years of service) based on their hire date
        /// </summary>
        /// <returns>
        /// The number of full years the employee has worked at the company as of today's date.
        /// </returns>
        public int GetYearsOfWork()
        {
            var today = DateTime.Today;
            var years = today.Year - HireDate.Year;

            if (HireDate.Date > today.AddYears(-years))
                years--;

            return years;
        }
    }
}
