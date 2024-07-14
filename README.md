
Create with ASP.NET Core, React.ts, mobX, EF core, PostgreSQL

How to start this application
(need the node.js and .net installed)
1. Clone this repository
2. Open the terminal and go to the project folder
3. fix the `appsettings.json` file to connect to the database and set your own connection string
4. Run the command `dotnet ef database update` in root folder to create the database
5. Run the command `npm i` in the `Client/Client` folder to install the packages
6. Run the command `npm run start` in the `Client/Client` folder to start the client
7. Run the command `dotnet run` in the root folder to start the server
8. Open the browser and go to the `http://localhost:3000/` to see the application
9. Open the browser and go to the `http://localhost:5274/` to see the swagger documentation

if your server start in another port, change the `index.ts` file in `Client/Client/src/http` to the your port

Made by **ULtaR**