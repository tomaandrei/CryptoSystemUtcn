using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CryptoSystemDissertation.BusinessLogic
{
    public class EncryptParameters<T>
    {
        private RSAParameters publicKey;
        private T plainText;
        private RSACryptoServiceProvider csp;

        public EncryptParameters(string publicKeyString, T plainText)
        {
            this.csp = new RSACryptoServiceProvider(2048);
            this.publicKey = ToRSAParameters(publicKeyString);
            this.plainText = plainText;
        }     

        public EncryptParameters(RSAParameters publicKey, T plainText)
        {
            this.publicKey = publicKey;
            this.plainText = plainText;
        }

        public string Encrypt()
        {
            csp.ImportParameters(publicKey);

            var bytesPlainTextData = Encoding.Unicode.GetBytes(GetPlainTextString());
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
            var cypherText = Convert.ToBase64String(bytesCypherText);

            return cypherText;
        }

        private string GetPlainTextString()
        {
            return Helper.Serialize(plainText);
        }

        private RSAParameters ToRSAParameters(string publicKeyString)
        {
            RSAParameters rsaParams;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(publicKeyString);
                rsaParams = RSA.ExportParameters(false);
            }
            return rsaParams;
        }
    }
}