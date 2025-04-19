using System.ComponentModel.DataAnnotations;

namespace CommonLib.DBModels;

public class BossNamesDBModel
{
    public BossNamesDBModel(string name)
    {
        Name = name;
    } 
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    
    public int BossId { get; set; }
    
    public BossDbModel Boss { get; set; }
}