FROM mcr.microsoft.com/dotnet/aspnet:5.0

RUN apt-get update

# Add a user for running applications.
RUN useradd apps
RUN mkdir -p /home/apps && chown apps:apps /home/apps

RUN apt-get install -y x11vnc xvfb fluxbox wmctrl wget gnupg2 unzip

# Set the Chrome repo.
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list

# Install Chrome.
RUN apt-get update && apt-get -y install google-chrome-stable

COPY bin/Release/net5.0/publish/ App/
#Download Chrome driver
RUN wget -O /tmp/chromedriver_linux64.zip https://chromedriver.storage.googleapis.com/91.0.4472.101/chromedriver_linux64.zip
RUN unzip /tmp/chromedriver_linux64.zip chromedriver -d /App
RUN rm /tmp/chromedriver_linux64.zip

WORKDIR /App

COPY bootstrap.sh /

#Switch user
USER apps
ENTRYPOINT ["/bootstrap.sh"]
CMD ["dotnet", "/App/PurchaseForMeService.dll"]