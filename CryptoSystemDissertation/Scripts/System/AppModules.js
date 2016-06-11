var ChaosParams = (function () {
    this.lambda = "";
    this.x = "";

    this.SetParams = function (lambda, x) {
        ChaosParams.lambda = lambda;
        ChaosParams.x = x;
    }

    this.GetLambda = function () {
        return ChaosParams.lambda;
    }

    this.GetX = function () {
        return ChaosParams.x;
    }

    return {
        SetParams: SetParams,
        GetLambda: GetLambda,
        GetX:GetX
    }

}());

var ChaoticImage = (function () {

    this.chaoticImage = "";

    this.SetImage = function (imageData) {
        ChaoticImage.chaoticImage = imageData;
    }

    this.GetImage = function () {
        return ChaoticImage.chaoticImage;
    }

    return {
        SetImage: SetImage,
        GetImage:GetImage
    }
}())

var RSACrypto = (function () {

    this.key = "";
    this.keySize = 1024 * 2;
    this.serverGetEncryptParamsEndpoint = "/ImageBoard/ParametersImage/";
    this.serverSendEncryptedImageEndpoint = "/ImageBoard/ImageString/"

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
                Image: "",
                RSAKeyXML: publicKey
        }

        getEncryptedParams(data).then(function (data) {
            if (!data.encryptParam)
                return;

            var encryptionParams = Decrypt(data.encryptParam);
            var $encryptionParamsXml = $($.parseXML(encryptionParams));
            var lambda = $encryptionParamsXml.find("Lambda")[0].textContent;
            var x = $encryptionParamsXml.find("X")[0].textContent;
            ChaosParams.SetParams(lambda, x);
            Crypto.Encrypt();
            $("#btnSendToReceiver").removeClass("hidden");
        })
    }

    this.SendEncryptedImage = function (receiverID) {
        var data = {
            ReceiverId: receiverID,
            Image: ChaoticImage.GetImage(),
            RSAKeyXML: ""
        }

        sendEncryptedImage(data).then(function (data) {
            var x = data;
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
            url: this.serverGetEncryptParamsEndpoint,
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

    var sendEncryptedImage = function (data) {

        var deferred = $.Deferred();

        $.ajax({
            type: "POST",
            url: this.serverSendEncryptedImageEndpoint,
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                deferred.resolve(data);
            },
            error: function () {
                console.log("Error sending encrypted image...")
            }
        });
        return deferred.promise();
    }

    return {
        SendPublicKey: this.SendPublicKey,
        SendEncryptedImage: this.SendEncryptedImage,
        Decrypt: this.Decrypt
    }

}());

var Crypto = (function () {

    var Encrypt = function (e) {

        var encryptActiveX = new ActiveXObject("csharpAx.Crypto");
        if (encryptActiveX != null) {
            encryptActiveX.SetEncryptionParameters(ChaosParams.GetX(), ChaosParams.GetLambda());
            var header = $('.preview-initial').attr('src').split(",")[0];
            var confussedImageSrc = header + "," + encryptActiveX.EncryptImage($('.preview-initial').attr('src').split(",")[1]);
            ChaoticImage.SetImage(confussedImageSrc);
            $('.preview-encrypted').attr('src', confussedImageSrc).width("100%");
        }
        else
            alert("Library not loaded. Please retrieve the dll file.");
    }

    var Decrypts = function (e) {
        var decryptActiveX = new ActiveXObject("csharpAx.Crypto");
        if (decryptActiveX != null) {
            decryptActiveX.SetEncryptionParameters("0.74", "3.93695629843");
            var header = $('.preview-initial').attr('src').split(",")[0];
            var decryptedImageSrc = header + "," + decryptActiveX.DecryptImage($('.preview-initial').attr('src').split(",")[1]);
            $('.preview-encrypted').attr('src', decryptedImageSrc).width("100%");
        }
        else
            alert("Library not loaded. Please retrieve the dll file.");
    }

    return {
        Encrypt: Encrypt
    }

}());

var ImageProcessor = (function () {

    var showImage = function (input, receiverId) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('.preview-initial')
                  .attr('src', e.target.result)
                  .width("100%")
            };
            reader.readAsDataURL(input.files[0]);
            RSACrypto.SendPublicKey(receiverId);
        }
    }

    return {
        showImage: showImage,
    }
}());
