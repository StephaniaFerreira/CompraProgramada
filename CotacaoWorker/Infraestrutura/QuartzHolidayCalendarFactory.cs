using BrazilHolidays.Net;
using Quartz.Impl.Calendar;

namespace CotacaoWorker.Infraestrutura
{
    public class QuartzHolidayCalendarFactory
    {
        public static HolidayCalendar CriarCalendarioUtilBrasileiro()
        {
            var calendario = new HolidayCalendar();

            for (int year = DateTime.Now.Year; year <= DateTime.Now.Year + 5; year++)
            {
                var holidays = Holiday.GetAllByYear(year);

                foreach (var holiday in holidays)
                {
                    calendario.AddExcludedDate(holiday.Date);
                }
            }

            return calendario;
        }
    }
}
