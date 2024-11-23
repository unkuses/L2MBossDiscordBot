using BossBot.DBModel;

namespace BossBot;

public static class BossCollection
{
    public static List<BossDbModel> GetBossesCollection()
        =>
        [
            new BossDbModel(name: "Basila", chance: 50, location: "Gludio", nickName: "базил", respawnTime: 4)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Базил"), new(name: "Basila") }
            },
            new BossDbModel(name: "Chertuba", chance: 50, location: "Gludio", nickName: "чертуба", respawnTime: 6)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Чертуба"), new(name: "Chertuba") }
            },
            new BossDbModel(name: "Kelsus", chance: 50, location: "Gludio", nickName: "кельсус", respawnTime: 10)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Кельсус"), new(name: "Kelsus") }
            },
            new BossDbModel(name: "Queen Ant", chance: 33, location: "Gludio", nickName: "квина", respawnTime: 6,
                restartRespawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>()
                    { new(name: "Королева Муравьев"), new(name: "Queen") }
            },
            new BossDbModel(name: "Saban", chance: 100, location: "Gludio", nickName: "сабянин", respawnTime: 12)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Сабан"), new(name: "Saban") }
            },
            // Dion
            new BossDbModel(name: "Contaminated Cruma", chance: 100, location: "Dion",
                nickName: "крума",
                respawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>()
                    { new(name: "Зараженный Крума"), new(name: "Contaminated Cruma"), }
            },
            new BossDbModel(name: "Core Susceptor", chance: 33, location: "Dion",
                nickName: "ядро", respawnTime: 10, restartRespawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Core"), new(name: "Ядр") }
            },
            new BossDbModel(name: "Enkura", chance: 50, location: "Dion", nickName: "энкура",
                    respawnTime: 6)
                { BossNames = new List<BossNamesDBModel>() { new(name: "Энкура"), new(name: "Enkura") } },
            new BossDbModel(name: "Felis", chance: 100, location: "Dion", nickName: "фелис",
                    respawnTime: 3)
                { BossNames = new List<BossNamesDBModel>() { new(name: "Фелис"), new(name: "Felis") } },
            new BossDbModel(name: "Katan", chance: 100, location: "Dion", nickName: "катан",
                    respawnTime: 10, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new(name: "Катан"), new(name: "Katan") } },
            new BossDbModel(name: "Mutated Cruma", chance: 100, location: "Dion",
                nickName: "мутант", respawnTime: 8, restartRespawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>()
                {
                    new(name: "Mutated Cruma"), new(name: "Мутант Крума"), new(name: "Cruma of Menace"),
                    new(name: "Безумный Крума")
                }
            },
            new BossDbModel(name: "Pan Draaed", chance: 100, location: "Dion",
                    nickName: "драйд", respawnTime: 12)
                { BossNames = new List<BossNamesDBModel>() { new("Pan Draaed"), new("Пан Драйд") } },
            new BossDbModel(name: "Sarka", chance: 100, location: "Dion", nickName: "шарка",
                    respawnTime: 10)
                { BossNames = new List<BossNamesDBModel>() { new("Sarka"), new("Шарка") } },
            new BossDbModel(name: "Stonegeist", chance: 100, location: "Dion",
                    nickName: "каменук", respawnTime: 7)
                { BossNames = new List<BossNamesDBModel>() { new("Каменук"), new("Stonegeist") } },
            new BossDbModel(name: "Talakin", chance: 100, location: "Dion", nickName: "талакин",
                    respawnTime: 10)
                { BossNames = new List<BossNamesDBModel>() { new("Талакин"), new("Talakin") } },
            new BossDbModel(name: "Timitris", chance: 100, location: "Dion",
                    nickName: "тимитрис", respawnTime: 8)
                { BossNames = new List<BossNamesDBModel>() { new("Тимитрис"), new("Timitris") } },
            new BossDbModel(name: "Valefar", chance: 100, location: "Dion", nickName: "буря",
                    respawnTime: 6)
                { BossNames = new List<BossNamesDBModel>() { new("Буря"), new("Valefar") } },
            // Giran 
            new BossDbModel(location: "Giran", respawnTime: 9, name: "Behemoth", chance: 100,
                    nickName: "чудовище")
                { BossNames = new List<BossNamesDBModel>() { new("Чудовище"), new("Behemoth") } },
            new BossDbModel(location: "Giran", respawnTime: 12, name: "Black Lily",
                chance: 100, nickName: "лилия", restartRespawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Черная Лилия"), new("Black Lily") }
            },
            new BossDbModel(location: "Giran", respawnTime: 6, name: "Breka", chance: 50,
                    nickName: "брека")
                { BossNames = new List<BossNamesDBModel>() { new("Брека"), new("Breka") } },
            new BossDbModel(location: "Giran", respawnTime: 6, name: "Matura", chance: 50,
                    nickName: "матура")
                { BossNames = new List<BossNamesDBModel>() { new("Матура"), new("Matura") } },
            new BossDbModel(location: "Giran", respawnTime: 10, name: "Medusa", chance: 100,
                    nickName: "медуза")
                { BossNames = new List<BossNamesDBModel>() { new("Медуза"), new("Medusa") } },
            new BossDbModel(location: "Giran", respawnTime: 5, name: "Pan Narod", chance: 50,
                    nickName: "марод")
                { BossNames = new List<BossNamesDBModel>() { new("Пан Марод"), new("Pan Narod") } },
            new BossDbModel(location: "Giran", respawnTime: 12, name: "Dragon Beast",
                chance: 33, nickName: "дракон", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Чудовищный Дракон"), new("Dragon Beast") }
            },
            // Oren
            new BossDbModel(location: "Oren", name: "Tromba", chance: 50, respawnTime: 7,
                    nickName: "фаробос")
                { BossNames = new List<BossNamesDBModel>() { new("Фаробос"), new("Tromba") } },
            new BossDbModel(location: "Oren", name: "Gahareth", chance: 50, respawnTime: 9,
                    nickName: "гарет")
                { BossNames = new List<BossNamesDBModel>() { new("Гарет"), new("Gahareth") } },
            new BossDbModel(location: "Oren", name: "Talkin", chance: 33, respawnTime: 8,
                    nickName: "талкин")
                { BossNames = new List<BossNamesDBModel>() { new("Талкин"), new("Talkin") } },
            new BossDbModel(location: "Oren", name: "Selu", chance: 33, respawnTime: 12,
                    nickName: "селу")
                { BossNames = new List<BossNamesDBModel>() { new("Селу"), new("Selu") } },
            new BossDbModel(location: "Oren", name: "Balbo", chance: 50, respawnTime: 12,
                    nickName: "бальбо")
                { BossNames = new List<BossNamesDBModel>() { new("Бальбо"), new("Balbo") } },
            new BossDbModel(location: "Oren", name: "Timiniel", chance: 100, respawnTime: 8,
                    nickName: "тиминиэль")
                { BossNames = new List<BossNamesDBModel>() { new("Тиминиэль"), new("Timiniel") } },
            new BossDbModel(location: "Oren", name: "Orfen", chance: 33, respawnTime: 24,
                    nickName: "орфен", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Орфен"), new("Orfen") } },
            new BossDbModel(location: "Oren", name: "Repiro", chance: 50, respawnTime: 7,
                    nickName: "репиро")
                { BossNames = new List<BossNamesDBModel>() { new("Репиро"), new("Repiro") } },
            new BossDbModel(location: "Oren", name: "Coroon", chance: 100, respawnTime: 12,
                    nickName: "корун", restartRespawnTime: 8)
                { BossNames = new List<BossNamesDBModel>() { new("Корун"), new("Coroon") } },
            new BossDbModel(location: "Oren", name: "Samuel", chance: 100, respawnTime: 12,
                    nickName: "самуэль", restartRespawnTime: 8, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Самуэль"), new("Samuel") } },
            // Aden
            new BossDbModel(location: "Aden", name: "Mirror of Oblivion ", chance: 100,
                    respawnTime: 11,
                    nickName: "зеркало", restartRespawnTime: 8, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Зеркало"), new("Mirror") } },
            new BossDbModel(location: "Aden", name: "Hisilrome", chance: 50, respawnTime: 6,
                    nickName: "хисилром", restartRespawnTime: 8)
                { BossNames = new List<BossNamesDBModel>() { new("Хисилром"), new("Hisilrome") } },
            new BossDbModel(location: "Aden", name: "Landor", chance: 100, respawnTime: 9,
                    nickName: "ландор", restartRespawnTime: 8)
                { BossNames = new List<BossNamesDBModel>() { new("Ландор"), new("Landor") } },
            new BossDbModel(location: "Aden", name: "Flynt", chance: 33, respawnTime: 5,
                    nickName: "фоллинт", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Фоллинт"), new("Flynt") } },
            new BossDbModel(location: "Aden", name: "Cabrio", chance: 50, respawnTime: 12,
                    nickName: "кабрио", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Кабрио"), new("Cabrio") } },
            new BossDbModel(location: "Aden", name: "Andras", chance: 50, respawnTime: 15,
                    nickName: "андрас", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Андрас"), new("Andras") } },
            new BossDbModel(location: "Aden", name: "Haff", chance: 33, respawnTime: 20,
                    nickName: "хафф", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Хафф"), new("Haff") } },
            new BossDbModel(location: "Aden", name: "Glaki", chance: 100, respawnTime: 8,
                    nickName: "глаки", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Глаки"), new("Glaki") } },
            new BossDbModel(location: "Aden", name: "Olkuth", chance: 33, respawnTime: 24,
                    nickName: "олкут", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Олкут"), new("Olkuth") } },
            new BossDbModel(location: "Aden", name: "Rahha", chance: 33, respawnTime: 33,
                nickName: "рахха", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>()
                    { new("Рахха") { Name = "Базил" }, new("Rahha") { Name = "Basila" } }
            },
            new BossDbModel(location: "Aden", name: "Thanatos", chance: 33, respawnTime: 25,
                    nickName: "танатос", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Танатос"), new("Thanatos") } },
            //Heine
            new BossDbModel(location: "Heine", name: "Phoenix", chance: 33, respawnTime: 24,
                    nickName: "птиц", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Феникс"), new("Phoenix") } },
            new BossDbModel(location: "Heine", name: "Naiad", chance: 33, respawnTime: 15,
                    nickName: "ная", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Наяда"), new("Naiad") } },
            new BossDbModel(location: "Heine", name: "Modeus", chance: 33, respawnTime: 24,
                    nickName: "мудя", restartRespawnTime: 14, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Модеус"), new("Modeus") } },
            new BossDbModel(location: "Heine", name: "Valak", chance: 33, respawnTime: 20,
                    nickName: "Гавнобоссина", restartRespawnTime: 8, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Баллак"), new("Valak") } },
            new BossDbModel(location: "Heine", name: "Cyrex", nickName: "Курис", chance: 33,
                    respawnTime: 24, restartRespawnTime:8, purpleDrop:true)
                { BossNames = new List<BossNamesDBModel>() { new("Сайракс"), new("Cyrex") } }
        ];
}