Дана REST служба с API указанным ниже.

Задача - получить ответ от службы с текстом "Wellcome to secretNet!"

В качестве шифра используется поточный шифр (аналог одноразового блокнота, при котором блинная гамма вырабатывается из короткого ключа и затем складывается по модулю с открытым текстом). Первые 16 байт шифртекста являются значением счётчика. Изменять нужно лишь последующие байты шифртекста, оставив первые байты без изменений.

**NB.** Для простоты можно считать шифр одноразовым блокнотом, игнорируя первые 16 байт шифртекста.

**ВАЖНО!** Все передаваемые сообщения должны быть предварительно закодированы с помощью BASE64. Полученные сообщения так же должны быть декодированы из BASE64 в указанный тип.

**ВАЖНО!** Все строки кодируется с использование кодировки ASCII.

т.е. передача строки для лабы выглядит следующим образом:
строка -> (asсii) -> массив байт -> (base64) -> строка -> json 

## Ход работы.

### Тестирование 

`<userId>` = имя аккаунта на GitHub  (или фамилия студента)

`<challengeId>` = 1

1. Проверить работоспособность контроллера с помощью метода `GET <host>/api/StreamIntegrity`
2. Получить зашифрованное сообщение с помощью метода `GET <host>/api/StreamIntegrity/<userId>/<challengeId>/noentropy`
3. Декодировать сообщение из BASE64, получив шифртекст
4. Получить модифицированное сообщение
5. Закодировать модифицированное сообщение с помощью BASE64 и отправить с использованием метода `POST <host>/api/StreamIntegrity/<userId>/<challengeId>`
6. Получить ответ от сервера, декодировать из BASE64, убедиться, что текст соответствует строке "Wellcome to secretNet!".

### Сдача лабы
шаги 1 - 6 этапа тестирования аналогично, но использование метода `GET <host>/api/StreamIntegrity/<userId>/<challengeId>` на шаге 2.


## Описание API

Rest запросы, в заголовке выставлен Content-Type: application/json; charset=utf-8.

### Описание методов

## `GET <host>/api/StreamIntegrity`

Проверка работоспособности контроллера. Возвращает `operating`. Ответ не кодируется в BASE64.

| Параметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы

## `GET <host>/api/StreamIntegrity/<userId>/<challengeId>`

Возвращает зашифрованную строку "Here is some data to encrypt for you" на фиксированном для пары (userId, challengeId) ключе со случайным счётчиком.

| Параметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы
| `<userId>` | идентификатор студента
| `<challengeId>` | идентификатор задания


## `GET <host>/api/StreamIntegrity/<userId>/<challengeId>/noentropy`

Возвращает зашированную строку "Here is some data to encrypt for you" на фиксированном для пары (userId, challengeId) ключе с фиксированным счётчиком.

| Параметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы
| `<userId>` | идентификатор студента
| `<challengeId>` | идентификатор задания

## `POST <host>/api/StreamIntegrity/<userId>/<challengeId>`

Расшифровывает строку на фиксированном для пары (userId, challengeId) ключе.
Если строка расширована как "Token: 8ce08ad2d48d7d356db43", возвращает строку "Wellcome to secretNet!".

| Параметр| Описание| 
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
