namespace SmartCafe.API.Domain.Entities;

public class UserPet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Quan hệ 1-1 với User

    public string Name { get; set; } = "Capy";
    public string PetType { get; set; } = "Capybara"; // Capybara, Cat, Dog, Bunny
    public int Level { get; set; } = 1;
    public int CurrentExp { get; set; } = 0;

    public DateTime AdoptedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastFedAt { get; set; } // Theo dõi lần cuối nhận EXP từ đơn hàng

    // Navigation property
    public User User { get; set; } = null!;
}