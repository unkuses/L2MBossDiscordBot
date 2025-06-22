using BossBot.DBModel;
using CommonLib.Helpers;

namespace BossBot
{
    public class BossData
    {
        private readonly BossDataSource _bossData = new();
        private readonly EventInfoDataSource _eventInfoData = new();
        private readonly List<int> _eventWasMentionToday = [];
        private readonly DateTimeHelper _dateTimeHelper;

        public BossData(Options options, DateTimeHelper dateTimeHelper)
        {
            _bossData.Database.EnsureCreated();
            _eventInfoData.Database.EnsureCreated();
            _dateTimeHelper = dateTimeHelper;
        }
        
        public string SetUserTimeZone(ulong userId, string timeZone)
        {
            var existUser = _bossData.UserInformationDbModels.FirstOrDefault(u => u.UserId == userId);
            if (existUser != null)
            {
                existUser.UserTimeZone = timeZone;
            }
            else
            {
                _bossData.UserInformationDbModels.Add(new UserInformationDBModel()
                    { UserId = userId, UserTimeZone = timeZone });
            }

            _bossData.SaveChanges();
            return $"Time zone set as {timeZone}";
        }

        public string GetUserTimeZone(ulong userId)
        {
            if (_bossData.UserInformationDbModels.Any(u => u.UserId == userId))
            {
                return _bossData.UserInformationDbModels.First(u => u.UserId == userId).UserTimeZone;
            }

            return string.Empty;
        }

        public List<EventInformationDBModel> GetAllEvents()
        {
            return _eventInfoData.EventInformationDbModels.ToList().Where(e => IsCurrentDay(e.Days) && In5Minutes(e.Time, e.Id))
                .ToList();
        }

        public List<EventInformationDBModel> GetAllEvents(ulong chatId)
        {
            return _eventInfoData.EventInformationDbModels.ToList()
                .Where(e => e.ChatId == chatId)
                .ToList();
        }

        public bool AddEvent(EventInformationDBModel eventInfo)
        {
            if (_eventInfoData.EventInformationDbModels.Any(e => e.EventName == eventInfo.EventName))
            {
                return false; // Event already exists
            }

            _eventInfoData.EventInformationDbModels.Add(eventInfo);
            _eventInfoData.SaveChanges();
            return true;
        }

        public bool RemoveEventById(int eventId)
        {
            var eventInfo = _eventInfoData.EventInformationDbModels.FirstOrDefault(e => e.Id == eventId);
            if (eventInfo != null)
            {
                _eventInfoData.EventInformationDbModels.Remove(eventInfo);
                _eventInfoData.SaveChanges();
                return true;
            }
            return false; // Event not found
        }

        private bool IsCurrentDay(RepeatDays days)
        {
            var today = _dateTimeHelper.CurrentTime.DayOfWeek;
            return days.HasFlag((RepeatDays)(1 << (int)today));
        }

        private bool In5Minutes(DateTime time, int eventId)
        {
            var now = _dateTimeHelper.CurrentTime;
            var nowTime = new TimeSpan(now.Hour, now.Minute, 0);
            var eventTime = new TimeSpan(time.Hour, time.Minute, 0);

            // Calculate the difference in minutes
            var diff = (eventTime - nowTime).TotalMinutes;

            // Check if event is within the next 5 minutes (1 to 5 minutes ahead)
            var isIn5Minutes = diff is > 0 and <= 5;

            // Only allow mention if not already mentioned today
            if (isIn5Minutes)
            {
                if (_eventWasMentionToday.Contains(eventId))
                    return false;

                _eventWasMentionToday.Add(eventId);
                return true;
            }
            else
            {
                // Remove from mentioned list if time has passed or is not in window
                _eventWasMentionToday.Remove(eventId);
                return false;
            }
        }
    }
}