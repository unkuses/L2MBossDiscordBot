using BossBot.DBModel;

namespace BossBot;

public static class BossCollection
{
    public static List<BossDbModel> GetBossesCollection()
        =>
        [
            new BossDbModel(name: "Basila", russionName: "Базил", chance: 50, location: "Gludio", nickName: "базил", respawnTime: 4),
            new BossDbModel(name: "Chertuba", russionName: "Чертуба",chance: 50, location: "Gludio", nickName: "чертуба", respawnTime: 6),
            new BossDbModel(name: "Kelsus", russionName: "Кельсус",chance: 50,  location: "Gludio", nickName: "кельсус", respawnTime: 10),
            new BossDbModel(name: "Queen Ant", russionName: "Королева Муравьев", chance: 33, location: "Gludio", nickName: "квина", respawnTime: 6, restartRespawnTime:8),
            new BossDbModel(name: "Saban", russionName: "Сабан",chance: 50, location: "Gludio", nickName: "сабянин", respawnTime: 12),
            // Dion
            new BossDbModel(name: "Contaminated Cruma", russionName: "Зараженный Крума",chance: 100, location: "Dion", nickName: "крума",
                respawnTime: 8),
            new BossDbModel(name: "Core Susceptor", russionName: "Сусцептор Ядра",chance: 33, location: "Dion", nickName: "ядро", respawnTime: 10, restartRespawnTime:8),
            new BossDbModel(name: "Enkura", russionName: "Энкура",chance: 50, location: "Dion", nickName: "энкура", respawnTime: 6),
            new BossDbModel(name: "Felis", russionName: "Фелис",chance: 100, location: "Dion", nickName: "фелис", respawnTime: 3),
            new BossDbModel(name: "Katan", russionName: "Катан",chance: 100, location: "Dion", nickName: "катан", respawnTime: 10),
            new BossDbModel(name: "Mutated Cruma", russionName: "Мутант Крума",chance: 100, location: "Dion", nickName: "мутант", respawnTime: 8, restartRespawnTime:8),
            new BossDbModel(name: "Pan Draaed", russionName: "Пан Драйд",chance: 100, location: "Dion", nickName: "драйд", respawnTime: 12),
            new BossDbModel(name: "Sarka", russionName: "Шарка",chance: 100, location: "Dion", nickName: "шарка", respawnTime: 10),
            new BossDbModel(name: "Stonegeist", russionName: "Каменук",chance: 100, location: "Dion", nickName: "каменук", respawnTime: 7),
            new BossDbModel(name: "Talakin", russionName: "Талакин",chance: 100, location: "Dion", nickName: "талакин", respawnTime: 10),
            new BossDbModel(name: "Timitris", russionName: "Тимитрис",chance: 100, location: "Dion", nickName: "тимитрис", respawnTime: 8),
            new BossDbModel(name: "Valefar", russionName: "Буря",chance: 100, location: "Dion", nickName: "буря", respawnTime: 6),
            // Giran 
            new BossDbModel(location: "Giran", russionName: "Чудовище",respawnTime: 9, name: "Behemoth", chance: 100, nickName: "чудовище"),
            new BossDbModel(location: "Giran", russionName: "Черная Лилия",respawnTime: 12, name: "Black Lily", chance: 100, nickName: "лилия", restartRespawnTime:8),
            new BossDbModel(location: "Giran", russionName: "Брека",respawnTime: 6, name: "Breka", chance: 50, nickName: "брека"),
            new BossDbModel(location: "Giran", russionName: "Матура",respawnTime: 6, name: "Matura", chance: 50, nickName: "матура"),
            new BossDbModel(location: "Giran", russionName: "Медуза",respawnTime: 10, name: "Medusa", chance: 100, nickName: "медуза"),
            new BossDbModel(location: "Giran", russionName: "Пан Марод",respawnTime: 5, name: "Pan Narod", chance: 50, nickName: "марод"),
            new BossDbModel(location: "Giran", russionName: "Чудовищный Дракон",respawnTime: 12, name: "Dragon Beast", chance: 33, nickName: "дракон", restartRespawnTime:14),
            // Oren
            new BossDbModel(location: "Oren", russionName: "Фаробос",name: "Tromba", chance: 50, respawnTime: 7, nickName: "фаробос"),
            new BossDbModel(location: "Oren", russionName: "Гарет",name: "Gahareth", chance: 50, respawnTime: 9, nickName: "гарет"),
            new BossDbModel(location: "Oren", russionName: "Талкин",name: "Talkin", chance: 33, respawnTime: 8, nickName: "талкин"),
            new BossDbModel(location: "Oren", russionName: "Селу",name: "Selu", chance: 33, respawnTime: 12, nickName: "селу"),
            new BossDbModel(location: "Oren", russionName: "Бальбо",name: "Balbo", chance: 50, respawnTime: 12, nickName: "бальбо"),
            new BossDbModel(location: "Oren", russionName: "Тиминиэль",name: "Timiniel", chance: 100, respawnTime: 8, nickName: "тиминиэль"),
            new BossDbModel(location: "Oren", russionName: "Орфен",name: "Orfen", chance: 33, respawnTime: 24, nickName: "орфен", restartRespawnTime:14),
            new BossDbModel(location: "Oren", russionName: "Репиро",name: "Repiro", chance: 50, respawnTime: 7, nickName: "репиро"),
            new BossDbModel(location: "Oren", russionName: "Корун",name: "Coroon", chance: 100, respawnTime: 12, nickName: "корун",restartRespawnTime:8),
            new BossDbModel(location: "Oren", russionName: "Самуэль",name: "Samuel", chance: 100, respawnTime: 12, nickName: "самуэль", restartRespawnTime:8),
            // Aden
            new BossDbModel(location: "Aden", russionName: "Зеркало Забвения",name: "Mirror of Oblivion ", chance: 100, respawnTime: 11,
                nickName: "зеркало", restartRespawnTime:8),
            new BossDbModel(location: "Aden", russionName: "Хисилром",name: "Hisilrome", chance: 50, respawnTime: 6, nickName: "хисилром", restartRespawnTime:8),
            new BossDbModel(location: "Aden", russionName: "Ландор",name: "Landor", chance: 100, respawnTime: 9, nickName: "ландор", restartRespawnTime:8),
            new BossDbModel(location: "Aden", russionName: "Фоллинт",name: "Flynt", chance: 33, respawnTime: 5, nickName: "фоллинт", restartRespawnTime:14),
            new BossDbModel(location: "Aden", russionName: "Кабрио",name: "Cabrio", chance: 50, respawnTime: 12, nickName: "кабрио", restartRespawnTime:14),
            new BossDbModel(location: "Aden", russionName: "Андрас",name: "Andras", chance: 50, respawnTime: 15, nickName: "андрас", restartRespawnTime:14),
            new BossDbModel(location: "Aden", russionName: "Хафф",name: "Haff", chance: 33, respawnTime: 20, nickName: "хафф", restartRespawnTime:14),
            new BossDbModel(location: "Aden", russionName: "Глаки",name: "Glaki", chance: 100, respawnTime: 8, nickName: "глаки", restartRespawnTime:14),
            new BossDbModel(location: "Aden", russionName: "Олкут",name: "Olkuth", chance: 33, respawnTime: 24, nickName: "олкут", restartRespawnTime:14),
            new BossDbModel(location: "Aden", russionName: "Рахха",name: "Rahha", chance: 33, respawnTime: 33, nickName: "рахха", restartRespawnTime:14),
            new BossDbModel(location: "Aden", russionName: "Танатос",name: "Thanatos", chance: 33, respawnTime: 25, nickName: "танатос", restartRespawnTime:14),
            //Heine
            new BossDbModel(location: "Heine", russionName: "Феникс",name: "Phoenix", chance: 66, respawnTime: 24, nickName: "птиц", restartRespawnTime:14),
            new BossDbModel(location: "Heine", russionName: "Наяда",name: "Naiad", chance: 33, respawnTime: 15, nickName: "ная", restartRespawnTime:14),
            new BossDbModel(location: "Heine", russionName: "Модеус",name: "Modeus", chance: 33, respawnTime: 24, nickName: "мудя", restartRespawnTime:14),
            new BossDbModel(location: "Heine", russionName: "Баллак",name: "Valak", chance: 33, respawnTime: 20, nickName:"Гавнобоссина", restartRespawnTime: 8),
            // new BossDbModel(location:"Heine", russionName:"Куриса", name:"Cyrex", nickName:"Cайракс", chance:33, respawnTime:25)
        ];
}