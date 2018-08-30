Дана REST служба с API указанным ниже.

Задача - получить ответ от службы с текстом "Wellcome to secretNet!"

**ВАЖНО!** Все передаваемые сообщения должны быть предварительно закодированы с помощью BASE64. Полученные сообщения так же должны быть декодированы из BASE64 в указанный тип.

**ВАЖНО!** Все строки кодируется с использование кодировки ASKII.

т.е. передача строки для лабы выглядит следующим образом:
строка -> (aski) -> массив байт -> (base64) -> строка -> json 

## Ход работы.

### Тестирование 

`<userId>` = имя аккаунта на GitHub

`<challengeId>` = 1

1. Проверить работоспособность контроллера с помощью метода `GET <host>/api/StreamIntegrity`
2. Получить зашифрованное сообщение с помощью метода `GET <host>/api/StreamIntegrity/<userId>/<challengeId>/noentropy`
3. Декодировать сообщение из BASE64, получив шифртекст
4. Получить модифицированное сообщение
5. Закодировать модифицированное сообщение с помощью BASE64 и отправить с использованием метода `POST <host>/api/StreamIntegrity/<userId>/<challengeId>`
6. Получить ответ от сервера, декодировать из BASE64, убедиться что текст соотвествует строке "Wellcome to secretNet!".

### Сдача лабы
шаги 1 - 6 этапа тестирования аналогично, но использование метода `GET <host>/api/StreamIntegrity/<userId>/<challengeId>` на шаге 2.


## Описание API

Rest запросы, в заголовке выстален Content-Type: application/json; charset=utf-8.

### Описание методов

## `GET <host>/api/StreamIntegrity`

Проверка работоспособности контроллера. Возращает `operating`.

| Парметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы

## `GET <host>/api/StreamIntegrity/<userId>/<challengeId>`

Возаращает зашированную строку "Here is some data to encrypt for you" на фиксированном для пары (userId, challengeId) ключе со случайным счётчиком.

| Парметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы
| `<userId>` | идентификатор студента
| `<challengeId>` | идентификатор задания


## `GET <host>/api/StreamIntegrity/<userId>/<challengeId>/noentropy`

Возаращает зашированную строку "Here is some data to encrypt for you" на фиксированном для пары (userId, challengeId) ключе с фиксированным счётчиком.

| Парметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы
| `<userId>` | идентификатор студента
| `<challengeId>` | идентификатор задания

## `POST <host>/api/StreamIntegrity/<userId>/<challengeId>`

Расшифровывает строку на фиксированном для пары (userId, challengeId) ключе.
Если строка расширована как "Token: 8ce08ad2d48d7d356db43", возвращает строку "Wellcome to secretNet!".

| Парметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы
| `<userId>` | идентификатор студента
| `<challengeId>` | идентификатор задания



### Пример составления тестового post запроса на python

```python
import json
import requests

r = requests.post("http://192.168.130.133/api/values", data=json.dumps("HereIsSomeStringData"), headers = {'Content-Type': 'application/json'})
print(r.status_code, r.reason)
print(r.text)
```

### Пример post запроса
```
POST http://192.168.130.133/api/values HTTP/1.1
Connection: Keep-Alive
Content-Type: application/json; charset=utf-8
Content-Length: 22
Host: 192.168.130.133

"HereIsSomeStringData"
```
