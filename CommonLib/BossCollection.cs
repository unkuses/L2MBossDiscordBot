using CommonLib.DBModels;

namespace CommonLib;

public static class BossCollection
{
    public static List<BossDbModel> GetBossesCollection()
        =>
        new List<BossDbModel>
        {
            new BossDbModel(id: 1, name: "Basila", chance: 50, location: "Gludio", nickName: "базил", respawnTime: 4)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Базил"), new(name: "Basila") }
            },
            new BossDbModel(id : 2, name: "Chertuba", chance: 50, location: "Gludio", nickName: "чертуба", respawnTime: 6)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Чертуба"), new(name: "Chertuba") }
            },
            new BossDbModel(id : 3, name: "Kelsus", chance: 50, location: "Gludio", nickName: "кельсус", respawnTime: 10)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Кельсус"), new(name: "Kelsus") }
            },
            new BossDbModel(id: 4, name: "Queen Ant", chance: 33, location: "Gludio", nickName: "квина", respawnTime: 6,
                restartRespawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>()
                    { new(name: "Королева Муравьев"), new(name: "Queen") }
            },
            new BossDbModel(id : 5, name: "Saban", chance: 100, location: "Gludio", nickName: "сабянин", respawnTime: 12)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Сабан"), new(name: "Saban") }
            },
            // Dion
            new BossDbModel(id: 6, name: "Contaminated Cruma", chance: 100, location: "Dion",
                nickName: "крума",
                respawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>()
                    { new(name: "Зараженный Крума"), new(name: "Contaminated Cruma") }
            },
            new BossDbModel(id: 7, name: "Core Susceptor", chance: 33, location: "Dion",
                nickName: "ядро", respawnTime: 10, restartRespawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Core"), new(name: "Ядр") }
            },
            new BossDbModel(id: 8,name: "Enkura", chance: 50, location: "Dion", nickName: "энкура",
                    respawnTime: 6)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Энкура"), new(name: "Enkura") }
            },
            new BossDbModel(id: 9,name: "Felis", chance: 100, location: "Dion", nickName: "фелис",
                    respawnTime: 3)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Фелис"), new(name: "Felis") }
            },
            new BossDbModel(id: 10, name: "Katan", chance: 100, location: "Dion", nickName: "катан",
                    respawnTime: 10, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new(name: "Катан"), new(name: "Katan") }
            },
            new BossDbModel(id: 11, name: "Mutated Cruma", chance: 100, location: "Dion",
                nickName: "мутант", respawnTime: 8, restartRespawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>()
                {
                    new(name: "Mutated Cruma"), new(name: "Мутант Крума"), new(name: "Cruma of Menace"),
                    new(name: "Безумный Крума")
                }
            },
            new BossDbModel(id: 12, name: "Pan Draaed", chance: 100, location: "Dion",
                    nickName: "драйд", respawnTime: 12)
            {
                BossNames = new List<BossNamesDBModel>() { new("Pan Draaed"), new("Пан Драйд") }
            },
            new BossDbModel(id: 13, name: "Sarka", chance: 100, location: "Dion", nickName: "шарка",
                    respawnTime: 10)
            {
                BossNames = new List<BossNamesDBModel>() { new("Sarka"), new("Шарка") }
            },
            new BossDbModel(id: 14, name: "Stonegeist", chance: 100, location: "Dion",
                    nickName: "каменук", respawnTime: 7)
            {
                BossNames = new List<BossNamesDBModel>() { new("Каменук"), new("Stonegeist") }
            },
            new BossDbModel(id: 15, name: "Talakin", chance: 100, location: "Dion", nickName: "талакин",
                    respawnTime: 10)
            {
                BossNames = new List<BossNamesDBModel>() { new("Талакин"), new("Talakin") }
            },
            new BossDbModel(id: 16, name: "Timitris", chance: 100, location: "Dion",
                    nickName: "тимитрис", respawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>() { new("Тимитрис"), new("Timitris") }
            },
            new BossDbModel(id: 17, name: "Valefar", chance: 100, location: "Dion", nickName: "буря",
                    respawnTime: 6)
            {
                BossNames = new List<BossNamesDBModel>() { new("Буря"), new("Valefar") }
            },
            // Giran
            new BossDbModel(id: 18, location: "Giran", respawnTime: 9, name: "Behemoth", chance: 100,
                    nickName: "чудовище")
            {
                BossNames = new List<BossNamesDBModel>() { new("Чудовище"), new("Behemoth") }
            },
            new BossDbModel(id: 19, location: "Giran", respawnTime: 12, name: "Black Lily",
                chance: 100, nickName: "лилия", restartRespawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Черная Лилия"), new("Black Lily") }
            },
            new BossDbModel(id: 20, location: "Giran", respawnTime: 6, name: "Breka", chance: 50,
                    nickName: "брека")
            {
                BossNames = new List<BossNamesDBModel>() { new("Брека"), new("Breka") }
            },
            new BossDbModel(id: 21, location: "Giran", respawnTime: 6, name: "Matura", chance: 50,
                    nickName: "матура")
            {
                BossNames = new List<BossNamesDBModel>() { new("Матура"), new("Matura") }
            },
            new BossDbModel(id: 22, location: "Giran", respawnTime: 10, name: "Medusa", chance: 100,
                    nickName: "медуза")
            {
                BossNames = new List<BossNamesDBModel>() { new("Медуза"), new("Medusa") }
            },
            new BossDbModel(id: 23, location: "Giran", respawnTime: 5, name: "Pan Narod", chance: 50,
                    nickName: "марод")
            {
                BossNames = new List<BossNamesDBModel>() { new("Пан Марод"), new("Pan Narod") }
            },
            new BossDbModel(id: 24, location: "Giran", respawnTime: 12, name: "Dragon Beast",
                chance: 33, nickName: "дракон", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Чудовищный Дракон"), new("Dragon Beast") }
            },
            // Oren
            new BossDbModel(id: 25, location: "Oren", name: "Tromba", chance: 50, respawnTime: 7,
                    nickName: "фаробос")
            {
                BossNames = new List<BossNamesDBModel>() { new("Фаробос"), new("Tromba") }
            },
            new BossDbModel(id: 26, location: "Oren", name: "Gahareth", chance: 50, respawnTime: 9,
                    nickName: "гарет")
            {
                BossNames = new List<BossNamesDBModel>() { new("Гарет"), new("Gahareth") }
            },
            new BossDbModel(id: 27, location: "Oren", name: "Talkin", chance: 33, respawnTime: 8,
                    nickName: "талкин")
            {
                BossNames = new List<BossNamesDBModel>() { new("Талкин"), new("Talkin") }
            },
            new BossDbModel(id: 28, location: "Oren", name: "Selu", chance: 33, respawnTime: 12,
                    nickName: "селу")
            {
                BossNames = new List<BossNamesDBModel>() { new("Селу"), new("Selu") }
            },
            new BossDbModel(id: 29, location: "Oren", name: "Balbo", chance: 50, respawnTime: 12,
                    nickName: "бальбо")
            {
                BossNames = new List<BossNamesDBModel>() { new("Бальбо"), new("Balbo") }
            },
            new BossDbModel(id: 30, location: "Oren", name: "Timiniel", chance: 100, respawnTime: 8,
                    nickName: "тиминиэль")
            {
                BossNames = new List<BossNamesDBModel>() { new("Тиминиэль"), new("Timiniel") }
            },
            new BossDbModel(id: 31, location: "Oren", name: "Orfen", chance: 33, respawnTime: 24,
                    nickName: "орфен", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Орфен"), new("Orfen") }
            },
            new BossDbModel(id: 32, location: "Oren", name: "Repiro", chance: 50, respawnTime: 7,
                    nickName: "репиро")
            {
                BossNames = new List<BossNamesDBModel>() { new("Репиро"), new("Repiro") }
            },
            new BossDbModel(id: 33, location: "Oren", name: "Coroon", chance: 100, respawnTime: 12,
                    nickName: "корун", restartRespawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>() { new("Корун"), new("Coroon") }
            },
            new BossDbModel(id: 34, location: "Oren", name: "Samuel", chance: 100, respawnTime: 12,
                    nickName: "самуэль", restartRespawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Самуэль"), new("Samuel") }
            },
            // Aden
            new BossDbModel(id: 35, location: "Aden", name: "Mirror of Oblivion ", chance: 100,
                    respawnTime: 11,
                    nickName: "зеркало", restartRespawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Зеркало"), new("Mirror") }
            },
            new BossDbModel(id: 36, location: "Aden", name: "Hisilrome", chance: 50, respawnTime: 6,
                    nickName: "хисилром", restartRespawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>() { new("Хисилром"), new("Hisilrome") }
            },
            new BossDbModel(id: 37, location: "Aden", name: "Landor", chance: 100, respawnTime: 9,
                    nickName: "ландор", restartRespawnTime: 8)
            {
                BossNames = new List<BossNamesDBModel>() { new("Ландор"), new("Landor") }
            },
            new BossDbModel(id: 38, location: "Aden", name: "Flynt", chance: 33, respawnTime: 5,
                    nickName: "фоллинт", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Фоллинт"), new("Flynt") }
            },
            new BossDbModel(id: 39, location: "Aden", name: "Cabrio", chance: 50, respawnTime: 12,
                    nickName: "кабрио", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Кабрио"), new("Cabrio") }
            },
            new BossDbModel(id: 40, location: "Aden", name: "Andras", chance: 50, respawnTime: 15,
                    nickName: "андрас", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Андрас"), new("Andras") }
            },
            new BossDbModel(id: 41, location: "Aden", name: "Haff", chance: 33, respawnTime: 20,
                    nickName: "хафф", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Хафф"), new("Haff") }
            },
            new BossDbModel(id: 42, location: "Aden", name: "Glaki", chance: 100, respawnTime: 8,
                    nickName: "глаки", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Глаки"), new("Glaki") }
            },
            new BossDbModel(id: 43, location: "Aden", name: "Olkuth", chance: 33, respawnTime: 24,
                    nickName: "олкут", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Олкут"), new("Olkuth") }
            },
            new BossDbModel(id: 44, location: "Aden", name: "Rahha", chance: 33, respawnTime: 33,
                nickName: "рахха", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>()
                    { new("Рахха"), new("Rahha") }
            },
            new BossDbModel(id: 45, location: "Aden", name: "Thanatos", chance: 33, respawnTime: 25,
                    nickName: "танатос", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Танатос"), new("Thanatos") }
            },
            // Heine
            new BossDbModel(id: 46, location: "Heine", name: "Phoenix", chance: 33, respawnTime: 24,
                    nickName: "птиц", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Феникс"), new("Phoenix") }
            },
            new BossDbModel(id: 47, location: "Heine", name: "Naiad", chance: 33, respawnTime: 15,
                    nickName: "ная", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Наяда"), new("Naiad") }
            },
            new BossDbModel(id: 48,location: "Heine", name: "Modeus", chance: 33, respawnTime: 24,
                    nickName: "мудя", restartRespawnTime: 14, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Модеус"), new("Modeus") }
            },
            new BossDbModel(id: 49, location: "Heine", name: "Valak", chance: 33, respawnTime: 20,
                    nickName: "Гавнобоссина", restartRespawnTime: 8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Баллак"), new("Valak") }
            },
            new BossDbModel(id: 50, location: "Heine", name: "Cyrex", nickName: "Курис", chance: 33,
                    respawnTime: 24, restartRespawnTime:8, purpleDrop:true)
            {
                BossNames = new List<BossNamesDBModel>() { new("Сайракс"), new("Cyrex") }
            }
        };
}
