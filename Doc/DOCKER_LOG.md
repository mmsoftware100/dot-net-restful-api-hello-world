PS D:\Cisco\Code\Workout\dot-net-restful-api-hello-world> docker build -t testapi .
[+] Building 389.6s (18/18) FINISHED                                                                       docker:desktop-linux 
 => [internal] load build definition from Dockerfile                                                                       0.1s 
 => => transferring dockerfile: 1.17kB                                                                                     0.0s 
 => [internal] load metadata for mcr.microsoft.com/dotnet/sdk:8.0                                                          2.4s 
 => [internal] load metadata for mcr.microsoft.com/dotnet/aspnet:8.0                                                       2.3s 
 => [internal] load .dockerignore                                                                                          0.0s 
 => => transferring context: 143B                                                                                          0.0s 
 => [build 1/7] FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:1682f3cd0cb51464568de4fa88c719bd3be55539653d97540718bdd5b8  278.9s 
 => => resolve mcr.microsoft.com/dotnet/sdk:8.0@sha256:1682f3cd0cb51464568de4fa88c719bd3be55539653d97540718bdd5b87062d4    0.0s 
 => => sha256:9aae76ae0a88482d734df1bd019dc0b4e6cd3e790cb66a620661dc96a0ff54b7 16.95MB / 16.95MB                          51.3s 
 => => sha256:ae4ea76689275e487288936c5d0aae4fd02bb8f72a021b32b6a1efdd2b5421c8 2.64kB / 2.64kB                             0.6s 
 => => sha256:c7f2762549c7d3f4f68be1f17529924e17a8ea96b6cebe61e2e3d6087d85b625 178.31MB / 178.31MB                       214.7s 
 => => sha256:530df4b5a091a089d89f3610b23e1fd21947fc9fd499e0dad8d7c5684c64bd1e 30.89MB / 30.89MB                          37.7s 
 => => extracting sha256:530df4b5a091a089d89f3610b23e1fd21947fc9fd499e0dad8d7c5684c64bd1e                                  1.1s 
 => => extracting sha256:c7f2762549c7d3f4f68be1f17529924e17a8ea96b6cebe61e2e3d6087d85b625                                  4.2s
 => => extracting sha256:ae4ea76689275e487288936c5d0aae4fd02bb8f72a021b32b6a1efdd2b5421c8                                  0.0s
 => => extracting sha256:9aae76ae0a88482d734df1bd019dc0b4e6cd3e790cb66a620661dc96a0ff54b7                                  0.5s
 => [internal] load build context                                                                                          0.3s
 => => transferring context: 115.62kB                                                                                      0.2s
 => [base 1/2] FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:0d4a76c0a692c7419acf59529ba6dae8552f4fd59e8453538b5291710  95.7s 
 => => resolve mcr.microsoft.com/dotnet/aspnet:8.0@sha256:0d4a76c0a692c7419acf59529ba6dae8552f4fd59e8453538b5291710a29fb9  0.0s 
 => => sha256:d0e012fe50a5fce165625f0c11fd7b0515caf18a29ee5d821560cb4d7ed32b0c 11.09MB / 11.09MB                          38.1s 
 => => sha256:db930ae65e7bca8983a28685652281dc12af44c148d44c306ecbc8b8fee5bd88 154B / 154B                                 0.6s 
 => => sha256:b54d92365d53180b8e71b66aa3bf88de33ac113d0d48777636c18a8092080a74 32.25MB / 32.25MB                          94.5s 
 => => sha256:53fa25c982c2a90e5be4dfd30805059f369af52a2b88e8267a2b69f6614cf7f2 3.28kB / 3.28kB                             0.7s 
 => => sha256:fdf52f814d990e5a1dd81d45781c811a493203ce6c9025a7cfd4cabf5e204a01 18.74MB / 18.74MB                          57.8s 
 => => sha256:4831516dd0cb86845f5f902cb9b9d25b5c853152c337eb57e4737a9b7e2a2eb9 28.23MB / 28.23MB                          88.9s 
 => => extracting sha256:4831516dd0cb86845f5f902cb9b9d25b5c853152c337eb57e4737a9b7e2a2eb9                                  1.3s 
 => => extracting sha256:fdf52f814d990e5a1dd81d45781c811a493203ce6c9025a7cfd4cabf5e204a01                                  0.4s 
 => => extracting sha256:53fa25c982c2a90e5be4dfd30805059f369af52a2b88e8267a2b69f6614cf7f2                                  0.0s 
 => => extracting sha256:b54d92365d53180b8e71b66aa3bf88de33ac113d0d48777636c18a8092080a74                                  0.5s 
 => => extracting sha256:db930ae65e7bca8983a28685652281dc12af44c148d44c306ecbc8b8fee5bd88                                  0.0s 
 => => extracting sha256:d0e012fe50a5fce165625f0c11fd7b0515caf18a29ee5d821560cb4d7ed32b0c                                  0.2s 
 => [base 2/2] WORKDIR /app                                                                                                0.7s 
 => [final 1/2] WORKDIR /app                                                                                               0.2s 
 => [build 2/7] WORKDIR /src                                                                                               1.9s 
 => [build 3/7] COPY [TestAPI.csproj, .]                                                                                   0.2s 
 => [build 4/7] RUN dotnet restore "./TestAPI.csproj"                                                                     95.0s 
 => [build 5/7] COPY . .                                                                                                   0.2s 
 => [build 6/7] WORKDIR /src/.                                                                                             0.1s 
 => [build 7/7] RUN dotnet build "./TestAPI.csproj" -c Release -o /app/build                                               6.7s 
 => [publish 1/1] RUN dotnet publish "./TestAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false                     2.5s 
 => [final 2/2] COPY --from=publish /app/publish .                                                                         0.2s 
 => exporting to image                                                                                                     1.9s 
 => => exporting layers                                                                                                    1.4s 
 => => exporting manifest sha256:fc3acb31e82eb716f771ad1c8948c96cfe21531c715d688c8b7a27f98d55ad5f                          0.0s 
 => => exporting config sha256:4752e1a5f957b859a0cc19f2040d8aa3e57785737cf418c15078f4b2b4a4c99c                            0.0s 
 => => exporting attestation manifest sha256:1592dfd1da0d00558248c5959bc0cf6c3266a2fd91b9ee89d8dffdb4cd7ab642              0.1s 
 => => exporting manifest list sha256:98349727867d92a7555d2e396b8424ed6ab7455882a4bfacdc96e8ffac9ef4da                     0.0s 
 => => naming to docker.io/library/testapi:latest                                                                          0.0s 
 => => unpacking to docker.io/library/testapi:latest                                                                       0.3s 
PS D:\Cisco\Code\Workout\dot-net-restful-api-hello-world> docker ps
CONTAINER ID   IMAGE     COMMAND   CREATED   STATUS    PORTS     NAMES
PS D:\Cisco\Code\Workout\dot-net-restful-api-hello-world> docker run -d -p 8080:8080 --name mytestapi testapi
ec57a77351bfea821a68264cffe031cf77c25382f2e739d26a1f2653ec6ad5b3
PS D:\Cisco\Code\Workout\dot-net-restful-api-hello-world> docker ps
CONTAINER ID   IMAGE     COMMAND                CREATED         STATUS         PORTS                                         NAMES
ec57a77351bf   testapi   "dotnet TestAPI.dll"   5 seconds ago   Up 3 seconds   0.0.0.0:8080->8080/tcp, [::]:8080->8080/tcp   mytestapi
PS D:\Cisco\Code\Workout\dot-net-restful-api-hello-world> 