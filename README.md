# Identified Vulnerabilities


## SQL Injection Risk in Login and Registration

Test cases like:
SQLadmin' OR '1'='1Robert'); DROP TABLE Users;--Show more lines
indicated potential injection attempts.
Root cause: Lack of input validation (though parameterized queries were used, malicious input could still be processed).

## Weak Password Verification

Risk that PasswordHasher.VerifyPassword could incorrectly validate arbitrary input if not securely implemented.

## JWT Handling

Token returned in response header but not properly stored or validated on the client side.
Potential exposure if token is mishandled.

# Fixes Applied


## SQL Injection Mitigation

Continued use of parameterized queries with MySqlCommand.
Added input validation for usernames and emails (e.g., regex to allow only alphanumeric characters).
Unit tests to ensure injection attempts fail:
C# Assert.False(_repo.VerifyLogin("admin' OR '1'='1", "password123"));Assert.DoesNotContain(users, u => u.Username == injectedUsername);

## Password Security

Verified secure hashing (PBKDF2/bcrypt recommended).
Ensured VerifyPassword compares hashed values correctly.

## JWT Best Practices

Returned token as JSON for easier handling:
C#return new JsonResult(new { token });Show more lines

Stored token in sessionStorage on the client:
JavaScriptsessionStorage.setItem("jwt", data.token);Show more lines

Attached token to all API requests via Authorization header.


# How Copilot Assisted

## Debugging & Guidance

Explained why SQL injection tests passed and identified logical flaws.
Suggested secure coding practices (parameterized queries, regex validation).
Provided complete examples for:

## Unit tests against SQL injection.
Secure password verification.
JWT generation and client-side storage.
Git setup and SSH troubleshooting.


## Step-by-Step Fixes

Helped configure .gitignore, GitHub SSH setup, and push workflow.
Offered ready-to-use snippets for Fetch/Axios with JWT headers.
Delivered best practices for Razor Pages and WebAPI integration.

✅ Result: Application now uses secure authentication, prevents SQL injection, and properly handles JWT tokens for protected routes.