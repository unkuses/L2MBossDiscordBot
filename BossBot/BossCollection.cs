using BossBot.DBModel;

namespace BossBot
{
    public static class BossCollection
    {
        public static List<BossDbModel> GetBossesCollection()
        =>
        [
            new BossDbModel(name: "Basila", chance: 50, location: "Gludio", nickName: "базил", respawnTime: 4),
            new BossDbModel()
                { Name = "Chertuba", Chance = 50, Location = "Gludio", NickName = "чертуба", RespawnTime = 6 },
            new BossDbModel()
                { Name = "Kelsus", Chance = 50, Location = "Gludio", NickName = "кельсус", RespawnTime = 10 },
            new BossDbModel()
                { Name = "Queen Ant", Chance = 33, Location = "Gludio", NickName = "квина", RespawnTime = 6 },
            new BossDbModel()
                { Name = "Saban", Chance = 50, Location = "Gludio", NickName = "сабянин", RespawnTime = 12 },
            // Dion
            new BossDbModel()
            {
                Name = "Contaminated Cruma", Chance = 100, Location = "Dion", NickName = "крума", RespawnTime = 8
            },
            new BossDbModel()
                { Name = "Core Susceptor", Chance = 33, Location = "Dion", NickName = "ядро", RespawnTime = 10 },
            new BossDbModel()
                { Name = "Enkura", Chance = 50, Location = "Dion", NickName = "энкура", RespawnTime = 6 },
            new BossDbModel()
                { Name = "Felis", Chance = 100, Location = "Dion", NickName = "фелис", RespawnTime = 3 },
            new BossDbModel()
                { Name = "Katan", Chance = 100, Location = "Dion", NickName = "катан", RespawnTime = 10 },
            new BossDbModel()
                { Name = "Mutated Cruma", Chance = 100, Location = "Dion", NickName = "мутант", RespawnTime = 8 },
            new BossDbModel()
                { Name = "Pan Draaed", Chance = 100, Location = "Dion", NickName = "драйд", RespawnTime = 12 },
            new BossDbModel()
                { Name = "Sarka", Chance = 100, Location = "Dion", NickName = "шарка", RespawnTime = 10 },
            new BossDbModel()
                { Name = "Stonegeist", Chance = 100, Location = "Dion", NickName = "каменук", RespawnTime = 7 },
            new BossDbModel()
                { Name = "Talakin", Chance = 100, Location = "Dion", NickName = "талакин", RespawnTime = 10 },
            new BossDbModel()
                { Name = "Timitris", Chance = 100, Location = "Dion", NickName = "тимитрис", RespawnTime = 8 },
            new BossDbModel()
                { Name = "Valefar", Chance = 100, Location = "Dion", NickName = "буря", RespawnTime = 6 },
            //Giran 
            new BossDbModel()
                { Location = "Giran", RespawnTime = 9, Name = "Behemoth", Chance = 100, NickName = "чудовище", },
            new BossDbModel()
                { Location = "Giran", RespawnTime = 12, Name = "Black Lily", Chance = 100, NickName = "лилия", },
            new BossDbModel()
                { Location = "Giran", RespawnTime = 6, Name = "Breka", Chance = 50, NickName = "брека", },
            new BossDbModel()
                { Location = "Giran", RespawnTime = 6, Name = "Matura", Chance = 50, NickName = "матура", },
            new BossDbModel()
                { Location = "Giran", RespawnTime = 10, Name = "Medusa", Chance = 100, NickName = "медуза", },
            new BossDbModel()
                { Location = "Giran", RespawnTime = 5, Name = "Pan Narod", Chance = 100, NickName = "марод", },
            new BossDbModel()
                { Location = "Giran", RespawnTime = 12, Name = "Dragon Beast", Chance = 33, NickName = "дракон", },
            // Oren
            new BossDbModel()
                { Location = "Oren", Name = "Tromba", Chance = 50, RespawnTime = 7, NickName = "фаробос" },
            new BossDbModel()
                { Location = "Oren", Name = "Gahareth", Chance = 50, RespawnTime = 9, NickName = "гарет" },
            new BossDbModel()
                { Location = "Oren", Name = "Talkin", Chance = 33, RespawnTime = 8, NickName = "талкин" },
            new BossDbModel()
                { Location = "Oren", Name = "Selu", Chance = 33, RespawnTime = 12, NickName = "селу" },
            new BossDbModel()
                { Location = "Oren", Name = "Balbo", Chance = 50, RespawnTime = 12, NickName = "бальбо" },
            new BossDbModel()
                { Location = "Oren", Name = "Timiniel", Chance = 100, RespawnTime = 8, NickName = "тиминиэль" },
            new BossDbModel()
                { Location = "Oren", Name = "Orfen", Chance = 33, RespawnTime = 24, NickName = "орфен" },
            new BossDbModel()
                { Location = "Oren", Name = "Repiro", Chance = 50, RespawnTime = 7, NickName = "репиро" },
            new BossDbModel()
                { Location = "Oren", Name = "Coroon", Chance = 100, RespawnTime = 12, NickName = "корун" },
            new BossDbModel()
                { Location = "Oren", Name = "Samuel", Chance = 100, RespawnTime = 12, NickName = "самуэль" },
            // Aden
            new BossDbModel()
            {
                Location = "Aden", Name = "Oblivion Mirror", Chance = 100, RespawnTime = 11, NickName = "зеркало"
            },
            new BossDbModel()
                { Location = "Aden", Name = "Hisilrome", Chance = 50, RespawnTime = 6, NickName = "хисилром" },
            new BossDbModel()
                { Location = "Aden", Name = "Landor", Chance = 100, RespawnTime = 9, NickName = "ландор" },
            new BossDbModel()
                { Location = "Aden", Name = "Flynt", Chance = 33, RespawnTime = 5, NickName = "фоллинт" },
            new BossDbModel()
                { Location = "Aden", Name = "Cabrio", Chance = 50, RespawnTime = 12, NickName = "кабрио" },
            new BossDbModel()
                { Location = "Aden", Name = "Andras", Chance = 50, RespawnTime = 15, NickName = "андрас" },
            new BossDbModel()
                { Location = "Aden", Name = "Haff", Chance = 33, RespawnTime = 20, NickName = "хафф" },
            new BossDbModel()
                { Location = "Aden", Name = "Glaki", Chance = 100, RespawnTime = 8, NickName = "глаки" },
            new BossDbModel()
                { Location = "Aden", Name = "Olkuth", Chance = 33, RespawnTime = 24, NickName = "олкут" },
            new BossDbModel()
                { Location = "Aden", Name = "Rahha", Chance = 33, RespawnTime = 33, NickName = "рахха" },
            new BossDbModel()
                { Location = "Aden", Name = "Thanatos", Chance = 33, RespawnTime = 24, NickName = "танатос" },
            //Heine
            new BossDbModel()
                { Location = "Heine", Name = "Phoenix", Chance = 66, RespawnTime = 24, NickName = "птиц" },
            new BossDbModel()
                { Location = "Heine", Name = "Naiad", Chance = 33, RespawnTime = 15, NickName = "ная" },
            new BossDbModel()
                { Location = "Heine", Name = "Modeus", Chance = 33, RespawnTime = 24, NickName = "мудя" }
        ];

    }
}
