# Manitoulin Health Centre Hospital Redesign Project

## Summary
The Hospital Project is a team Capstone project, for the courses HTTP5206 Web Information Architecture and HTTP5204 Mobile Development.
It is a group project that requires us to create a Content Management System which reflects an existing hospital. 

## Group Members
Below are detailed summaries of what each group member has completed, what they are currently working on and what will be completed next.

### Neil's Feature Summary- FAQ
FAQ feature consists of 2 entities with full CRUD functionality with many to many relationship
- [x] FAQ -the admin can list, create, update,delete and also list a particular faq from a category
- [x] FAQ Categories - The public user can view the list of Faq category and can view all the faqs listed under that category.The admin user has full CRUD acces.

Since the hospital has a lot of information and links displayed on their webpage which the user has to navigate, there will be a lot of common frequently asked questions which the user would like to get access of at one place rather than reading through and navigating the entire website. In order for the user to access the right information, the Faqs are are nested inside their respective categories inorderfor the user to find the information they need based on the category.  

### Yifat's Feature Summary
Feedback feature will allow website users and patients to leave their feedback on any issue possible. users will be able to categorize their feedbacks from a provided drop down list such as: Compliment, Concern, Suggestion, Question and Other.
- [x] Feedback -the admin can create, read, update and delete feedbacks and public user can create new feedback through a form.
- [x] Feedback Categories - the admin can create, read, update and delete feedback Categories and public user can read (choose a category) on the feedback form.

### Vedanshi's Feature Summary
#### Included
* CRUD
* Authentication
* Responsive Design

#### Appointment Feature
Patients can book an appointment with available doctors and Doctors can handle booked appointments whereas Admin can perform full CRUD operation 
#### Department Feature
Patients and Doctors can see list of departments as well available jobs with that particular department of the hospital and Admin has full CRUD operation permission. 



### Ruth's Feature Summary
The Volunter Listings feature displays all available volunteer listings in the hospital and allows admin users to perform full CRUD operations. Authentication is implemented and only admin users can perform full CRUD operations. For users, the listings are read-only. In the future, conditional rendering will be implemented and Application Users more robust.
- Listing entity has a M-M relationship with Application Users (Entity Framework)
- Listing entity has a 1-M relationship with Department
- Listings table is utilizing a foreign key from Departments table
- Listing: admin users can list, create, update and delete Volunteer Listings
- Users: information about the users are displayed in associated listings and can be unassociated with a particular listing

### Justin's Feature Summary
The patient e-card feature allows users to send virtual greeting cards to patients at the hospital. It consists of a 1-M relationship between the Template Entity and the Ecard Entity. The user can create a new e-card by selecting a template and filling in further details (senders name, patients name and message). 
- [x] Template - The admin can create, update and delete a template. While a user can view a list of templates and choose a template when creating a new e-card. 
- [x] Ecard -  A user can create a new e-card while an admin can update, view and delete an e-card. 


### Natasha's Feature Summary
Pay an invoice feature, allows users to pay an invoice online. There is a 1-M relationship between the Invoice Entity and the Payment Entity. I will be using stripe to hash the credit/debit card information of each user.
- [x] Invoice - Users can add an invoice, Admin can view a list of users' invoices,edit (if requested by user), and delete invoices
- [x] Payment - Users can add a payment, view their payments, edit their payments. Admin can view a list of users' payments, edit and delete (if requested by user), 
