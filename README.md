# workbench-light
Windows Service for turning on and off overhead light depending on computer power state

## Background
I've been building an electrical engineering workbench, and one feature that annoyed me was the switch for my lights. Instead of fumbling around the power cord, I decided it would be best if they would turn on and off at the same time as my computer. I bought some of [these](https://www.amazon.com/Etekcity-Voltson-Monitoring-Required-Assistant/dp/B074GVPYPY/ref=sr_1_3?s=lamps-light&ie=UTF8&qid=1533522245&sr=1-3&keywords=smart+outlet) and reverse-engineered VeSync's app to use the smart outlets with just API calls. Then, I built this windows service to call the API depending on the power state; computer turns on, so do the lights. Computer turns off, so do the lights.

## Installation
1. Copy the contents of /bin/Debug to a folder of your choice, I like `C:\Program Files\Workbench Light`
2. In that folder, run the command `installutil.exe "Light Service.exe"`
3. Edit `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Workbench Light` in your registry, and add "token" "accountid" and "outletid" string values with the appropriate information.

## Determining Account Information
API Login Request:

`POST https://smartapi.vesync.com/vold/user/login`

`Content-Type: application/json`

`Body: {"account": "myname@gmail.com", "devToken": "", "password": "<md5 hash of password>"}`

API Login Response:

`{"tk":"token, goes in token registry subkey","accountID":"account id, goes in accountid subkey","nickName":"myname","avatarIcon":"","userType":1,"acceptLanguage":"","termsStatus":false}`

---

API Device List Request:

`GET https://smartapi.vesync.com/vold/user/devices`

`tk: your token from above`

API Device List Response:

`[{"deviceName":"vesync_wifi_outlet","deviceImg":"https://smartapi.vesync.com/v1/app/imgs/wifi/outlet/smart_wifi_outlet.png","cid":"outlet id, goes in outletid registry subkey","deviceStatus":"on","connectionType":"wifi","connectionStatus":"online","deviceType":"wifi-switch-1.3","model":"wifi-switch","currentFirmVersion":"1.89"},{"deviceName":"Test1","deviceImg":"https://smartapi.vesync.com/v1/app/imgs/wifi/outlet/smart_wifi_outlet.png","cid":"outlet id, goes in outletid registry subkey","deviceStatus":"on","connectionType":"wifi","connectionStatus":"offline","deviceType":"wifi-switch-1.3","model":"wifi-switch","currentFirmVersion":"1.89"}]`

---

API Outlet Turn On/Off Request:

`PUT: https://smartapi.vesync.com/v1/wifi-switch-1.3/your outlet id from above/status/on` (or off)

`Content-Type: application/json`

`tk: your token from above`

`accountid: your account id from above`

API Outlet Turn On/Off Response:

Just a 200 OK, no data
