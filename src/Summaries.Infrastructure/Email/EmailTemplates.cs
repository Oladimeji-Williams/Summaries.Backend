namespace Summaries.Infrastructure.Email;

internal static class EmailTemplates
{
    public static string PasswordReset(string resetLink, string logoUrl, DateTimeOffset sentAt)
    {
        var formattedDateTime = sentAt.ToString("dd MMMM yyyy, h:mm tt");

        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>Reset your password</title>
        </head>
        <body style="margin:0; padding:0; background-color:#f1f5f9; font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width:100%; background-color:#f1f5f9;">
            <tr>
              <td align="center" style="padding:32px 16px;">
                <a href="{{resetLink}}" style="display:block; width:100%; max-width:480px; margin:0 auto; color:inherit; text-decoration:none;">
                  <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width:100%; max-width:480px; background-color:#ffffff; border-radius:12px; overflow:hidden;">
                    <tr>
                      <td align="center" style="padding:32px 32px 0 32px;">
                        <img src="{{logoUrl}}" width="48" height="48" alt="Summaries" style="display:block; margin:0 auto; border-radius:10px;" />
                        <div style="margin-top:12px; font-size:18px; line-height:24px; font-weight:700; color:#0f172a;">Summaries</div>
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:24px 32px 8px 32px;">
                        <h1 style="margin:0 0 12px 0; font-size:20px; line-height:28px; font-weight:700; color:#0f172a;">Reset your password</h1>
                        <p style="margin:0; font-size:14px; line-height:22px; color:#475569;">We received a request to reset the password for your Summaries account.</p>
                        <p style="margin:12px 0 0 0; font-size:14px; line-height:22px; color:#475569;">Click anywhere in this email to choose a new password.</p>
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:24px 32px;">
                        <table role="presentation" cellpadding="0" cellspacing="0" border="0" style="margin:0 auto;">
                          <tr>
                            <td align="center" style="border-radius:8px; background-color:#4f46e5;">
                              <span style="display:inline-block; padding:14px 32px; font-size:15px; line-height:20px; font-weight:600; color:#ffffff;">Reset password</span>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:0 32px 24px 32px;">
                        <p style="margin:0; font-size:12px; line-height:18px; color:#94a3b8;">If clicking the email does not work, copy and paste this link into your browser:</p>
                        <p style="margin:6px 0 0 0; font-size:12px; line-height:18px; word-break:break-all;"><span style="color:#4f46e5;">{{resetLink}}</span></p>
                      </td>
                    </tr>
                    <tr>
                      <td style="padding:0 32px;"><div style="border-top:1px solid #e2e8f0; height:1px; line-height:1px;"></div></td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:20px 32px 32px 32px;">
                        <p style="margin:0; font-size:12px; line-height:18px; color:#94a3b8;">If you didn't request a password reset, you can safely ignore this email. Your password will remain unchanged.</p>
                        <p style="margin:16px 0 0 0; font-size:11px; line-height:17px; color:#cbd5e1;">Email sent on {{formattedDateTime}} WAT</p>
                      </td>
                    </tr>
                  </table>
                </a>
                <p style="margin:20px 0 0 0; font-size:12px; line-height:18px; color:#94a3b8; text-align:center;">&copy; {{sentAt.Year}} Summaries</p>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }

