namespace API.Core.Dtos
{
    // Email Notification DTO
    public class EmailNotificationDto
    {
        public Guid NotificationGuid { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
        public string? RecipientName { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string? FailureReason { get; set; }
        public int RetryCount { get; set; }
        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Create Email Notification DTO
    public class CreateEmailNotificationDto
    {
        public string RecipientEmail { get; set; } = string.Empty;
        public string? RecipientName { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Priority { get; set; } = "Normal";
        public DateTime? ScheduledAt { get; set; }
        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
    }

    // Email Template DTO
    public class EmailTemplateDto
    {
        public Guid TemplateGuid { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string BodyTemplate { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // Save Email Template DTO
    public class SaveEmailTemplateDto
    {
        public string TemplateName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string BodyTemplate { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // Email Statistics DTO
    public class EmailStatisticsDto
    {
        public EmailStatsOverview Overview { get; set; } = new();
        public List<EmailStatsByType> ByType { get; set; } = new();
        public List<EmailStatsByDay> DailyTrend { get; set; } = new();
    }

    public class EmailStatsOverview
    {
        public int TotalNotifications { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public int PendingCount { get; set; }
        public decimal SuccessRate { get; set; }
        public decimal AvgDeliveryTimeSeconds { get; set; }
    }

    public class EmailStatsByType
    {
        public string NotificationType { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public int PendingCount { get; set; }
    }

    public class EmailStatsByDay
    {
        public DateTime Date { get; set; }
        public int TotalCount { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
    }

    // Email Notification List Response
    public class EmailNotificationListResponse
    {
        public List<EmailNotificationDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    // Send Test Email DTO
    public class SendTestEmailDto
    {
        public string RecipientEmail { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public Dictionary<string, string> TemplateData { get; set; } = new();
    }
}