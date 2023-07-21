using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using WebApi.DTOs;

namespace WebApi.Servicios
{
    public class HashService
    {
        public ResultadoHash Hash(string textoPlano)
        {
            var sal = new byte[16];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);  
            }
            return Hash(textoPlano,sal);

        }

        public ResultadoHash Hash(string textoPlano, byte[] sal)
        {
            /*KeyDerivation.Pbkdf2: Es un método que implementa el algoritmo PBKDF2
             * (Password-Based Key Derivation Function 2). PBKDF2 es un algoritmo de derivación de clave que toma 
             prf: KeyDerivationPrf.HMACSHA1: El parámetro prf especifica el algoritmo de firma de clave pseudorandom 
            (PRF) a utilizar. En este caso, se está utilizando HMACSHA1 como el PRF.

            iterationCount: 10000: El parámetro iterationCount determina el 
            número de iteraciones del algoritmo PBKDF2 que se realizarán. 
            Más iteraciones aumentan la seguridad, pero también requieren más tiempo de procesamiento.

            numBytesRequested: 32: El parámetro numBytesRequested indica el 
            tamaño en bytes de la clave derivada que se generará. En este caso, se está solicitando una clave de 32 bytes.
             */
            var llaveDerivada = KeyDerivation.Pbkdf2(password: textoPlano, salt: sal, prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(llaveDerivada);

            return new ResultadoHash()
            {
                Hash = hash,
                Sal = sal
            };
        }


    }
}
