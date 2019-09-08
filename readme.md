Assuming user does not need password to sign in
Assuming if user does not exist , it will sign up, otherwise sign in

separate sign up/ login is out of scope in term of effort level

use ssl https, but self generated certificate
 
OSX use SQLite by default, or explicitly specified otherwise use localdb
TODO: write about changing your connection string in appsettings.json



Design choice of using note as main entity, location's as lat and long
piggyback in note


Using moq

Apply Repository pattern on top of EF Core unit of work pattern, for testability

using xUnit.

cd into ClientApp and run npm install
runing npm install 


global exception handler , only expose the outer message, log the rest

validate model logic using data annotation where I can
validate business logic using service

Future work (normalize lat lng) so many user, many locations,
 each user-location relationship represent by a note
 Using validator , and dependency injection for scalability

 Future work separate login, auth, implement db with hash and salt

 future work implement pagination for notes first