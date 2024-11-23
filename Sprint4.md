### Sprint 4
### Adjusting Entries and Financial Reports Module Features:

| **Feature Number** | **Feature**                                                                 | **User Role**          |
|--------------------|-----------------------------------------------------------------------------|------------------------|
| **4.1.1**          | Can generate, view, save, email, or print trial balance, income statement, balance sheet, and retained earnings statement for a particular date or a date range. | Manager                |
| **4.1.2**          | Can approve or reject adjusting journal entries prepared by accountant. If adjusting journal entry submitted is rejected, manager must enter reason in the comment field. | Manager                |
| **4.1.3**          | Once journal entry is approved, the entry must be reflected in the ledger for the account as well as in the financial statements. | Manager                |
| **4.1.4**          | Must be able to view all adjusting journal entries submitted for approval with pending approval status. | Manager                |
| **4.1.5**          | Must be able to view all approved adjusting journal entries.               | Manager                |
| **4.1.6**          | Must be able to view all rejected adjusting journal entries.               | Manager                |
| **4.1.7**          | Must be able to filter journal entries displayed for pending, approved, and rejected categories by date. | Manager                |
| **4.1.8**          | Must be able to search a journal by account name, amount, or date.         | Manager                |
| **4.1.9**          | View event logs for each account in the chart of accounts.                 | Manager                |
| **4.1.10**         | Must be able to click an account name or account number to go to the ledger of the account. | Manager                |
| **4.1.11**         | From the ledger page, must be able to click the post reference (PR) to go to the journal entry which created the account. | Manager                |
| **4.2.1**          | Can create adjusting journal entries using only accounts found in the chart of accounts. | Accountant             |
| **4.2.2**          | Must be able to attach source documents to each journal entry of type pdf, word, excel, csv, jpg, and png. | Accountant             |
| **4.2.3**          | Can cancel or reset an adjusting journal entry before it is submitted if restarting is desired but once an adjusting journal entry is submitted the accountant cannot delete it. | Accountant             |
| **4.2.4**          | Can prepare and submit journal entries.                                     | Accountant             |
| **4.2.5**          | Can view journal entries created by the manager or other accountants.      | Accountant             |
| **4.2.6**          | Can view the status of all adjusting journal entries submitted for approval with pending, approved, or rejected status. | Accountant             |
| **4.2.7**          | Must be able to filter journal entries displayed for pending, approved, and rejected categories by date. | Accountant             |
| **4.2.8**          | Must be able to search a journal by account name, amount, or date.         | Accountant             |
| **4.2.9**          | View event logs for each account in the chart of accounts.                 | Accountant             |
| **4.2.10**         | Must be able to click an account name or number to go to the ledger of the account. | Accountant             |
| **4.2.11**         | Once journal entry is approved, the entry must be reflected in the ledger for the account as well as in the financial statements. | Accountant             |
| **4.2.12**         | From the ledger page, must be able to click the post reference (PR) to go to the journal entry which created the account. | Accountant             |
| **4.2.13**         | Each transaction must have at least one debit and one credit.              | Accountant             |
| **4.2.14**         | Do not allow submitting of a transaction containing an error.              | Accountant             |
| **4.2.15**         | Must be able to send email to the manager or the administrator from the account page. | Accountant             |
| **4.2.16**         | Total of debits in a journal entry must equal total of credits otherwise an appropriate error message must be displayed, and the user should be able to use the error message to correct the problem. | Accountant             |
| **4.2.17**         | Error messages must be housed in a database table.                         | Accountant             |
| **4.2.18**         | Error messages must be displayed in red color.                             | Accountant             |
| **4.2.19**         | Once root cause of error is corrected, error should go away.               | Accountant             |
| **4.2.20**         | Manager must get notification when adjusting journal entry is submitted for approval. | Accountant             |
| **4.2.21**         | Clicking an account name or number on the chart of accounts page should lead to the ledger page for the account. | Accountant             |
| **4.2.22**         | Each entry in the account ledger must have a clickable post reference column which will lead to the journal entry which created it. | Accountant             |
| **4.2.23**         | The ledger page must show the date of the journal entry, a description column, a debit & a credit, and a balance column. The balance after each transaction and posting must be accurate. | Accountant             |
| **4.2.24**         | The ledger page must have filtering and search features. You need to allow filtering by date or date range, and be able to search by account name or amount. | Accountant             |