using System.Net.Mail;
using Knygynas.Models;

namespace Knygynas.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendOrderConfirmationEmail(string recipientEmail, Order order)
    {
        try
        {
            using var client = new SmtpClient("localhost", 1025);
            // Mailpit has accepting_any and allow_insecure enabled by default
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@bookstore.com", "Bookstore HQ"),
                Subject = $"Order Confirmation - Order #{order.Id}",
                Body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #2b7a78;'>Thank you for your order!</h2>
                        <p>Hi, your order <strong>#{order.Id}</strong> has been successfully received and is currently being processed.</p>
                        
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                        
                        <h3>Order Summary</h3>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <thead>
                                <tr style='border-bottom: 2px solid #eee;'>
                                    <th style='text-align: left; padding: 8px;'>Item</th>
                                    <th style='text-align: center; padding: 8px;'>Qty</th>
                                    <th style='text-align: right; padding: 8px;'>Price</th>
                                </tr>
                            </thead>
                            <tbody>
                                {string.Join("", order.OrderItems.Select(oi => $@"
                                    <tr style='border-bottom: 1px solid #eee;'>
                                        <td style='padding: 8px;'>{oi.Book?.Title ?? oi.BookISBN}</td>
                                        <td style='padding: 8px; text-align: center;'>{oi.Quantity}</td>
                                        <td style='padding: 8px; text-align: right;'>${(oi.Price * oi.Quantity):F2}</td>
                                    </tr>
                                "))}
                            </tbody>
                        </table>
                        
                        <div style='text-align: right; margin-top: 20px;'>
                            <strong style='font-size: 1.2rem; color: #2b7a78;'>Total Paid: ${order.TotalPrice:F2}</strong>
                        </div>

                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>

                        <h3>Delivery Information</h3>
                        <p><strong>Delivery Method:</strong> {order.DeliveryMethod}</p>
                        <p><strong>From Warehouse:</strong> {order.FromCity}, {order.FromAddress}</p>
                        <p><strong>Shipping to:</strong> {order.ToCity}, {order.ToAddress}</p>
                        <p><strong>Current Status:</strong> <span style='background-color: #ffe066; padding: 4px 8px; border-radius: 4px; font-weight: bold;'>{order.State}</span></p>

                        <div style='margin-top: 30px; text-align: center;'>
                            <p style='color: #888; font-size: 0.9rem;'>You can track your order live on our bookstore portal.</p>
                        </div>
                    </div>
                ",
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(recipientEmail);
            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Successfully sent order confirmation email to {Email} for Order #{OrderId}", recipientEmail, order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order confirmation email to {Email} for Order #{OrderId}", recipientEmail, order.Id);
        }
    }

    public async Task SendOrderStatusUpdateEmail(string recipientEmail, Order order)
    {
        try
        {
            using var client = new SmtpClient("localhost", 1025);
            var stateColor = order.State switch
            {
                OrderState.Processing => "#ffe066",
                OrderState.Prepared => "#17a2b8",
                OrderState.Shipped => "#007bff",
                OrderState.Finished => "#28a745",
                _ => "#6c757d"
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@bookstore.com", "Bookstore HQ"),
                Subject = $"Order Status Update: {order.State} - Order #{order.Id}",
                Body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                        <h2 style='color: #2b7a78;'>Order Status Update!</h2>
                        <p>Hi, your order <strong>#{order.Id}</strong> status has been updated to <span style='background-color: {stateColor}; color: white; padding: 4px 8px; border-radius: 4px; font-weight: bold;'>{order.State}</span>.</p>
                        
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                        
                        <h3>Delivery Details</h3>
                        <p><strong>Shipping to:</strong> {order.ToCity}, {order.ToAddress}</p>
                        <p><strong>Delivery Method:</strong> {order.DeliveryMethod}</p>
                        <p><strong>Contact Phone:</strong> {order.PhoneNumber}</p>
                        
                        <div style='margin-top: 30px; text-align: center;'>
                            <p style='color: #888; font-size: 0.9rem;'>You can track your order live on our bookstore portal.</p>
                        </div>
                    </div>
                ",
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(recipientEmail);
            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Successfully sent order status update email to {Email} for Order #{OrderId} (New State: {State})", recipientEmail, order.Id, order.State);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order status update email to {Email} for Order #{OrderId}", recipientEmail, order.Id);
        }
    }
}
