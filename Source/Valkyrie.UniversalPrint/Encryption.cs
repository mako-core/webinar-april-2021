// -----------------------------------------------------------------------
//  <copyright file="Encryption.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2020 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

namespace Valkyrie.UniversalPrint
{
    public static class Encryption
    {
        private static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            // Generate private/public key pair
            var generator = new RsaKeyPairGenerator();
            var keyParams = new KeyGenerationParameters(new SecureRandom(), 2048);

            generator.Init(keyParams);

            return generator.GenerateKeyPair();
        }

        private static string RemovePemHeaderFooter(string input)
        {
            var headerFooterList = new List<string>
            {
                "-----BEGIN CERTIFICATE REQUEST-----",
                "-----END CERTIFICATE REQUEST-----",
                "-----BEGIN PUBLIC KEY-----",
                "-----END PUBLIC KEY-----",
                "-----BEGIN RSA PRIVATE KEY-----",
                "-----END RSA PRIVATE KEY-----"
            };

            var trimmed = input;
            foreach (var hf in headerFooterList)
            {
                trimmed = trimmed.Replace(hf, string.Empty);
            }

            return trimmed.Replace("\r\n", string.Empty);
        }

        private static string GenerateCertRequest(AsymmetricCipherKeyPair keyPair)
        {
            var values = new Dictionary<DerObjectIdentifier, string> {
                {X509Name.CN, "Sample Company"},
                {X509Name.O, "Sample Corp."},
                {X509Name.L, "Seattle"},
                {X509Name.ST, "Washington"},
                {X509Name.C, "US"},
            };

            var subject = new X509Name(values.Keys.Reverse().ToList(), values);
            var csr = new Pkcs10CertificationRequest(new Asn1SignatureFactory("SHA256withRSA", keyPair.Private), subject, keyPair.Public, null);

            // Convert BouncyCastle csr to PEM format
            var csrPem = new StringBuilder();
            var csrPemWriter = new PemWriter(new StringWriter(csrPem));

            csrPemWriter.WriteObject(csr);
            csrPemWriter.Writer.Flush();

            return RemovePemHeaderFooter(csrPem.ToString());
        }

        public static (string csrData, string transportKey) CreateCsr()
        {
            var keyPair = GenerateKeyPair();
            var keyPem = new StringBuilder();
            var keyPemWriter = new PemWriter(new StringWriter(keyPem));

            keyPemWriter.WriteObject(keyPair.Public);
            keyPemWriter.Writer.Flush();

            var transportKey = RemovePemHeaderFooter(keyPem.ToString());
            var csrData = GenerateCertRequest(keyPair);

            return (csrData, transportKey);
        }
    }
}