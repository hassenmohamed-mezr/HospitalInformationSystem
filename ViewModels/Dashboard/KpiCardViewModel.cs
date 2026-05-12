namespace HospitalInformationSystem.ViewModels.Dashboard
{
    public class KpiCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string IconClass { get; set; } = "bi-circle";
        /// <summary>Bootstrap semantic: primary, success, warning, danger, info.</summary>
        public string Accent { get; set; } = "primary";
        public string? TrendText { get; set; }
        public string TrendCss { get; set; } = "text-muted";
    }
}
