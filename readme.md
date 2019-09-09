# Landmark Remark

## Introduction
This source code contain the solution to Tigerspike technical challenge's **Landmark remark**

## Requirement
### Explicit requirement
| 1. As a user (of the application) I can see my current location on a map
 2. As a user I can save a short note at my current location
3. As a user I can see notes that I have saved at the location they were saved
on the map
4. As a user I can see the location, text, and user-name of notes other users
have saved
5. As a user I have the ability to search for a note based on contained text or
user-name
### Implicit requirement
1. The user must be able to sign up or login to see location/ retreive notes, search for note.
2. Users only need their username to login.  Login with password is out of scope for current solution iteration 
3. When user login, if the username not existed in database, the application will sign user up. Separate login, sign up will be implement in future iteration
4. A user can have many notes, and a note belongs to one user (one -to - many relationship)

## Solution Architecture & Technology use
The solution make use of **Backend RESTful API** + **Frontend SPA** application for scalability and maintainability.
Total time spend developing the solution is about **15 man hours**, including **2 hour** writing this document.

### Backend (API)
The main project is scaffolded using **Visual studio 2017**. The scafolding template use **.Net core 2.1**, 

The application make use of **Entity Framework Core** as a default ORM. I decided to use SQLite database for development to let application can be developed on both Mac and Window environment.

The test project use [Xunit](https://xunit.net/) as main testing framework. I use [Moq](https://github.com/Moq/moq4/wiki/Quickstart) for mocking repository when testing service. In memory SQLite is used when testing repository.  

I spent about **9 man hours** develop the backend and its associate tests.

### Frontend (SPA)
The frontend code is under `/ClientApp` and is implemented using **React.js** with **Redux**. The front end is scaffolded using [create-react-app](https://github.com/facebook/create-react-app)
Other technologies use:
- [Google map Javascript API] (https://developers.google.com/maps/documentation/javascript/tutorial)
- [google-maps-react](https://www.npmjs.com/package/google-maps-react) to display location and marker on map
- [React-bootstrap v3](https://react-bootstrap-v3.netlify.com/) for stylings and responsive design

I spent about **4 man hours** develop the frontend.

## Start up guide
The submission should contain all the file need to run the solution. However, there are few potentiall issue due to different running environment. (Maybe containerization in future work)

### Database
I included the SQLite database file to minimize runtime errors. But if there is no *.db file (maybe for best practice I deleted it), You can open terminal and rerun the [migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/) (check out the `Update the database` section)
### Frontend
Ensure you have [NodeJS] v8 and above installed.
The application should taken care of resporing npm packages for frontend when building in Visual Studio. But if manually cd into `/ClientApp` and run `npm install` to ensure the packages are all up to date.

### Test
Visual studio 2017 should be able to run test contain in LandmarkRemark.Test. However, if you like CLI and have .net core runtime install, you can `cd` into LandmarkRemark.Test and run`dotnet test`

