using System.Collections.Generic;

namespace Schedule_Constructor.Models
{
    public class ScheduleViewModel
    {
        public int Group { get; set; }
        public List<ScheduleData> ScheduleData { get; set; }
    }

    public class ScheduleData
    {
        public int DayOfWeek { get; set; }
        public int LessonNumber { get; set; }
        public int SubjectId { get; set; }
    }
}