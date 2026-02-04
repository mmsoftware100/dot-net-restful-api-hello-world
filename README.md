# RESTFul API Hello World .NET 8


## Development Setup

```bash
# install entity framework tool
dotnet tool install --global dotnet-ef

# migrate
dotnet ef database update

dotnet run

#info: Microsoft.Hosting.Lifetime[14]
#      Now listening on: http://localhost:5122

```


## Docker

docker build -t testapi .
docker run -d -p 8080:8080 --name mytestapi testapi

docker rm -f mytestapi



###  check logs
docker logs -f mytestapi

## Production Deployment


Connect with mysql database

host: localhost
port: 3306
db: dotnetapi
username: root
password: {blank}


- [x] Home Page added.


Let's create Wish Bottle API

that anyone (anonymous) can create a wish bottle.
- [x] Wish Bottle API added.
- [x] Wish Bottle Model added.
- [x] Wish Bottle Controller added.

Main CRUD Feature

- [ ] create bottle (just plain text , 256 character or something)
- [ ] get one random bottle per request


## Deployment on ubnut


CI CD? 

on thank book server?


Seeder ဘယ်လိုထည့်မလဲ?

dotnet run
# Uses local MySQL database


ASPNETCORE_ENVIRONMENT=Production dotnet run
# Uses production MySQL database at 103.****


dotnet run --launch-profile production 

a little messy, but we get what we want.




<!-- Security scan triggered at 2025-09-28 15:30:51 -->

<!-- Security scan triggered at 2025-09-28 15:36:56 -->