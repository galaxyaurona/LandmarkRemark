Assuming user does not need password to sign in
Assuming if user does not exist , it will sign up, otherwise sign in

separate sign up/ login is out of scope in term of effort level

use ssl https, but self generated certificate
 
OSX use SQLite by default, or explicitly specified otherwise use localdb
TODO: write about changing your connection string in appsettings.json



Design choice of location  (need to be normalized ?)

Future work (normalize lat lng) so many user, many locations,
each pair contain note, timestamp, etc


Using mog

Apply Repository pattern on top of EF Core unit of work pattern

using xUnit.