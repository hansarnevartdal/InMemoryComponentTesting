using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;

namespace ComponentBoundaries.Testing.Http.Auth0.Helpers
{
    public static class SignatureHelper
    {
        public static Tuple<SigningCredentials,X509Certificate2> GenerateSigningInformation()
        {
            var kp = GenereateKeys();
            var gen = new X509V3CertificateGenerator();

            var certName = new X509Name("CN=Test_" + Guid.NewGuid());
            var serialNo = BigInteger.ProbablePrime(120, new Random());

            gen.SetSerialNumber(serialNo);
            gen.SetSubjectDN(certName);
            gen.SetIssuerDN(certName);
            gen.SetNotAfter(DateTime.Now.AddYears(100));
            gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
            gen.SetPublicKey(kp.Public);

            gen.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id,
                false,
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kp.Public),
                    new GeneralNames(new GeneralName(certName)),
                    serialNo));

            gen.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id,
                false,
                new ExtendedKeyUsage(new ArrayList() { new DerObjectIdentifier("1.3.6.1.5.5.7.3.1") }));

            var signatureFactory = new Asn1SignatureFactory("MD5WithRSA", kp.Private);
            var x509Certificate = new X509Certificate2(gen.Generate(signatureFactory).GetEncoded());
            var rsa = ToRsaParameters((RsaPrivateCrtKeyParameters)kp.Private);
            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256Signature);
            return new Tuple<SigningCredentials, X509Certificate2>(signingCredentials, x509Certificate);
        }

        private static AsymmetricCipherKeyPair GenereateKeys()
        {
            var kpgen = new RsaKeyPairGenerator();
            kpgen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            return kpgen.GenerateKeyPair();
        }

        private static RSAParameters ToRsaParameters(RsaPrivateCrtKeyParameters privKey)
        {
            return new RSAParameters
            {
                Modulus = privKey.Modulus.ToByteArrayUnsigned(),
                Exponent = privKey.PublicExponent.ToByteArrayUnsigned(),
                D = privKey.Exponent.ToByteArrayUnsigned(),
                P = privKey.P.ToByteArrayUnsigned(),
                Q = privKey.Q.ToByteArrayUnsigned(),
                DP = privKey.DP.ToByteArrayUnsigned(),
                DQ = privKey.DQ.ToByteArrayUnsigned(),
                InverseQ = privKey.QInv.ToByteArrayUnsigned()
            };
        }
    }
}