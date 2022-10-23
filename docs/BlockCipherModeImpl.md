Здесь и далее используется AES с длиной ключа 128 бит.

## 1. Реализовать функцию на вашем языке программирования со следующим интерфейсом:

### `void SetKey(byte[] key)` - установка ключа шифрования\расшифрования, где

`key` - байтовое представление ключа блочного шифра

### `void SetMode(string mode)` - указание режима шифрования, где

`mode` - режим шифрования, допустимые значения ECB, CBC, CFB, OFB, CTR. Может быть задан через `Enum` на c#, или через константы.

### `byte[] ProcessBlockEncrypt(byte[] data, bool isFinalBLock, string padding)` - функция добавления блока открытого текста для зашифрования, содержит вызов функции `BlockCipherEncrypt`, где

`data` - блок для шифрования

`isFinalBLock` - флаг того, что передан последний блок шифруемого открытого текста

`padding` - вид дополнения, принимает значение `PKCS7` или `NON`.

возвращает зашифрованный блок данных.

В рамках данной функции должна быть реализована логика всех режимов, т.е. ветвление по режимам происходит тут.

### `byte[] BlockCipherEncrypt(byte[] data)` - функция шифрования блока данных алгоритмом AES, где

`data` - блок для шифрования

возвращает зашифрованный блок данных

В ходе реализации функции `BlockCipherEncrypt` необходимо пользоваться стандартной или общеизвестной реализацией алгоритма AES. Если библиотека не поддерживает одноблочное шифрование AES, необходимо воспользоваться режимом ECB.

Аналогично реализовать функцию расшифрования `BlockCipherDecrypt` и `ProcessBlockDecrypt`.

При шифровании/расшифровании необходимо инициализировать объект блочного шифра только один раз для сообщения (не на каждый блок).

Пример одноблочного шифрования на C#.
```csharp
namespace AesExample
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // Your Key here, 16 bytes
            byte[] key =
                { 0x73, 0x69, 0x78, 0x74, 0x65, 0x65, 0x6e, 0x2d,
                0x62, 0x79, 0x74, 0x65, 0x2d, 0x6b, 0x65, 0x79 };

            // Plaintext, 16 bytes string, utf-8
            string stringPt = "sixteen-byte-msg";

            // Raw Plaintext, bytes
            byte[] pt = Encoding.UTF8.GetBytes(stringPt);

            // Resulted Ciphertext will be here
            byte[] ct = new byte[16];

            // Create new AES instance
            using(Aes aes = new AesCryptoServiceProvider())
            {
                // Select Encryption mode
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                // Create encryptor with your key and zero IV
                var aesEncryptor = aes.CreateEncryptor(key, new byte[16]);
                
                // Transform one block
                ct = aesEncryptor.TransformFinalBlock(pt, 0, 16);

                // Get hex-string representation of Ciphertext
                string hex = BitConverter.ToString(ct);
                Console.WriteLine(hex.Replace("-", ""));
            }
        }
    }
}
```

c++
```cpp
#include <stdio.h> 
#include <openssl/aes.h>   

static const unsigned char key[] = {
    0x73, 0x69, 0x78, 0x74, 
    0x65, 0x65, 0x6e, 0x2d,
    0x62, 0x79, 0x74, 0x65,
    0x2d, 0x6b, 0x65, 0x79
};

int main()
{
    unsigned char text[]="sixteen-byte-msg";
    unsigned char enc_out[80];
    unsigned char dec_out[80];

    AES_KEY enc_key;

    AES_set_encrypt_key(key, 128, &enc_key);
    AES_encrypt(text, enc_out, &enc_key);      

    int i;

    for(i=0;*(enc_out+i)!=0x00;i++)
    {
       printf("%X ",*(enc_out+i));
    }
    printf("\n");

    return 0;
} 
```

python
```python
from Crypto.Cipher import AES
cipher = AES.new(b'sixteen-byte-key',AES.MODE_ECB)
cipher.encrypt(b'sixteen-byte-msg').hex()
```

## 2. Реализовать режимы ECB, CBC, CFB, OFB, CTR с использованием функции `ProcessBlock`.

