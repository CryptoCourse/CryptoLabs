Стойкость симметричный криптопримитивов с ключом основывается на предположении о случайности данного ключа. При этом предполагается, что ключ был получен случайно из равномерно распределённого множества ключей достаточной размерности.

Иными словами, стойкость симметричной криптосистемы требует максимальной энтропии ключа.

В реальности, однако, большинство источников случайности (энтропии) не обладают равномерным распределением, и ключевой материал, полученный из данных источников, не может быть использованы напрямую в качестве симметричных ключей.

Если же необходимо получить симметричный ключ на основе такого ключевого материала используют так называемые Функции Выработки Ключа (Key Derivation Functions, KDF).

![img](https://github.com/CryptoCourse/CryptoLabs/blob/master/Impl/imgs/Single-phase-model-for-KDFs_W640.jpg)

В данной работе рассматривается применение KDF к неравномерно распределенному источнику энтропии с целью выработки равномерно распределённых симметричных ключей.

## `HKDF`

KDF состоит их двух подфункций: извлечения (extract) и расширения (expand).

![img](https://github.com/CryptoCourse/CryptoLabs/blob/master/Impl/imgs/Extract-then-expand-model-for-KDFs_W640.jpg)

Функция извлечения получает равномерно распределённый случайный ключ, используя неравномерно распределённый ключевой материал.

Функция расширения формирует последовательность ключей на основе одного случайного равномерно распределённого ключа.

`HKDF` - KDF на основе кода аутентичности `HMAC`. 
```
Извлечение: PRK <- HMAC(XTS, SKM)
Расширение: K_i <- HMAC(PRK, К_{i-1} CTX, i), если i=1, K_0 - пустая строка
```

### 1. На основе файла [weather.json](https://github.com/CryptoCourse/CryptoLabs/blob/master/Impl/weather.json) построить гистограммы 

температуры, влажности, скорости ветра, облачности и озонового слоя (выбрать данные по часам). "Толшину" столбца гистограммы выбрать так, что бы было наглядно неравномерное распределение величин. Выбрать одну из указанных величин (или комбинацию величин) в качестве ключевого материала.

### 2. Реализовать [HMAC](https://en.wikipedia.org/wiki/HMAC) на основе хэш функции `SHA-256`. 

В качестве SHA-256 использовать криптографически стойкую реализацию из общераспространённой библиотеке на вашем языке.

![img](https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcR45Fu58KVP7gP_YF4SnuWl0kR5hYwawtMpiIpVBqUHU4RtYmGa)

`HMAC(K,C) = H((K + opad) || H((K + ipad) || m))`

`opad = 0x5c, ..., 0x5c`

`ipad = 0x36, ..., 0x36`

Интерфейс функции: `byte[] HmacSha256(byte[] key, byte[] data)`

### 3. Реализовать функцию `HkdfExtract`, которая на основе `HMAC`, в качестве псевдослучайной функции, соли `XTS` и ключевого материала `SKM`) получает ключ `PRK` для псевдослучайной функции.

Интерфейс функции: `byte[] HkdfExtract(byte[] XTS, byte[] SKM)`

RFC 5869, 2.2

```
 HKDF-Extract(salt, IKM) -> PRK

   Options:
      Hash     a hash function; HashLen denotes the length of the
               hash function output in octets

   Inputs:
      salt     optional salt value (a non-secret random value);
               if not provided, it is set to a string of HashLen zeros.
      IKM      input keying material

   Output:
      PRK      a pseudorandom key (of HashLen octets)

   The output PRK is calculated as follows:

   PRK = HMAC-Hash(salt, IKM)
```

### 4. Реализовать функцию `HkdfExpand`, которая на основе псевдослучайной функции `HMAC`, её ключа `PRK`, контекста `CTX`, прошлого ключа `lastKey` и счетчика `i` получает `i`-й симметричный ключ. 

Если прошлого ключа нет передаётся значение `null`.

Интерфейс функции: `byte[] HkdfExpand(byte[] PRK, byte[] lastKey, byte[] CTX, int i)`

RFC 5869, 2.3

```
HKDF-Expand(PRK, info, L) -> OKM

   Options:
      Hash     a hash function; HashLen denotes the length of the
               hash function output in octets
   Inputs:
      PRK      a pseudorandom key of at least HashLen octets
               (usually, the output from the extract step)
      info     optional context and application specific information
               (can be a zero-length string)
      L        length of output keying material in octets
               (<= 255*HashLen)

   Output:
      OKM      output keying material (of L octets)

   The output OKM is calculated as follows:

   N = ceil(L/HashLen)
   T = T(1) | T(2) | T(3) | ... | T(N)
   OKM = first L octets of T

   where:
   T(0) = empty string (zero length)
   T(1) = HMAC-Hash(PRK, T(0) | info | 0x01)
   T(2) = HMAC-Hash(PRK, T(1) | info | 0x02)
   T(3) = HMAC-Hash(PRK, T(2) | info | 0x03)
   ...

   (where the constant concatenated to the end of each T(n) is a
   single octet.)
```

### 5. Для `i` =1..1000 получить 1000 симметричных ключей длины 256 бит на основе `HKDF`, где `data` - выбранные данные на шаге 1.
В качестве соли использовать случайную равномерно распределённую величину, полученную и использованием криптографического Г(П)СЧ, реализованного в вашем языке. Длина соли `XTS` - 256 бит.

```
CTX <- "Ваше имя"
XTS <- Crypto.Random(256)
PRK <- HkdfExtract(XTS, data)
K_i = HkdfExpand(PRK, K_{i-1} CTX, i)
```

### 5. Убедиться в равномерной распределённости первых 10 бит ключей, построив гистограмму.

## `PBKDF2`

`HKDF` позволяет получить равномерные данные из неравномерно распределённого источника энтропии. Однако если источник обладает низкой энтропией (например - пароли)
необходимо использовать `PBKDF2`, основным изменением которого является медленное хэширование, необходимое для увеличения сложности перебора ключевого материала.
`P` - пароль пользователя (ключевой материал), `S` - соль, `len` = `|K|/|HMAC|`, `|K|` - размер ключа, `|HMAC|` - размер выхода HMAC.

```
U_1 = HMAC(P, S||i)
U_c = HMAC(P, U_{c-1})
F(P,S, c, i) = U_1 + .... + U_c, '+' = XOR
T_i = F(P, S, c, i)
K = T_1 || T_len
```

![img](https://upload.wikimedia.org/wikipedia/commons/7/70/Pbkdf2_nist.png)

### 1. На основе файла [passwords.json](https://github.com/CryptoCourse/CryptoLabs/blob/master/Impl/passwords.json) построить гистограмму распределения первых 10 бит паролей (кодировка ASCII).
### 2. Реализовать `PBKDF2` с использованием `HMAC` в качестве `PRF`, с использованием случайного `S`. Число итераций 10000.
### 3. Получить симметричный ключ для каждого пароля длины 512 бит.
### 4. Убедиться в равномерной распределённости первых 10 бит ключей, построив гистограмму.

## Результат работы
**HKDF** Гистограммы (любой из: jpg, pdf, png, xlsx), обоснование выбора данных для ключевого материала, код.

**PBKDF** Гистограмма (любой из: jpg, pdf, png, xlsx), код.

## Дополнительные ссылки

https://github.com/CryptoCourse/CryptoLectures/blob/master/Lectures/KdfLite.pdf

https://en.wikipedia.org/wiki/HMAC

https://tools.ietf.org/html/rfc2104

https://www.rfc-editor.org/rfc/rfc5869

https://en.wikipedia.org/wiki/PBKDF2

https://tools.ietf.org/html/rfc2898#section-5.2

https://eprint.iacr.org/2010/264.pdf
