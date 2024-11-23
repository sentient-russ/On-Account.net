### Sprint 2
### Chart of Accounts Module Features:

| **Feature Number** | **Feature**                                                                 | **User Role**          |
|--------------------|-----------------------------------------------------------------------------|------------------------|
| **2.1.1**          | Add, View, Edit, or Deactivate accounts.                                     | Administrator          |
| **2.1.2**          | Select service before displaying the appropriate user interface.            | Administrator          |
| **2.1.3**          | Store required information in the database when an account is added.        | Administrator          |
| **2.1.3.a**        | - Account name                                                              | Administrator          |
| **2.1.3.b**        | - Account number (correct starting values)                                  | Administrator          |
| **2.1.3.c**        | - Account description                                                       | Administrator          |
| **2.1.3.d**        | - Normal side                                                               | Administrator          |
| **2.1.3.e**        | - Account category (e.g., asset)                                            | Administrator          |
| **2.1.3.f**        | - Account subcategory (e.g., current assets)                                | Administrator          |
| **2.1.3.g**        | - Initial balance                                                           | Administrator          |
| **2.1.3.h**        | - Debit                                                                     | Administrator          |
| **2.1.3.i**        | - Credit                                                                    | Administrator          |
| **2.1.3.j**        | - Balance                                                                   | Administrator          |
| **2.1.3.k**        | - Date/time account added                                                   | Administrator          |
| **2.1.3.l**        | - User ID                                                                   | Administrator          |
| **2.1.3.m**        | - Order (e.g., cash can be 01)                                              | Administrator          |
| **2.1.3.n**        | - Statement (e.g., IS (income statement), BS (balance sheet), RE (Retained Earnings statement)) | Administrator          |
| **2.1.3.o**        | - Comment                                                                   | Administrator          |
| **2.1.4**          | Duplicate account numbers or names should not be allowed.                   | Administrator          |
| **2.1.5**          | All monetary values should have two decimal spaces.                         | All                    |
| **2.1.6**          | All monetary values must be formatted using commas when appropriate.        | All                    |
| **2.1.7**          | Account numbers should not allow decimal spaces or alphanumeric values.     | All                    |
| **2.1.8**          | Accounts with balance greater than zero cannot be deactivated.              | Administrator          |
| **2.1.9**          | View either individual accounts and their details or a report of all accounts found in the chart of accounts. | All                    |
| **2.1.10**         | Search using either account number or account name to locate an account in the chart of accounts. | All                    |
| **2.1.11**         | The name of the logged user must be shown on the top left corner of the page. | All                    |
| **2.1.12**         | The logo of the software must display on each page.                         | All                    |
| **2.1.13**         | Clicking each account in the chart of accounts should take you to the ledger of each account. | All                    |
| **2.1.14**         | Filter the data in the chart of accounts page using various tokens such as by account name, number, category, subcategory, amount, etc. | All                    |
| **2.1.15**         | A pop-up calendar should display at the top left corner of the page.        | All                    |
| **2.1.16**         | Buttons to other services provided in the software such as journalizing must be found at the top of each page. | All                    |
| **2.1.17**         | An event log showing the before and after image of each record added, modified, or deactivated should be generated each time data changes by any of the users. | Administrator          |
| **2.1.18**         | The event logs must be kept on a table.                                     | Administrator          |
| **2.1.19**         | The user ID and the time and date of the user who made changes to the data must be saved. | Administrator          |
| **2.1.20**         | Each event must have a unique auto-generated ID.                            | Administrator          |
| **2.1.21**         | Each of the pages in the application must have a consistent color and layout scheme. | All                    |
| **2.1.22**         | Each button must have a built-in tool-tip providing information about the purpose of the control. | All                    |
| **2.1.23**         | Each page must have a help button with information about the entire software organized by topic. | All                    |
| **2.2.1**          | Can view accounts but can’t add, edit, or deactivate accounts.              | Manager                |
| **2.2.2**          | Can perform the rest of the services the administrator can perform.         | Manager                |
| **2.3.1**          | Can view accounts but can’t add, edit, or deactivate accounts.              | Accountant             |
| **2.3.2**          | Can perform the rest of the services the administrator can perform.         | Accountant             |