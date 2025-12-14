using System.Text.Json.Serialization;

namespace BossBot.Model;

public record CharacterStatsModel
{
    [JsonPropertyName("Damage")] public int? Damage { get; init; } 
    [JsonPropertyName("Accuracy")] public int? Accuracy { get; init; }
    [JsonPropertyName("CritAtkPercent")] public int? CritAtkPercent { get; init; }

    [JsonPropertyName("bonusCritDamage")] public int? BonusCritDamage { get; init; }
    [JsonPropertyName("doubleDamageChancePercent")] public int? DoubleDamageChancePercent { get; init; }
    [JsonPropertyName("tripleDamageChancePercent")] public int? TripleDamageChancePercent { get; init; }
    [JsonPropertyName("weaponBlockPercent")] public int? WeaponBlockPercent { get; init; }

    // --- Defense ---
    [JsonPropertyName("defense")] public int? Defense { get; init; } 
    [JsonPropertyName("skillResistance")] public int? SkillResistance { get; init; }

    // --- Damage increase / resist ---
    [JsonPropertyName("weaponDamageIncreasePercent")] public int? WeaponDamageIncreasePercent { get; init; } 
    [JsonPropertyName("weaponDamageResistancePercent")] public int? WeaponDamageResistancePercent { get; init; } 
    [JsonPropertyName("skillDamageIncreasePercent")] public int? SkillDamageIncreasePercent { get; init; }  
    [JsonPropertyName("skillDamageResistancePercent")] public int? SkillDamageResistancePercent { get; init; } 

    // --- Special resists / penetration / CC ---
    [JsonPropertyName("doubleDamageResistancePercent")] public int? DoubleDamageResistancePercent { get; init; } 
    [JsonPropertyName("tripleDamageResistancePercent")] public int? TripleDamageResistancePercent { get; init; } 
    [JsonPropertyName("blockPenetrationPercent")] public int? BlockPenetrationPercent { get; init; }
    [JsonPropertyName("ignoreDamageReduction")] public int? IgnoreDamageReduction { get; init; }
    [JsonPropertyName("stunChancePercent")] public int? StunChancePercent { get; init; } 

    // --- Control resist / abnormal ---
    [JsonPropertyName("stunResistancePercent")] public int? StunResistancePercent { get; init; }  
    [JsonPropertyName("holdResistancePercent")] public int? HoldResistancePercent { get; init; } 
    [JsonPropertyName("aggroResistancePercent")] public int? AggroResistancePercent { get; init; }  
    [JsonPropertyName("silenceResistancePercent")] public int? SilenceResistancePercent { get; init; } 
    [JsonPropertyName("abnormalStatusChancePercent")] public int? AbnormalStatusChancePercent { get; init; } 
    [JsonPropertyName("abnormalStatusResistancePercent")] public int? AbnormalStatusResistancePercent { get; init; }
    [JsonPropertyName("abnormalStatusDurationReductionPercent")] public int? AbnormalStatusDurationReductionPercent { get; init; }
}