Реализовать интерфейс

### `byte[] Encrypt(byte[] data, byte[] iv = null)` - ф-я зашифрования на заданном ключе

### `byte[] Decrypt(byte[] data, byte[] iv = null)` - ф-я расшифрования на заданном ключе, где

`data` - массив байт для шифрования

`iv` - вектор инициализации или начальное заполнение счётчика в указанном режиме. Если значение не передано, или передано значение null (или пустой массив), но режим требует использования IV или счётчика - значение должно быть сгенерировано (через отдельный метод, с использованием системного криптографически стойкого генератора).

Для генерации ключей и IV использовать стойкие стандартные генераторы. В качестве дополнения (для режима CBC) использовать PKCS7 padding (дополнение, см [википедия](https://en.wikipedia.org/wiki/Padding_(cryptography)#PKCS%235_and_PKCS%237))

**IV должен быть случайным для режимов CBC, CFB, OFB.**

**Режимы CBC и ECB должен быть выполнен с использованием `PKCS7` дополнения, режимы  CFB, OFB, CTR - без паддинга (`NON`), путём отбрасывания лишних битов гаммы.**

**Выбор начального заполнения счётчика для режима CTR должен быть согласован с [rfc3686](https://tools.ietf.org/html/rfc3686#page-7).**

При использовании IV (или счётчика) шифртекст дополняется им с начала сообщения.

`c = IV || E(k, m)`

## 2.5 Использовать реализации режима CBC в вашем языке программирования для валидации вашей реализации режима CBC.

## 3. Расшифровать следующие шифртексты:

Режим CBC и CTR. IV = 16 байт. IV добавлен к зашифрованному тексту в начале. CBC: PKCS7(PKCS5) padding, ascii


    CBC key: 140b41b22a29beb4061bda66b6747e14
    CBC Ciphertext 1: 4ca00ff4c898d61e1edbf1800618fb2828a226d160dad07883d04e008a7897ee2e4b7465d5290d0c0e6c6822236e1daafb94ffe0c5da05d9476be028ad7c1d81
    
    CBC key: 140b41b22a29beb4061bda66b6747e14
    CBC Ciphertext 2: 5b68629feb8606f9a6667670b75b38a5b4832d0f26e1ab7da33249de7d4afc48e713ac646ace36e872ad5fb8a512428a6e21364b0c374df45503473c5242a253
    
    CTR key: 36f18357be4dbd77f050515c73fcf9f2
    CTR Ciphertext 1: 69dda8455c7dd4254bf353b773304eec0ec7702330098ce7f7520d1cbbb20fc388d1b0adb5054dbd7370849dbf0b88d393f252e764f1f5f7ad97ef79d59ce29f5f51eeca32eabedd9afa9329
    
    CTR key: 36f18357be4dbd77f050515c73fcf9f2
    CTR Ciphertext 2: 770b80259ec33beb2561358a9f2dc617e46218c0a53cbeca695ae45faa8952aa0e311bde9d4e01726d3184c34451
    
## 4. Для каждого режима шифрования зашифровать и расшифровать произвольный текст длины 2,5 блока.



## PS. Небольшие спойлеры по лабе в части CTR, если очень сложно читать rfc

 * `nonce` - уникально в рамках одного соединения

 * `IV` уникально для каждого сообщения

 * `Block Counter` уникальность для каждого блока

```
The encryptor can generate the IV in any
manner that ensures uniqueness. Common approaches to IV generation
include incrementing a counter for each packet and linear feedback
shift registers (LFSRs).
```

 * если для `IV` и `counter` использовать счётчики - всё будет хорошо в плане безопасности

 * `nonce` нужно генерить случано, если следовать rfc, но один раз для одного соединения

 * пара `nonce`-`iv` действительно должна быть уникальной для всех сообщений при фиксированном ключе

При расшифровании разбивать счётчик на части не нужно. Можно трактовать первые 16 байт шт как начальное заполнение счётчика, которое используется для выработки гаммы для первого блока от. Для шифрования последующих блоков - начальное заполнение инкрементируется нужное число раз перед зашифрованием с целью получения гаммы для соответствующих блоков от.
