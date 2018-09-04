#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 64325
EXPOSE 44342

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["FlyMe/FlyMe.csproj", "FlyMe/"]
RUN dotnet restore "FlyMe/FlyMe.csproj"
COPY . .
WORKDIR "/src/FlyMe"
RUN dotnet build "FlyMe.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FlyMe.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FlyMe.dll"]