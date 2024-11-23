### Sprint 3
### Journalizing & Ledger Module Features:

| **Feature Number** | **Feature**                                                                 | **User Role**          |
|--------------------|-----------------------------------------------------------------------------|------------------------|
| **3.1.1**          | Add, View, Edit, or Deactivate accounts (implemented in Sprint 2).          | Administrator          |
| **3.1.2**          | View event logs for each account in the chart of accounts. Event logs must show the before and after image of each account. If an account is added for the first time, there will be no before image. If an account name is modified, there must be a before and after account image including the user id of the person who made the change and the time and date of the change. | Administrator          |
| **3.1.3**          | Be able to send email to manager or accountant user from the chart of accounts page. | Administrator          |
| **3.2.1**          | Can create journal entries using only accounts found in the chart of accounts. | Manager                |
| **3.2.2**          | Can approve or reject journal entries prepared by accountant. If journal entry submitted is rejected, manager must enter reason in the comment field. | Manager                |
| **3.2.3**          | Once journal entry is approved, the entry must be reflected in the ledger for the account. | Manager                |
| **3.2.4**          | Must be able to view all journal entries submitted for approval with pending approval status. | Manager                |
| **3.2.5**          | Must be able to view all approved journal entries.                         | Manager                |
| **3.2.6**          | Must be able to view all rejected journal entries.                         | Manager                |
| **3.2.7**          | Must be able to filter journal entries displayed for pending, approved, and rejected categories by date. | Manager                |
| **3.2.8**          | Must be able to search a journal by account name, amount, or date.         | Manager                |
| **3.2.9**          | View event logs for each account in the chart of accounts. Event logs must show the before and after image of each account. If an account is added for the first time, there will be no before image. If an account name is modified, there must be a before and after account image including the user id of the person who made the change and the time and date of the change. | Manager                |
| **3.2.10**         | Must be able to click an account name to go to the ledger of the account.  | Manager                |
| **3.2.11**         | From the ledger page, must be able to click the post reference (PR) to go to the journal entry which created the account. | Manager                |
| **3.2.12**         | Clicking an account on the chart of accounts page should lead to the ledger page for the account where all entries can be viewed. | Manager                |
| **3.2.13**         | Each entry in the account ledger must have a clickable post reference column which will lead to the journal entry which created it. | Manager                |
| **3.2.14**         | The ledger page must show the date of the journal entry, a description column which is usually empty, a debit, a credit column, and a balance column. The balance after each transaction and posting must be accurate. | Manager                |
| **3.2.15**         | The ledger page must have filtering and search features. You need to allow filtering by date or date range, and be able to search by account name or amount. | Manager                |
| **3.3.1**          | Can create journal entries using only accounts found in the chart of accounts. | Accountant             |
| **3.3.2**          | Debits come before credits in each journal entry created.                  | Accountant             |
| **3.3.3**          | Multiple debits and multiple credits must be possible for each journal entry recorded. | Accountant             |
| **3.3.4**          | Must be able to attach source documents to each journal entry of type pdf, word, excel, csv, jpg, and png. | Accountant             |
| **3.3.5**          | Can cancel or reset a journal entry before it is submitted if restarting is desired but once a journal entry is submitted the accountant cannot delete it. | Accountant             |
| **3.3.6**          | Can prepare and submit journal entries.                                     | Accountant             |
| **3.3.7**          | Can view journal entries created by the manager or other accountants.      | Accountant             |
| **3.3.8**          | Can view the status of all journal entries submitted for approval with pending, approved, or rejected status. | Accountant             |
| **3.3.9**          | Must be able to filter journal entries displayed for pending, approved, and rejected categories by date. | Accountant             |
| **3.3.10**         | Must be able to search a journal by account name, amount, or date.         | Accountant             |
| **3.3.11**         | View event logs for each account in the chart of accounts. Event logs must show the before and after image of each account. If an account is added for the first time, there will be no before image. If an account name is modified, there must be a before and after account image including the user id of the person who made the change and the time and date of the change. | Accountant             |
| **3.3.12**         | Must be able to click an account name to go to the ledger of the account.  | Accountant             |
| **3.3.13**         | Once journal entry is approved, the entry must be reflected in the ledger for the account. | Accountant             |
| **3.3.14**         | From the ledger page, must be able to click the post reference (PR) to go to the journal entry which created the account. | Accountant             |
| **3.3.15**         | Each transaction must have at least one debit and one credit.              | Accountant             |
| **3.3.16**         | Do not allow submitting of a transaction containing an error.              | Accountant             |
| **3.3.17**         | Must be able to send email to the manager or the administrator from the account page. | Accountant             |
| **3.3.18**         | Total of debits in a journal entry must equal total of credits otherwise an appropriate error message must be displayed, and the user should be able to use the error message to correct the problem. Think of all the errors that can happen and come up with appropriate error messages. | Accountant             |
| **3.3.19**         | Error messages must be housed in a database table.                         | Accountant             |
| **3.3.20**         | Error messages must be displayed in red color.                             | Accountant             |
| **3.3.21**         | Once root cause of error is corrected, error should go away.               | Accountant             |
| **3.3.22**         | Manager must get notification when journal entry is submitted for approval. | Accountant             |
| **3.3.23**         | Clicking an account on the chart of accounts page should lead to the ledger page for the account where all entries can be viewed. | Accountant             |
| **3.3.24**         | Each entry in the account ledger must have a clickable post reference column which will lead to the journal entry which created it. | Accountant             |
| **3.3.25**         | The ledger page must show the date of the journal entry, a description column which is usually empty, a debit, a credit column, and a balance column. The balance after each transaction and posting must be accurate. | Accountant             |
| **3.3.26**         | The ledger page must have filtering and search features. You need to allow filtering by date or date range, and be able to search by account name or amount. | Accountant             |