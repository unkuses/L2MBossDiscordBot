using BossBot.DBModel;

namespace BossBot;

public static class BossCollection
{
    public static List<BossDbModel> GetBossesCollection()
        =>
        [
            new BossDbModel(name: "Basila", chance: 50, location: "Gludio", nickName: "базил", respawnTime: 4),
            new BossDbModel(name: "Chertuba", chance: 50, location: "Gludio", nickName: "чертуба", respawnTime: 6),
            new BossDbModel(name: "Kelsus", chance: 50, location: "Gludio", nickName: "кельсус", respawnTime: 10),
            new BossDbModel(name: "Queen Ant", chance: 33, location: "Gludio", nickName: "квина", respawnTime: 6, restartRespawnTime:8),
            new BossDbModel(name: "Saban", chance: 50, location: "Gludio", nickName: "сабянин", respawnTime: 12),
            // Dion
            new BossDbModel(name: "Contaminated Cruma", chance: 100, location: "Dion", nickName: "крума",
                respawnTime: 8),
            new BossDbModel(name: "Core Susceptor", chance: 33, location: "Dion", nickName: "ядро", respawnTime: 10, restartRespawnTime:8),
            new BossDbModel(name: "Enkura", chance: 50, location: "Dion", nickName: "энкура", respawnTime: 6),
            new BossDbModel(name: "Felis", chance: 100, location: "Dion", nickName: "фелис", respawnTime: 3),
            new BossDbModel(name: "Katan", chance: 100, location: "Dion", nickName: "катан", respawnTime: 10),
            new BossDbModel(name: "Mutated Cruma", chance: 100, location: "Dion", nickName: "мутант", respawnTime: 8, restartRespawnTime:8),
            new BossDbModel(name: "Pan Draaed", chance: 100, location: "Dion", nickName: "драйд", respawnTime: 12),
            new BossDbModel(name: "Sarka", chance: 100, location: "Dion", nickName: "шарка", respawnTime: 10),
            new BossDbModel(name: "Stonegeist", chance: 100, location: "Dion", nickName: "каменук", respawnTime: 7),
            new BossDbModel(name: "Talakin", chance: 100, location: "Dion", nickName: "талакин", respawnTime: 10),
            new BossDbModel(name: "Timitris", chance: 100, location: "Dion", nickName: "тимитрис", respawnTime: 8),
            new BossDbModel(name: "Valefar", chance: 100, location: "Dion", nickName: "буря", respawnTime: 6),
            // Giran 
            new BossDbModel(location: "Giran", respawnTime: 9, name: "Behemoth", chance: 100, nickName: "чудовище"),
            new BossDbModel(location: "Giran", respawnTime: 12, name: "Black Lily", chance: 100, nickName: "лилия", restartRespawnTime:8),
            new BossDbModel(location: "Giran", respawnTime: 6, name: "Breka", chance: 50, nickName: "брека"),
            new BossDbModel(location: "Giran", respawnTime: 6, name: "Matura", chance: 50, nickName: "матура"),
            new BossDbModel(location: "Giran", respawnTime: 10, name: "Medusa", chance: 100, nickName: "медуза"),
            new BossDbModel(location: "Giran", respawnTime: 5, name: "Pan Narod", chance: 50, nickName: "марод"),
            new BossDbModel(location: "Giran", respawnTime: 12, name: "Dragon Beast", chance: 33, nickName: "дракон", restartRespawnTime:14),
            // Oren
            new BossDbModel(location: "Oren", name: "Tromba", chance: 50, respawnTime: 7, nickName: "фаробос"),
            new BossDbModel(location: "Oren", name: "Gahareth", chance: 50, respawnTime: 9, nickName: "гарет"),
            new BossDbModel(location: "Oren", name: "Talkin", chance: 33, respawnTime: 8, nickName: "талкин"),
            new BossDbModel(location: "Oren", name: "Selu", chance: 33, respawnTime: 12, nickName: "селу"),
            new BossDbModel(location: "Oren", name: "Balbo", chance: 50, respawnTime: 12, nickName: "бальбо"),
            new BossDbModel(location: "Oren", name: "Timiniel", chance: 100, respawnTime: 8, nickName: "тиминиэль"),
            new BossDbModel(location: "Oren", name: "Orfen", chance: 33, respawnTime: 24, nickName: "орфен", restartRespawnTime:14),
            new BossDbModel(location: "Oren", name: "Repiro", chance: 50, respawnTime: 7, nickName: "репиро"),
            new BossDbModel(location: "Oren", name: "Coroon", chance: 100, respawnTime: 12, nickName: "корун",restartRespawnTime:8),
            new BossDbModel(location: "Oren", name: "Samuel", chance: 100, respawnTime: 12, nickName: "самуэль", restartRespawnTime:8),
            // Aden
            new BossDbModel(location: "Aden", name: "Oblivion Mirror", chance: 100, respawnTime: 11,
                nickName: "зеркало", restartRespawnTime:8),
            new BossDbModel(location: "Aden", name: "Hisilrome", chance: 50, respawnTime: 6, nickName: "хисилром", restartRespawnTime:8),
            new BossDbModel(location: "Aden", name: "Landor", chance: 100, respawnTime: 9, nickName: "ландор", restartRespawnTime:8),
            new BossDbModel(location: "Aden", name: "Flynt", chance: 33, respawnTime: 5, nickName: "фоллинт", restartRespawnTime:14),
            new BossDbModel(location: "Aden", name: "Cabrio", chance: 50, respawnTime: 12, nickName: "кабрио", restartRespawnTime:14),
            new BossDbModel(location: "Aden", name: "Andras", chance: 50, respawnTime: 15, nickName: "андрас", restartRespawnTime:14),
            new BossDbModel(location: "Aden", name: "Haff", chance: 33, respawnTime: 20, nickName: "хафф", restartRespawnTime:14),
            new BossDbModel(location: "Aden", name: "Glaki", chance: 100, respawnTime: 8, nickName: "глаки", restartRespawnTime:14),
            new BossDbModel(location: "Aden", name: "Olkuth", chance: 33, respawnTime: 24, nickName: "олкут", restartRespawnTime:14),
            new BossDbModel(location: "Aden", name: "Rahha", chance: 33, respawnTime: 33, nickName: "рахха", restartRespawnTime:14),
            new BossDbModel(location: "Aden", name: "Thanatos", chance: 33, respawnTime: 25, nickName: "танатос", restartRespawnTime:14),
            //Heine
            new BossDbModel(location: "Heine", name: "Phoenix", chance: 66, respawnTime: 24, nickName: "птиц", restartRespawnTime:14),
            new BossDbModel(location: "Heine", name: "Naiad", chance: 33, respawnTime: 15, nickName: "ная", restartRespawnTime:14),
            new BossDbModel(location: "Heine", name: "Modeus", chance: 33, respawnTime: 24, nickName: "мудя", restartRespawnTime:14),
            new BossDbModel(location: "Heine", name: "Valak", chance: 33, respawnTime: 20, nickName:"Гавнобоссина", restartRespawnTime: 8)
        ];
}