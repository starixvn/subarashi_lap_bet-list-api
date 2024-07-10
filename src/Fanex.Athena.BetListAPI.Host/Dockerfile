FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR "/src"

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

# Copy common project and restore as distinct layers

COPY ["src/Fanex.Athena.Resources/Fanex.Athena.Resources.csproj", "./Fanex.Athena.Resources/"]
RUN dotnet restore "Fanex.Athena.Resources/Fanex.Athena.Resources.csproj" -v n

COPY ["src/Fanex.Athena.Common.Constant/Fanex.Athena.Common.Constant.csproj", "./Fanex.Athena.Common.Constant/"]
RUN dotnet restore "Fanex.Athena.Common.Constant/Fanex.Athena.Common.Constant.csproj" -v n

COPY ["src/Fanex.Athena.Common.Model/Fanex.Athena.Common.Model.csproj", "./Fanex.Athena.Common.Model/"]
RUN dotnet restore "Fanex.Athena.Common.Model/Fanex.Athena.Common.Model.csproj" -v n

COPY ["src/Fanex.Athena.Common.Utility/Fanex.Athena.Common.Utility.csproj", "./Fanex.Athena.Common.Utility/"]
RUN dotnet restore "Fanex.Athena.Common.Utility/Fanex.Athena.Common.Utility.csproj" -v n

COPY ["src/Fanex.Athena.Common.DataAccess/Fanex.Athena.Common.DataAccess.csproj", "./Fanex.Athena.Common.DataAccess/"]
RUN dotnet restore "Fanex.Athena.Common.DataAccess/Fanex.Athena.Common.DataAccess.csproj" -v n

COPY ["src/Fanex.Athena.Common.Services/Fanex.Athena.Common.Services.csproj", "./Fanex.Athena.Common.Services/"]
RUN dotnet restore "Fanex.Athena.Common.Services/Fanex.Athena.Common.Services.csproj" -v n

COPY ["src/Fanex.Athena.Common.Site/Fanex.Athena.Common.Site.csproj", "./Fanex.Athena.Common.Site/"]
RUN dotnet restore "Fanex.Athena.Common.Site/Fanex.Athena.Common.Site.csproj" -v n

# Copy Betlist Project and restore as distinct layers

COPY ["src/Fanex.Athena.BetList.Model/Fanex.Athena.BetList.Model.csproj", "./Fanex.Athena.BetList.Model/"]
RUN dotnet restore "Fanex.Athena.BetList.Model/Fanex.Athena.BetList.Model.csproj" -v n

COPY ["src/Fanex.Athena.BetList.DataAccess/Fanex.Athena.BetList.DataAccess.csproj", "./Fanex.Athena.BetList.DataAccess/"]
RUN dotnet restore "Fanex.Athena.BetList.DataAccess/Fanex.Athena.BetList.DataAccess.csproj" -v n

COPY ["src/Fanex.Athena.BetList.Service/Fanex.Athena.BetList.Service.csproj", "./Fanex.Athena.BetList.Service/"]
RUN dotnet restore "Fanex.Athena.BetList.Service/Fanex.Athena.BetList.Service.csproj" -v n

COPY ["src/Fanex.Athena.BetList.Excel/Fanex.Athena.BetList.Excel.csproj", "./Fanex.Athena.BetList.Excel/"]
RUN dotnet restore "Fanex.Athena.BetList.Excel/Fanex.Athena.BetList.Excel.csproj" -v n

COPY ["src/Fanex.Athena.BetList.Controllers/Fanex.Athena.BetList.Controllers.csproj", "./Fanex.Athena.BetList.Controllers/"]
RUN dotnet restore "Fanex.Athena.BetList.Controllers/Fanex.Athena.BetList.Controllers.csproj" -v n

# Copy Betlist result project and restore as distinct layers

COPY ["src/Fanex.Athena.Result.Model/Fanex.Athena.Result.Model.csproj", "./Fanex.Athena.Result.Model/"]
RUN dotnet restore "Fanex.Athena.Result.Model/Fanex.Athena.Result.Model.csproj" -v n

COPY ["src/Fanex.Athena.Result.DataAccess/Fanex.Athena.Result.DataAccess.csproj", "./Fanex.Athena.Result.DataAccess/"]
RUN dotnet restore "Fanex.Athena.Result.DataAccess/Fanex.Athena.Result.DataAccess.csproj" -v n

COPY ["src/Fanex.Athena.Result.Service/Fanex.Athena.Result.Service.csproj", "./Fanex.Athena.Result.Service/"]
RUN dotnet restore "Fanex.Athena.Result.Service/Fanex.Athena.Result.Service.csproj" -v n

COPY ["src/Fanex.Athena.Result.Controller/Fanex.Athena.Result.Controller.csproj", "./Fanex.Athena.Result.Controller/"]
RUN dotnet restore "Fanex.Athena.Result.Controller/Fanex.Athena.Result.Controller.csproj" -v n


# Copy host project and restore as distinct layers
COPY ["src/Fanex.Athena.BetListAPI.Host/Fanex.Athena.BetListAPI.Host.csproj", "./Fanex.Athena.BetListAPI.Host/"]
RUN dotnet restore "Fanex.Athena.BetListAPI.Host/Fanex.Athena.BetListAPI.Host.csproj" -v n

COPY "/src" .

WORKDIR "/src/Fanex.Athena.BetListAPI.Host"
RUN dotnet build "Fanex.Athena.BetListAPI.Host.csproj" -c Release -o /app/build

RUN dotnet publish "Fanex.Athena.BetListAPI.Host.csproj" -c Release -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-alpine

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Dont run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR "/app"

COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Athena.BetListAPI.Host.dll"]
