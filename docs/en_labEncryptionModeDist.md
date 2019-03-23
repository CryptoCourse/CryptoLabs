REST service with api defined below.

The goal is to build efficient cipher mode distinguisher for encryption oracle (ECB, CBC).

**Important!** All text messages are encoded the following way:
string -> (asсii) -> byte_array -> (base64) -> string -> json

## 

1. Fix arbitrary `userId` string of your choise.

2. Create distinguisher for method `POST <host>/api/EncryptionModeOracle/<userId>/<challengeId>/noentropy` for different `challengeId`.

3. Valide the result of distinguisher using method `GET <host>/api/EncryptionModeOracle/<userId>/<challengeId>/verify`

4. Make shure the distinguisher is running correctly for method `POST <host>/api/EncryptionModeOracle/<userId>/<challengeId>/`

## API Description


Rest requests, Content-Type: `application/json; charset=utf-8`.


### Methods description:



## `GET <host>/api/EncryptionModeOracle`



Test controller. Return `operating`. Answer is plain string without base64 encoding.



| Parameter | Description | 
| --- | --- |
| `<host>` | service host name |



## `POST <host>/api/EncryptionModeOracle/<userId>/<challengeId>`


Appends random data at the beginning and at the end of user data. 
After that encrypts it with either CBC or ECB mode with fixed key.

| Parameter | Description | 
| :--- | :--- |
| `<host>` | service host name |
| `<userId>` | userId |
| `<challengeId>` | task id |

## `POST <host>/api/EncryptionModeOracle/<userId>/<challengeId>/noentropy`

Appends fixed data at the beginning and at the end of user data. 
After that encrypts it with either CBC or ECB mode with fixed key.

| Parameter | Description | 
| :--- | :--- |
| `<host>` | service host name |
| `<userId>` | userId |
| `<challengeId>` | challenge id |

## `GET <host>/api/EncryptionModeOracle/<userId>/<challengeId>/verify`

Return coorect answer for challenge in`POST <host>/api/EncryptionModeOracle/<userId>/<challengeId>/noentropy`.
Returns one of the following plain strings: "ECB" or "CBC"

| Parameter | Description | 
| --- | --- |
| `<host>` | service host name|
| `<userId>` | userId|
| `<challengeId>` | challenge id|


## Example python post request (without base64 encoding)

```python
import json
import requests

r = requests.post("http://192.168.130.133/api/values", data=json.dumps("HereIsSomeStringData"), headers = {'Content-Type': 'application/json'})
print(r.status_code, r.reason)
print(r.text)
```

[more info on python requests](http://docs.python-requests.org/en/master/http://docs.python-requests.org/en/master/)

[base64 in python](https://docs.python.org/2/library/base64.html)
