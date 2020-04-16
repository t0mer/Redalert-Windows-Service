# Red Alert Windows Service
__________________________________________

c# based windows service that reads json from [Oref Website](https://www.oref.org.il/) and publishes it over MQTT Protocol and/or Telegram Bot.

## Installation
In order to install the service download the code and compile it. than move the binary files as listed below in to a new folder under Program Files:
 - RedAlert.exe
 - RedAlert.exe.config
 - Newtonsoft.Json.dll
 - M2Mqtt.Net.dll
 And run the following command from Command line (Make sure to run as admin):
```
 c:\Windows\Microsoft.NET\Framework64\v4.0.30319 "c:\Program Files\RedAlert\RedAlert.exe"
```

## Service configuration
### RedAlert.exe.config
In order to use this service, some configuration need to be done.
Open RedAlert.exe.config in any text editor and edit the required settings as follows:
```xml
 <appSettings>
    <!--General Settings-->
    <add key="IsDebugMode" value="false"/>
    <!--Telegrem Bot Settings-->
    <add key="IsTelegramEnabled" value="false"/>
    <add key="telegramApi" value=""/>
    <add key="channelId" value=""/>
    <add key="telegramApiUrl" value="https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&amp;text={2}" />
    <!--Mqtt Settings-->
    <add key="IsMqttEnabled" value="false"/>
    <add key="MqttHost" value=""/>
    <add key="MqttUser" value=""/>
     <add key="MqttUPAss" value=""/>
 </appSettings>
```
[How to create Telgram Bot](https://techblog.co.il/2019/11/%D7%A9%D7%9C%D7%99%D7%97%D7%AA-%D7%94%D7%95%D7%93%D7%A2%D7%95%D7%AA-%D7%9C%D7%A2%D7%A8%D7%95%D7%A5-%D7%98%D7%9C%D7%92%D7%A8%D7%9D-%D7%91%D7%93%D7%A8%D7%9A-%D7%94%D7%A7%D7%9C%D7%94/)

## Usage
### Adding Sensor in Home-Assistant
#### Get full json (including date and id)
```yaml
  - platform: mqtt
    name: "Red Alert"
    state_topic: "/redalert/"
    # unit_of_measurement: '%'
    icon: fas:broadcast-tower
    value_template: "{{ value_json }}"
    qos: 1
```

#### Get json with alert areas only
```yaml
  - platform: mqtt
    name: "Red Alert"
    state_topic: "/redalert/"
    # unit_of_measurement: '%'
    icon: fas:broadcast-tower
    value_template: "{{ value_json.data }}"
    qos: 1
```

