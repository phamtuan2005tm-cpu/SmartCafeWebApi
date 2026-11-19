namespace SmartCafe.API.Domain.DTOs.Gamification;

public class PetResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PetType { get; set; } = string.Empty;
    public int Level { get; set; }
    public int CurrentExp { get; set; }
    public int ExpToNextLevel { get; set; } // Mốc EXP cần để lên cấp tiếp theo
    public int AgeInDays { get; set; }      // Tuổi pet tính từ ngày nhận nuôi
    public DateTime AdoptedAt { get; set; }
    public DateTime? LastFedAt { get; set; }
}