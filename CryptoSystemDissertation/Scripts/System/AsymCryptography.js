var RSACrypto = (function () {

    this.key = "";
    this.keySize = 1024 * 4;
    this.serverEndpoint = "/ImageBoard/ParametersImage/";

    this.GetRSAKeyPair = function () {
        var encryptActiveX = new ActiveXObject("csharpAx.Crypto");
        if (encryptActiveX != null) {
            return encryptActiveX.GetRSAKeyPair(this.keySize);
        }
        else {
            alert("Library not loaded. Please retrieve the dll file.");
            return null;
        }
    }
    this.GetNewRsaProvider = function () {
        return new System.Security.Cryptography.RSACryptoServiceProvider();
    }

    this.SendPublicKey = function (receiverID) {
        //rsa keys
        var rsa = GetNewRsaProvider();
        key = GetRSAKeyPair();
        rsa.FromXmlString(key);
        var publicKey = rsa.ToXmlString(false);
        var data = {
                ReceiverId: receiverID,
                ImageData: "",
                RSAKeyXML: publicKey
        }

        getEncryptedParams(data).then(function (data) {
            if (!data.encryptParam)
                return;

            var encryptionParams = Decrypt(data.encryptParam);
        })

    }

    this.Decrypt = function (encryptedBytes) {
        var xmlParams = key;

        //rsa keys
        var rsa = GetNewRsaProvider();
        rsa.FromXmlString(xmlParams);
        var decryptedBytes = rsa.Decrypt(System.Convert.FromBase64String(encryptedBytes), false);
        // ------------------------------------------------
        // Display the decrypted data.
        var decryptedString = System.Text.Encoding.UTF8.GetString(decryptedBytes);
        return decryptedString;
    }

    var getEncryptedParams = function (data) {

       var deferred = $.Deferred();

        $.ajax({
            type: "POST",
            url: this.serverEndpoint,
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                deferred.resolve(data);
            },
            error: function () {
                console.log("Error getting encrypted params...")
            }
        });
        return deferred.promise();
    }

    return {
        SendPublicKey: this.SendPublicKey,
        Decrypt: this.Decrypt
    }

}());
