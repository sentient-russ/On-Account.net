### Sprint 1
### User Interface Module Features:

| **Feature**                                                                 | **User Role**          |
|-----------------------------------------------------------------------------|------------------------|
| **User Authentication:**                                                    |                        |
| - Supports three types of users: administrator, manager, and regular user (accountant). | All                    |
| - Each user can log in to the system once credentials are created.          | All                    |
| - Displays login username and picture on the top right corner of the login page after successful login. | All                    |
| **Login Page:**                                                             |                        |
| - Includes a text box for entering the username.                            | All                    |
| - Includes a text box for entering a password, which is hidden as the user types. | All                    |
| - Includes a submit button for login.                                       | All                    |
| - Includes a "Forgot Password" button.                                      | All                    |
| - Includes a "Create New User" button.                                      | All                    |
| - Displays a logo on all pages of the application.                          | All                    |
| **User Management (Administrator Privileges):**                             |                        |
| - Can create users and assign roles.                                        | Administrator          |
| - Can update information about system users.                                | Administrator          |
| - Can activate or deactivate each type of user.                             | Administrator          |
| - Can view a report of all users in the system.                             | Administrator          |
| - Can suspend any user from a start date to an expiry date.                 | Administrator          |
| - Can send emails to any user from within the system.                       | Administrator          |
| **Password Management:**                                                    |                        |
| - Passwords must be a minimum of 8 characters, start with a letter, and include a letter, a number, and a special character. | All                    |
| - Passwords used in the past cannot be reused.                              | All                    |
| - Passwords must be encrypted.                                              | All                    |
| - Allows a maximum of three wrong password attempts before suspending the user. | All                    |
| - Notifies the user three days before the password expires.                 | All                    |
| - Provides a "Forgot Password" feature that prompts the user to enter their email address, user ID, and answer security questions to reset the password. | All                    |
| **User Creation:**                                                          |                        |
| - The "Create New User" button allows first-time users to request access by providing personal information (first name, last name, address, DOB). | All                    |
| - The administrator receives an email request to approve or reject the new user request. | Administrator          |
| - If approved, the user receives an email with a link to log in to the system. | Administrator          |
| **Username Format:**                                                        |                        |
| - Usernames are generated as the first name initial, full last name, and a four-digit code (two-digit month and two-digit year) of account creation. | All                    |
| **Reporting:**                                                              |                        |
| - The administrator can view a report of all expired passwords.             | Administrator          |
| - All login information is stored in database tables.                       | All                    |