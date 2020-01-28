namespace DFC.App.FindACourse.ViewModels
{
    public class SideBarViewModel
    {
        public FiltersListViewModel CourseStudyTime { get; set; }
        public FiltersListViewModel CourseHours { get; set; }
        public FiltersListViewModel CourseFilter { get; set; }
        public FiltersListViewModel StartDate { get; internal set; }
    }
}
