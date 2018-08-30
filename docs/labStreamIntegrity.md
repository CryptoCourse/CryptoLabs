## `GET <host>/api/StreamIntegrity`

Проврека работоспособности контроллера. Возращает `operating`.

| Парметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы

## `GET <host>/api/StreamIntegrity/<userId>/<challengeId>`

Возаращает зашированную строку на фиксированном для пары (userId, challengeId) ключе со случайным счётчиком.

| Парметр| Описание| 
| --- | --- 
| `<host>` | имя хоста веб службы
| `<userId>` | идентификатор студента
| `<challengeId>` | идентификатор задания


## `GET <host>/api/StreamIntegrity/<userId>/<challengeId>/noentropy`

Возаращает зашированную строку на фиксированном для пары (userId, challengeId) ключе с фиксированным счётчиком.

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
