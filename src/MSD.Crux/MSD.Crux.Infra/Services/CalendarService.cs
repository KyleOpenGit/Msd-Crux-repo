using System;
using System.Collections.Generic;
using System.Globalization;
using MSD.Crux.Common;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services;

/// <summary>
/// 주차번호와 관련된 캘린더 서비스 인터페이스 구현체
/// </summary>
public class CalendarService : ICalendarService
{
    public DateListsOfWeekNumbersRspDto GetDateLists(int weekNumber, int count)
    {
        if (weekNumber < 1 || weekNumber > 53)
        {
            throw new ArgumentException("주차 번호는 1에서 53 사이여야 합니다.");
        }

        if (count <= 0)
        {
            throw new ArgumentException("조회할 주차 수는 1 이상이어야 합니다.");
        }

        int currentYear = DateTime.Now.Year;
        DateListsOfWeekNumbersRspDto result = new() { DatesOfWeekNumberList = new List<DatesOfWeekNumber>() };

        for (int i = 0; i < count; i++)
        {
            int targetWeekNumber = weekNumber + i;
            int targetYear = currentYear;

            while (targetWeekNumber > 53)
            {
                targetWeekNumber -= 53;
                targetYear++;
            }

            DateTime firstDayOfWeek = ISOWeek.ToDateTime(targetYear, targetWeekNumber, DayOfWeek.Monday);
            DatesOfWeekNumber weekData = WCalendar.GetDatesOfISOWeek(firstDayOfWeek);
            result.DatesOfWeekNumberList.Add(weekData);
        }

        return result;
    }
}
