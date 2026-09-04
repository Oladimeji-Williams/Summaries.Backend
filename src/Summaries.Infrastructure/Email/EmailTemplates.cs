namespace Summaries.Infrastructure.Email;

internal static class EmailTemplates
{
    public static string PasswordReset(string resetLink)
    {
        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>Reset your password</title>
        </head>
        <body style="margin:0; padding:0; background-color:#f1f5f9; font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;">
          <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:#f1f5f9; padding:32px 16px;">
            <tr>
              <td align="center">
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:480px; background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 16px rgba(15,23,42,0.08);">

                  <!-- Header / logo -->
                  <tr>
                    <td style="padding:32px 32px 0 32px; text-align:center;">
                      <div style="display:inline-block; width:48px; height:48px; border-radius:12px; background-color:#4f46e5; line-height:48px; font-size:22px; color:#ffffff; font-weight:700;">
                        S
                      </div>
                      <div style="margin-top:12px; font-size:18px; font-weight:700; color:#0f172a;">
                        Summaries
                      </div>
                    </td>
                  </tr>

                  <!-- Body -->
                  <tr>
                    <td style="padding:24px 32px 8px 32px; text-align:center;">
                      <h1 style="margin:0 0 12px 0; font-size:20px; color:#0f172a;">Reset your password</h1>
                      <p style="margin:0; font-size:14px; line-height:22px; color:#475569;">
                        We received a request to reset the password for your Summaries account.
                        Click the button below to choose a new one. This link will expire soon for your security.
                      </p>
                    </td>
                  </tr>

                  <!-- Button -->
                  <tr>
                    <td style="padding:24px 32px; text-align:center;">
                      <table role="presentation" cellpadding="0" cellspacing="0" style="margin:0 auto;">
                        <tr>
                          <td style="border-radius:8px; background-color:#4f46e5;">
                            <a href="{{resetLink}}"
                               style="display:inline-block; padding:14px 32px; font-size:15px; font-weight:600; color:#ffffff; text-decoration:none; border-radius:8px;">
                              Reset password
                            </a>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>

                  <!-- Fallback link -->
                  <tr>
                    <td style="padding:0 32px 24px 32px; text-align:center;">
                      <p style="margin:0; font-size:12px; line-height:18px; color:#94a3b8;">
                        Button not working? Copy and paste this link into your browser:
                      </p>
                      <p style="margin:6px 0 0 0; font-size:12px; word-break:break-all;">
                        <a href="{{resetLink}}" style="color:#4f46e5;">{{resetLink}}</a>
                      </p>
                    </td>
                  </tr>

                  <!-- Divider -->
                  <tr>
                    <td style="padding:0 32px;">
                      <div style="border-top:1px solid #e2e8f0;"></div>
                    </td>
                  </tr>

                  <!-- Footer -->
                  <tr>
                    <td style="padding:20px 32px 32px 32px; text-align:center;">
                      <p style="margin:0; font-size:12px; line-height:18px; color:#94a3b8;">
                        If you didn't request a password reset, you can safely ignore this email —
                        your password will remain unchanged.
                      </p>
                    </td>
                  </tr>

                </table>

                <p style="margin:20px 0 0 0; font-size:12px; color:#94a3b8;">
                  &copy; {{DateTime.UtcNow.Year}} Summaries
                </p>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
    }
}