FROM microsoft/dotnet:2.1-aspnetcore-runtime
COPY /deploy /
WORKDIR /Server
EXPOSE 8085
ENTRYPOINT ["dotnet", "Server.dll"]
ENV DOTNET_VERSION 2.1.6
ENV ASPNETCORE_PKG_VERSION 2.1.6