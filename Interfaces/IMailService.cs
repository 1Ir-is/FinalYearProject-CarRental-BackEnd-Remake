﻿namespace CarRental_BE.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
