# PROG6212-Part-3


Youtube link -https://youtu.be/buAFxQ5s260

Changes made from part 2 to 3 
- Created the Auto calculation and validation for the lecturer submit claim 
- Created the automatic validation for the Programme manager and Co Ordinator 
- Created HR that is able to generate reports as well as being able to create,delete and edit users. They can also see the submitted claims 

Lectuer Feedback 
Criterion Feedback
add descriptive messages fro your commits as well to guide the reader
- I did this by ensuring my commit messages were descriptive.

Login Details 

HR 
Username- Jenny
Password- J12345
Username -hradmin
password -HR@2025


Lecturer
Username - Tim 
Password - T12345



Programme Co Ordinator 
Username -  Ben 
Password - B12345


Academic Manager 
Username - Simon 
Password - S12345


Lecturer Claim Application 

This is a ASP>NET core MVC application that is used to manage the claims of lecturers 

The purpose and why the appliction was designed was for Independent contracts(Lectures) to be able to submit their claims and 
get paid for the hours they worked once their claim had been reviewed by the Programme co-ordinator and the Academic Co ordinator.
Once it has been approved the the HR pays the IC for the work they have completed. 

1. Main Idea of Claim application

The lecturer is able to submit a claim. 
The follwoing information is stored: 
-Hours 
- any notes 
- The amount is auto calculated for the amount the IC needs to be payed. 
- Supporting documentation 
-The name and hourly rate are pulled from when the lectuer is registered. 

- The programme co ordinator needs to approve or reject the claim 
- It then moves on to the Academic manager for them to approve to reject the claim, once it is approve it can then be payed by Hr 
- The Hr is able to register uses but also see alll the claims that have been submitted and generate reports to see what has been 
payed 

2 . Roles and what each person is able to do 

Lecturer 
They are able to login with the credentials they are given from Hr 
They are able to submit their claims as well as see the claims they have submitted

Programme Co ordinator
They are able to log in with the credentials given by the HR 
They are able to see the claims and approve or reject them. 

Academic Manager 
They are able to log in with the credentials given by the HR 
They are able to view the claims that need to be approved or rejected 

HR
They HR is able to register as well as edit and delete diffrent users 
They are able to View the claims that need to be payed for 
They can generate diffrent reports and invoices for the claims submitted 
The Hr is able to print the homescreen of the claims 

3. Different technology that was used
Asp.NET Core MVC 
Entity Framework core 
Sql Server 
BootStrap for the styling

4. How to run the application 
Clone the repository 
git clone https://github.com/AnderssenJustin/PROG6212-Part-3.git 
cd Prog6212 Part 3
