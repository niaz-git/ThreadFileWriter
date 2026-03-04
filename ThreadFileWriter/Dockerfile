FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ThreadFileWriter.csproj ./
COPY . ./
RUN dotnet restore ./ThreadFileWriter.csproj
RUN dotnet build -c Release
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
RUN mkdir -p /log
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ThreadFileWriter.dll"]
