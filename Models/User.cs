using System.ComponentModel.DataAnnotations;

namespace Pinjet.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "نام و نام خانوادگی الزامی می باشد")]
        [MinLength(3,ErrorMessage ="حداقل سه کارکتر وارد کنید")]
        [MaxLength(100,ErrorMessage ="حداکثر 100 کارکتر مجاز است")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage ="ایمیل الزمی است")]
        [EmailAddress(ErrorMessage ="فرمت ایمیل صحیح نیست")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
