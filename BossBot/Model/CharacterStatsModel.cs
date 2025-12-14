using System.Text.Json;
using System.Text.Json.Serialization;

namespace BossBot.Model;

public record CharacterStatsModel
{
    [JsonPropertyName("Damage")] public int? Damage { get; set; } 
    [JsonPropertyName("Accuracy")] public int? Accuracy { get; set; }
    [JsonPropertyName("CritAtkPercent")] public int? CritAtkPercent { get; set; }

    [JsonPropertyName("bonusCritDamage")] public int? BonusCritDamage { get; set; }
    [JsonPropertyName("doubleDamageChancePercent")] public int? DoubleDamageChancePercent { get; set; }
    [JsonPropertyName("tripleDamageChancePercent")] public int? TripleDamageChancePercent { get; set; }
    [JsonPropertyName("weaponBlockPercent")] public int? WeaponBlockPercent { get; set; }

    // --- Defense ---
    [JsonPropertyName("defense")] public int? Defense { get; set; }
    [JsonPropertyName("skillResistance")] public int? SkillResistance { get; set; }

    // --- Damage increase / resist ---
    [JsonPropertyName("weaponDamageIncreasePercent")] public int? WeaponDamageIncreasePercent { get; set; } 
    [JsonPropertyName("weaponDamageResistancePercent")] public int? WeaponDamageResistancePercent { get; set; } 
    [JsonPropertyName("skillDamageIncreasePercent")] public int? SkillDamageIncreasePercent { get; set; }  
    [JsonPropertyName("skillDamageResistancePercent")] public int? SkillDamageResistancePercent { get; set; } 

    // --- Special resists / penetration / CC ---
    [JsonPropertyName("doubleDamageResistancePercent")] public int? DoubleDamageResistancePercent { get; set; } 
    [JsonPropertyName("tripleDamageResistancePercent")] public int? TripleDamageResistancePercent { get; set; } 
    [JsonPropertyName("blockPenetrationPercent")] public int? BlockPenetrationPercent { get; set; }
    [JsonPropertyName("ignoreDamageReduction")] public int? IgnoreDamageReduction { get; set; }
    [JsonPropertyName("stunChancePercent")] public int? StunChancePercent { get; set; }

    // --- Control resist / abnormal ---
    [JsonPropertyName("stunResistancePercent")] public int? StunResistancePercent { get; set; }  
    [JsonPropertyName("holdResistancePercent")] public int? HoldResistancePercent { get; set; } 
    [JsonPropertyName("aggroResistancePercent")] public int? AggroResistancePercent { get; set; }  
    [JsonPropertyName("silenceResistancePercent")] public int? SilenceResistancePercent { get; set; } 
    [JsonPropertyName("abnormalStatusChancePercent")] public int? AbnormalStatusChancePercent { get; set; } 
    [JsonPropertyName("abnormalStatusResistancePercent")] public int? AbnormalStatusResistancePercent { get; set; }
    [JsonPropertyName("abnormalStatusDurationReductionPercent")] public int? AbnormalStatusDurationReductionPercent { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}