# TeamProject

Project theme : Online store that sales computer technique

- Description

Project consists of three roles : Customer, Admin and Manager : 

- Customer opportunity : customer can view products, read information about them, add product to cart and confirm order also he can leave some comment to specified product
- Manager opportunity : manager performs managing of orders, change status of order and send letter to customer's e-mail(optional) 
- Admin opportunity : admin has the biggest functional, it is possible to create, edit, delete and filter customers, producers, storages, products also admin can delete all of left comments if it is neccessary

Main parts of project : 

- Main page (here is all avaliable to sale products, user can filter, sort it by specified features also it is possible to show page of product)
- Product page (here is all information about product, his specification, user can add product to his cart or leave comment and evaluate prodcut by stars from one to five, also it is possible to edit owns comments and delete them)
- Authentication (if user is not registered, it is possible fill form and register account, for registered people there is opportunity to log in by e-mail and password, also there is function to reset password by code that sends to customer's e-mail)
- Manager panel (it is functionality for managers, he can to view all orders, change status and end point of order and can send letter to customer's e-mail)
- Admin panel (it is functionality for admins, admin can edit, add or remove customers, producers, products, storages, it is possible to view orders and their owner)
- Setting page (part that helps to change some information (expect of e-mail) for customer, also customer can reset his password)
- Cart page (when customer tap on the button "add to cart" on page of product it stores in cart, there customer can set count of products he wants to buy, choose end point of order and confirm his purchase)

Architecture of solution : 

- DAL (class library project that contains models, repositories and classes relating to the database)
- Store (web project that contains main part of application, in particular : controllers, views, viewmodels, migrations and file with configuration : Startup.cs, and Program file that contains main method, also it contains directory with static files (images, scripts, css files))
- NUnitTestStore - (nunit test project that contains tests for methods in application)