![Beautiful Library](https://upload.wikimedia.org/wikipedia/commons/thumb/2/21/Biblioth%C3%A8que_de_l%27Assembl%C3%A9e_Nationale_%28Lunon%29.jpg/1200px-Biblioth%C3%A8que_de_l%27Assembl%C3%A9e_Nationale_%28Lunon%29.jpg)
# Library Database

#### C# Friday Independant Project, 07.31.2020

## By Benjamin Russell, Micheal Hansen

## Description

_This application will help a librarian keep track of the books and authors in their library. Will also allow users to register for accounts on the library database and checkout books. There is a New York Times API that shows the top stories of the day on a page._

## Setup/Installation Requirements

* Clone this repository from GitHub https://github.com/brussell36/Library.Solution.git
* Open the downloaded directory in a text editor of your choice. (VSCode, Atom, etc.)
* Install [.Net Core](https://dotnet.microsoft.com/download/dotnet-core/2.2) 
* To install the REPL dotnet script, run ```dotnet tool install -g dotnet-script``` in your terminal.
* Run ```dotnet restore``` in terminal to get all dependencies.
* Run the command ```dotnet build```.

#### Additional Setup/Installation Notes:

* You will need to have MySQL installed on your computer to start the database for this project. 
* Once you have it installed open your terminal and run ```mysql -uroot -pepicodus```. This will start the mysql server on your computer. 
* With mysql running in your terminal copy/paste the code snippet below.
* Run ```dotnet ef migrations add Initial``` and ```dotnet ef database update``` to create the schema and all required tables.
* Then you will have the database correctly set up and you can start the project in a separate terminal that the one running mysql by running ```dotnet run```.

## Specifications

| Behavior | Input | Output |
| -------- | ----- | ------ |
| 1. As a librarian, I want to create, read, update, delete, and list books in the catalog, so that we can keep track of our inventory. |  | List of all books |
| 2. As a librarian, I want to search for a book by author or title, so that I can find a book when there are a lot of books in the library. | Cats Cradle | Cats Cradle by Kurt Vonnegut |
| 3. As a librarian, I want to enter multiple authors for a book, so that I can include accurate information in my catalog. | Issac Asimov  | Cats Cradle by Kurt Vonnegut, Issac Asimov |
| 4. As a patron, I want to check a book out, so that I can take it home with me.| Cats Cradle | Cats Cradle in Patron details |


## Known Bugs

_No known issues. Please contact me if you have any problems._


## Support and contact details

Please feel free to contact me through GitHub (username: brussell36) with any questions, ideas or concerns.  

## Technologies Used

* C#
* .NET Core 2.2
* Visual Studio Code 
* Git and Git BASH 
* MySQL Workbench
* MySQL Database
* Entity Framework
* Identity


### License

*This site is licensed under the MIT license.*

Copyright (c) 2020 **_Benjamin Russell and Micheal Hansen_**