    public static string Notification(
        string logoUrl, string title, string message, string? actionUrl, string? actionLabel, DateTimeOffset sentAt)
    {
        var formattedDateTime = sentAt.ToString("dd MMMM yyyy, h:mm tt");

        var buttonBlock = actionUrl is null ? "" : $$"""
            <tr>
              <td align="center" style="padding:24px 32px;">
                <table role="presentation" cellpadding="0" cellspacing="0" border="0" style="margin:0 auto;">
                  <tr>
                    <td align="center" style="border-radius:8px; background-color:#4f46e5;">
                      <a href="{{actionUrl}}" style="display:inline-block; padding:14px 32px; font-size:15px; line-height:20px; font-weight:600; color:#ffffff; text-decoration:none;">{{actionLabel}}</a>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            """;

        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>{{title}}</title>
        </head>
        <body style="margin:0; padding:0; background-color:#f1f5f9; font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width:100%; background-color:#f1f5f9;">
            <tr>
              <td align="center" style="padding:32px 16px;">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width:100%; max-width:480px; background-color:#ffffff; border-radius:12px; overflow:hidden;">
                  <tr>
                    <td align="center" style="padding:32px 32px 0 32px;">
                      <img src="{{logoUrl}}" width="48" height="48" alt="Summaries" style="display:block; margin:0 auto; border-radius:10px;" />
                      <div style="margin-top:12px; font-size:18px; line-height:24px; font-weight:700; color:#0f172a;">Summaries</div>
                    </td>
                  </tr>
                  <tr>
                    <td align="center" style="padding:24px 32px 8px 32px;">
                      <h1 style="margin:0 0 12px 0; font-size:20px; line-height:28px; font-weight:700; color:#0f172a;">{{title}}</h1>
                      <p style="margin:0; font-size:14px; line-height:22px; color:#475569;">{{message}}</p>
                    </td>
                  </tr>
                  {{buttonBlock}}
                  <tr>
                    <td style="padding:0 32px;"><div style="border-top:1px solid #e2e8f0; height:1px; line-height:1px;"></div></td>
                  </tr>
                  <tr>
                    <td align="center" style="padding:20px 32px 32px 32px;">
                      <p style="margin:0; font-size:12px; line-height:18px; color:#94a3b8;">If this wasn't you, please secure your account by changing your password immediately.</p>
                      <p style="margin:16px 0 0 0; font-size:11px; line-height:17px; color:#cbd5e1;">Sent on {{formattedDateTime}} WAT</p>
                    </td>
                  </tr>
                </table>
                <p style="margin:20px 0 0 0; font-size:12px; line-height:18px; color:#94a3b8; text-align:center;">&copy; {{sentAt.Year}} Summaries</p>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }

    public static string EmailConfirmation(string confirmLink, string logoUrl, DateTimeOffset sentAt)
    {
        var formattedDateTime = sentAt.ToString("dd MMMM yyyy, h:mm tt");

        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>Confirm your email</title>
        </head>
        <body style="margin:0; padding:0; background-color:#f1f5f9; font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width:100%; background-color:#f1f5f9;">
            <tr>
              <td align="center" style="padding:32px 16px;">
                <a href="{{confirmLink}}" style="display:block; width:100%; max-width:480px; margin:0 auto; color:inherit; text-decoration:none;">
                  <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="width:100%; max-width:480px; background-color:#ffffff; border-radius:12px; overflow:hidden;">
                    <tr>
                      <td align="center" style="padding:32px 32px 0 32px;">
                        <img src="{{logoUrl}}" width="48" height="48" alt="Summaries" style="display:block; margin:0 auto; border-radius:10px;" />
                        <div style="margin-top:12px; font-size:18px; line-height:24px; font-weight:700; color:#0f172a;">Summaries</div>
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:24px 32px 8px 32px;">
                        <h1 style="margin:0 0 12px 0; font-size:20px; line-height:28px; font-weight:700; color:#0f172a;">Confirm your email</h1>
                        <p style="margin:0; font-size:14px; line-height:22px; color:#475569;">Thanks for signing up for Summaries. Click anywhere in this email to verify your email address.</p>
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:24px 32px;">
                        <table role="presentation" cellpadding="0" cellspacing="0" border="0" style="margin:0 auto;">
                          <tr>
                            <td align="center" style="border-radius:8px; background-color:#4f46e5;">
                              <span style="display:inline-block; padding:14px 32px; font-size:15px; line-height:20px; font-weight:600; color:#ffffff;">Verify email</span>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:0 32px 24px 32px;">
                        <p style="margin:0; font-size:12px; line-height:18px; color:#94a3b8;">If clicking the email does not work, copy and paste this link into your browser:</p>
                        <p style="margin:6px 0 0 0; font-size:12px; line-height:18px; word-break:break-all;"><span style="color:#4f46e5;">{{confirmLink}}</span></p>
                      </td>
                    </tr>
                    <tr>
                      <td style="padding:0 32px;"><div style="border-top:1px solid #e2e8f0; height:1px; line-height:1px;"></div></td>
                    </tr>
                    <tr>
                      <td align="center" style="padding:20px 32px 32px 32px;">
                        <p style="margin:0; font-size:12px; line-height:18px; color:#94a3b8;">If you didn't create a Summaries account, you can safely ignore this email.</p>
                        <p style="margin:16px 0 0 0; font-size:11px; line-height:17px; color:#cbd5e1;">Sent on {{formattedDateTime}} WAT</p>
                      </td>
                    </tr>
                  </table>
                </a>
                <p style="margin:20px 0 0 0; font-size:12px; line-height:18px; color:#94a3b8; text-align:center;">&copy; {{sentAt.Year}} Summaries</p>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }
}